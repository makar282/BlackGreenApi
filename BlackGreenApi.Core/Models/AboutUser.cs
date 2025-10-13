namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Таблица для aboutme
    /// </summary>
    public class AboutUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? AboutMe { get; set; }

        public User User { get; set; }
    }

}
