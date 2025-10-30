//using BlackGreenApi.Application.Services.Interfaces;
//using BlackGreenApi.Core.Models;
//using Newtonsoft.Json.Linq;

//namespace BlackGreenApi.Application.Services
//{
//    public class ParserJsonService(ApplicationDbContext dbContext,
//        ILogger<RecommendationMatcherService> logger,
//        IRecommendationMatcher recommendationMatcher) : IParserJson
//    {
//        private readonly ApplicationDbContext _dbContext = dbContext;
//        private readonly ILogger<RecommendationMatcherService> _logger = logger;
//        private readonly IRecommendationMatcher _recommendationMatcherService = recommendationMatcher;

//        private async Task ParseReceiptDataAndSaveItems(Receipt receipt, string responseContent)
//        {
//            if (string.IsNullOrWhiteSpace(responseContent))
//            {
//                _logger.LogWarning("Response content is empty for receipt ID {ReceiptId}", receipt.Id);
//                return;
//            }

//            try
//            {
//                JObject parsed = JObject.Parse(responseContent);
//                JToken? items = parsed["data"]?["json"]?["items"];

//                // Альтернативный путь для парсинга
//                if (items == null)
//                {
//                    items = parsed["ticket"]?["document"]?["receipt"]?["items"];
//                }

//                if (items != null)
//                {
//                    foreach (JToken item in items)
//                    {
//                        string? name = item["name"]?.ToString();
//                        string? priceStr = item["price"]?.ToString();

//                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(priceStr))
//                        {
//                            _logger.LogWarning("Invalid item data in receipt ID {ReceiptId}: name or price is missing", receipt.Id);
//                            continue;
//                        }

//                        decimal.TryParse(priceStr, out decimal price);

//                        // Подбор рекомендации
//                        Recommendation? recommendation = await _recommendationMatcherService.GetRecommendationAsync(name);

//                        _dbContext.Items.Add(new Item
//                        {
//                            Receipt = receipt,
//                            ReceiptId = receipt.Id,
//                            ProductName = name,
//                            Price = price / 100,
//                            RecommendationId = recommendation?.Id
//                        });
//                    }

//                    await _dbContext.SaveChangesAsync();
//                    _logger.LogInformation("Items saved for receipt ID {ReceiptId}", receipt.Id);
//                }
//                else
//                {
//                    _logger.LogWarning("No items found in response for receipt ID {ReceiptId}", receipt.Id);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error parsing receipt data for receipt ID {ReceiptId}", receipt.Id);
//            }
//        }

//        Task IParserJson.ParseReceiptDataAndSaveItems(Receipt receipt, string responseContent)
//        {
//            return ParseReceiptDataAndSaveItems(receipt, responseContent);
//        }
//    }
//}
