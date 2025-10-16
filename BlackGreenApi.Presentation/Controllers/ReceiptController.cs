using Microsoft.AspNetCore.Mvc;

namespace BlackGreenApi.Presentation.Controllers
{
	 [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController(HttpClient httpClient,
                             ApplicationDbContext dbContext,
                             ILogger<ReceiptController> logger,
                             IRecommendationMatcher recommendationMatcherService,
                             IReceiptApi apiService,
                             IParserJson parserJson
                                 ) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly ILogger<ReceiptController> _logger = logger;
        private readonly IReceiptApi _apiService = apiService;
        private readonly IRecommendationMatcher _recommendationMatcherService = recommendationMatcherService;
        private readonly IParserJson _parserJson = parserJson;
        private const string ApiUrl = "https://proverkacheka.com/api/v1/check/get";
        private const string ApiToken = "33812.cIfyRu0WPp22Y8btm";

        /// <summary>
        /// Пост-запрос добавления чека
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("add-receipt")]
        public async Task<IActionResult> AddReceipt([FromBody] QrCodeRequest request, string userName)
        {
            _logger.LogInformation("Received request: {Request}", System.Text.Json.JsonSerializer.Serialize(request));

            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("User must be authenticated");
                return Unauthorized("User must be logged in");
            }

            bool userExists = await _dbContext.Users.AnyAsync(predicate: u => u.UserName == userName);
            if (!userExists)
            {
                _logger.LogWarning("User {UserName} not found in AspNetUsers", userName);
                return BadRequest(new { errors = new { General = "User not found." } });
            }

            if (request == null)
            {
                _logger.LogError("Request is null");
                return BadRequest(new { errors = new { General = "Request body is null." } });
            }

            if (string.IsNullOrEmpty(request.QrRaw) && string.IsNullOrEmpty(request.QrUrl))
            {
                _logger.LogWarning("Both QrRaw and QrUrl are empty");
                return BadRequest(new { errors = new { General = "QR code data or URL is required." } });
            }

            try
            {
                var jsonData = await _apiService.FetchReceiptAsync(request);

                JObject parsed = JObject.Parse(jsonData);
                JToken? receiptContent = parsed["data"]?["json"];

                // Альтернативный путь для парсинга
                if (receiptContent == null)
                {
                    receiptContent = parsed["ticket"]?["document"]?["receipt"];
                }

                // Извлекаем totalSum
                string? totalSumStr = receiptContent["totalSum"]?.ToString();
                decimal totalSum = decimal.TryParse(totalSumStr,
                                                    System.Globalization.NumberStyles.Any,
                                                    System.Globalization.CultureInfo.InvariantCulture,
                                                    out decimal sum) ? sum / 100 : 0;

                // Извлекаем purchaseDate
                string? purchaseDateStr = receiptContent["dateTime"]?.ToString() ?? parsed["createdAt"]?.ToString();
                _logger.LogInformation("Extracted purchaseDateStr: {PurchaseDateStr}", purchaseDateStr ?? "null");
                DateTime purchaseDate;

                // Пробуем парсить как dd.MM.yyyy HH:mm:ss
                if (DateTime.TryParseExact(purchaseDateStr,
                                           "dd.MM.yyyy HH:mm:ss",
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.AssumeUniversal,
                                           out DateTime date))
                {
                    purchaseDate = date.ToUniversalTime();
                }
                else
                {
                    _logger.LogWarning("purchaseDateStr is null or empty. Using DateTime.UtcNow.");
                    purchaseDate = DateTime.UtcNow;
                }

                Receipt receipt = new Receipt
                {
                    TotalAmount = totalSum,
                    PurchaseDate = purchaseDate,
                    CreatedAt = DateTime.UtcNow,
                    UserName = userName
                };

                _dbContext.Receipts.Add(receipt);
                await _dbContext.SaveChangesAsync();

                _ = _parserJson.ParseReceiptDataAndSaveItems(receipt, jsonData);

                _logger.LogInformation("Receipt and items added successfully for receipt ID {ReceiptId}", receipt.Id);
                return Ok(new { message = "Чек и товары успешно добавлены!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding receipt");
                return StatusCode(500, new { errors = new { ServerError = ex.Message } });
            }
        }


        [HttpGet("get-receipts")]
        public async Task<IActionResult> GetReceipts()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("UserName must be authenticated");
                return BadRequest("User must be logged in");
            }

            string? userName = User.Identity.Name;
            // LINQ - запрос
            List<Receipt> receipts = await _dbContext.Receipts
                        .Where(r => r.UserName == userName)
                        .Include(r => r.Items)

                        // Метод ThenInclude используется для загрузки данных, связанных с уже подгруженными данными такими как Recommendation
                        .ThenInclude(i => i.Recommendation)
                        .OrderByDescending(r => r.CreatedAt)
                        .ToListAsync();

            var result = receipts.Select(r => new
            {
                Id = r.Id,
                PurchaseDate = r.PurchaseDate.ToString("o"), // ISO 8601 формат, например, 2024-07-16T15:55:00.0000000Z
                Items = r.Items.Select(i => new
                {
                    i.ProductName,
                    EcoScore = i.Recommendation?.EcoScoreRecomendation ?? 0,
                    RecommendationText = i.Recommendation?.RecommendationText ?? "Нет рекомендации"
                }).ToList()
            }).ToList();

            _logger.LogInformation("Retrieved receipts for user: {UserName}", userName);
            return Ok(result);
        }

        [HttpGet("get-ecorating")]
        public async Task<IActionResult> GetEcoRating()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("UserName must be authenticated");
                return BadRequest("User must be logged in");
            }

            string? userName = User.Identity.Name;
            List<Receipt> receipts = await _dbContext.Receipts
                .Where(r => r.UserName == userName)
                .Include(r => r.Items)
                .ThenInclude(i => i.Recommendation)
                .ToListAsync();

            List<Item> allItems = receipts.SelectMany(r => r.Items).ToList();
            int ecoRating = await CalculateEcoRating(allItems);

            return Ok(new { EcoRating = ecoRating });
        }

        private async Task<int> CalculateEcoRating(List<Item> items)
        {
            if (items == null || items.Count == 0) return 50; // Нейтральный рейтинг, если нет товаров

            // Подгружаем рекомендации для всех товаров
            List<int> recommendationIds = items
                .Where(i => i.RecommendationId.HasValue)
                .Select(i => i.RecommendationId.Value)
                .Distinct()
                .ToList();
            Dictionary<int, int> recommendations = await _dbContext.Recommendations
                .Where(r => recommendationIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id, r => r.EcoScoreRecomendation);

            // Вычисляем средний EcoScore
            double totalEcoScore = 0;
            foreach (Item item in items)
            {
                int ecoScore = item.RecommendationId.HasValue && recommendations.TryGetValue(item.RecommendationId.Value, out int score)
                    ? score
                    : 0; // 0 для товаров без рекомендаций
                totalEcoScore += ecoScore;
            }

            double averageEcoScore = totalEcoScore / items.Count;
            // Нормализуем из [-100, 100] в [0, 100]
            int normalizedRating = (int)Math.Round((averageEcoScore + 100) / 2); // Пример: -100 -> 0, 0 -> 50, 100 -> 100
            return normalizedRating;
        }
    }

}

//"_id": "4807c876cb05c79b88473e6e" - id чека
//"createdAt": "2025-05-12T18:10:00+03:00" - дата создания
//"totalSum": 35100 - суммарное Итого чека   (делить на 100)
//"price": 11800 - внутри "items" - цена каждого из товаров  (делить на 100)
//"name": "ARAVIA Laboratories Крем для лица увлажняющий с гиалуроновой кислотой Hyaluron Filler Hydrating Cream, 50 мл" тоже в "items" - наименование каждой покупки.
//в items может быть несколько товаров