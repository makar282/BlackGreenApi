using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackGreenApi.Presentation.Controllers
{
	 [Authorize]
    [Route(template: "api/receipt")]
    [ApiController]
    public class DashboardController(ILogger<DashboardController> logger, ApplicationDbContext dbContext) : ControllerBase
    {
        private readonly ILogger<DashboardController> _logger = logger;
        private readonly ApplicationDbContext _dbContext = dbContext;


        // GET api/receipt/purchase-types
        [HttpGet("purchase-types")]
        public async Task<IActionResult> GetPurchaseTypes()
        {
            try
            {
                // проверяем, что пользователь вошёл в систему
                if (User?.Identity?.IsAuthenticated != true)
                    return Unauthorized("User must be logged in");

                string? userName = User.Identity!.Name;

                // выбираем все товары текущего пользователя с их рекомендациями
                IQueryable<Models.Item> itemsQuery = _dbContext.Items
                                            .Include(i => i.Recommendation)
                                            .Include(i => i.Receipt)
                                            .Where(i => i.Receipt!.UserName == userName);

                int positive = await itemsQuery
                    .CountAsync(i => i.Recommendation != null && i.Recommendation.EcoScoreRecomendation > 0);

                int negative = await itemsQuery
                    .CountAsync(i => i.Recommendation != null && i.Recommendation.EcoScoreRecomendation < 0);


                int total = await itemsQuery.CountAsync();
                int neutral = total - positive - negative;

                _logger.LogInformation("Purchase‑types: +{Pos}, 0:{Neu}, -{Neg}",
                                       positive, neutral, negative);

                return Ok(new
                {
                    positiveCount = positive,
                    neutralCount = neutral,
                    negativeCount = negative
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase types");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }


        [HttpGet("eco-spending")]
        public async Task<IActionResult> GetEcoSpending()
        {
            try
            {
                DateTime startDate = DateTime.UtcNow.AddYears(-1); // Последний год

                // Получаем чеки с товарами
                List<Models.Receipt> receiptsWithItems = await _dbContext.Receipts
                    .Include(r => r.Items)
                    .ThenInclude(i => i.Recommendation)
                    .Where(r => r.PurchaseDate >= startDate)
                    .ToListAsync();

                // Группируем по месяцам
                var groupedData = receiptsWithItems
                    .GroupBy(r => new { r.PurchaseDate.Year, r.PurchaseDate.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        PositiveEco = new
                        {
                            Count = g.Count(r => r.Items.Any(i => (i.Recommendation?.EcoScoreRecomendation ?? 0) > 0)),
                            Total = g.Where(r => r.Items.Any(i => (i.Recommendation?.EcoScoreRecomendation ?? 0) > 0))
                                     .Sum(r => r.TotalAmount)
                        },
                        NegativeEco = new
                        {
                            Count = g.Count(r => r.Items.Any(i => (i.Recommendation?.EcoScoreRecomendation ?? 0) < 0)),
                            Total = g.Where(r => r.Items.Any(i => (i.Recommendation?.EcoScoreRecomendation ?? 0) < 0))
                                     .Sum(r => r.TotalAmount)
                        }
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToList();

                var result = groupedData.Select(g => new
                {
                    Label = $"{g.Month:D2}.{g.Year}",
                    PositiveEcoCount = g.PositiveEco.Count,
                    PositiveEcoTotal = g.PositiveEco.Total,
                    NegativeEcoCount = g.NegativeEco.Count,
                    NegativeEcoTotal = g.NegativeEco.Total
                }).ToList();

                _logger.LogInformation("Eco spending data retrieved: {Count} records", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving eco spending");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}