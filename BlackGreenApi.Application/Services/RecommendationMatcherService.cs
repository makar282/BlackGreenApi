//using BlackGreenApi.Application.Services.Interfaces;
//using BlackGreenApi.Core.Models;

//namespace BlackGreenApi.Application.Services
//{
//    public class RecommendationMatcherService : IRecommendationMatcher
//    {
//        private readonly ApplicationDbContext _dbContext;
//        private readonly ILogger<RecommendationMatcherService> _logger;


//        public RecommendationMatcherService(ApplicationDbContext dbContext, ILogger<RecommendationMatcherService> logger)
//        {
//            _dbContext = dbContext;
//            _logger = logger;
//        }

//        public async Task<Recommendation?> GetRecommendationAsync(string productName)
//        {
//            if (string.IsNullOrWhiteSpace(productName))
//            {
//                _logger.LogWarning("Product name is null or empty");
//                return null;
//            }

//            var recommendation = await _dbContext.Recommendations
//                .Where(r => productName.ToLower().Contains(r.Purchase.ToLower()))
//                .FirstOrDefaultAsync();

//            if (recommendation == null)
//            {
//                _logger.LogInformation("No recommendation found for product: {ProductName}", productName);
//            }

//            return recommendation;
//        }
//    }
//}