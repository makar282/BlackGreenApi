using BlackGreenApi.Application.Services.Interfaces;

namespace BlackGreenApi.Application.Services
{
    public class UserService(HttpClient httpClient,
                            ApplicationDbContext dbContext,
                            ILogger<UserService> logger,
                            IUserRepo userRepo,
                            IPasswordHasher passwordHasher,
                            IJwtProvider jwtProvider) : IUserService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        public async Task Register(string userName, string password)
        {
            string passwordHash = _passwordHasher.Hash(password);

            var user = User.Create(Guid.NewGuid(), userName, passwordHash);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            await _userRepo.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> Login(string userName, string password)
        {
            var user = await _userRepo.GetByUserName(userName);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserName}", userName);
                throw new Exception("User not found");
            }

            var result = _passwordHasher.Verify(password, user.PasswordHash);
            if (!result)
            {
                _logger.LogWarning("Password verification failed for {UserName}", userName);
                throw new Exception("Invalid password");
            }

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }

        public async Task<int> CalculateAndSaveEcoRatingAsync(string _userName)
        {
            List<Receipt> receipts = await _dbContext.Receipts
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

        public async Task<int> GetEcoRatingAsync(string userName)
        {
            // Находим пользователя по UserName, чтобы получить его Id
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                _logger.LogWarning("User {UserName} not found", userName);
                throw new ArgumentException("User not found");
            }

            // Ищем рейтинг по UserId
            var ecoRating = await _dbContext.EcoRatings
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
