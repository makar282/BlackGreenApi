namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Поля пользователя для редактирования
    /// </summary>
    public class EditProfileViewModel
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? AboutMe { get; set; }
    }
}