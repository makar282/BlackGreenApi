namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Эко-рейтинг
    /// </summary>
    public class EcoRating
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? AboutMe { get; set; } = null;
        public DateTime LastUpdated { get; set; }
        public required User User { get; set; }
    }
}