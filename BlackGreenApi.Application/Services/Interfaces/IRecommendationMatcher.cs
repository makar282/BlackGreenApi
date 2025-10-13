namespace SaveNature.Services.Interfaces
{
    public interface IRecommendationMatcher
    {
        Task<Recommendation?> GetRecommendationAsync(string productName);
    }
}
