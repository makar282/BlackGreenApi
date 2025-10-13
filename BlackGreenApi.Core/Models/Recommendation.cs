namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Рекомендация к товару
    /// </summary>
    public class Recommendation(int id, string purchase, string recommendationText, int ecoScoreRecomendation)
    {
        public int Id { get; set; } = id;
        public string Purchase { get; set; } = purchase;
        public string? RecommendationText { get; set; } = recommendationText;
        public int EcoScoreRecomendation { get; set; } = ecoScoreRecomendation;

        public static Recommendation Create(int id, string purchase, string recommendationText, int ecoScoreRecomendation)
        {
            return new Recommendation(id, purchase, recommendationText, ecoScoreRecomendation);
        }
    }
}
