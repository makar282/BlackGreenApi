using BlackGreenApi.Application.Services.Interfaces;
using BlackGreenApi.Core.Models;
using BlackGreenApi.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace BlackGreenApi.Application.Services
{
    public class EcoRatingManager(ILogger<UserService> logger, ApplicationDbContext dbContext, HttpClient httpClient) : IEcoRatingManager
    {
        public async Task<int> CalculateAndSaveEcoRatingAsync(string _userName)
        {
            List<Receipt> receipts = await dbContext.Receipts
                 .Where(r => r.UserName == _userName)
                 .Include(r => r.Items)
                 .ThenInclude(i => i.Recommendation)
                 .ToListAsync();

            List<Item> items = [.. receipts.SelectMany(r => r.Items)];

            if (items == null || items.Count == 0)
            {
                return 50; // Нейтральный рейтинг, если нет товаров
            }

            // Подгружаем рекомендации для всех товаров
            List<int> recommendationIds = items
                 .Where(i => i.RecommendationId.HasValue)
                 .Select(i => i.RecommendationId.Value)
                 .Distinct()
                 .ToList();

            Dictionary<int, int> recommendations = await dbContext.Recommendations
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

        public async Task<int> GetEcoRatingAsync(string userName)
        {
            // Находим пользователя по UserName, чтобы получить его Id
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                logger.LogWarning("User {UserName} not found", userName);
                throw new ArgumentException("User not found");
            }

            // Ищем рейтинг по UserId
            var ecoRating = await dbContext.EcoRatings
                 .OrderByDescending(e => e.LastUpdated) // Берём самый свежий рейтинг
                 .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (ecoRating == null)
            {
                // Рассчитываем рейтинг, если он ещё не сохранён
                return await CalculateAndSaveEcoRatingAsync(userName);
            }

            return ecoRating.Rating;
        }

    }
}
