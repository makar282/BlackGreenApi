using Microsoft.AspNetCore.Identity;

namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User(Guid id, string userName, string passwordHash)
    {
        public Guid Id { get; set; } = id;
        public string UserName { get; private set; } = userName;
        public string PasswordHash { get; private set; } = passwordHash;

        public static User Create(Guid id, string userName, string passwordHash)
        {
            return new User(id, userName, passwordHash);
        }

        // Методы для изменения — они могут валидировать данные
        public void UpdateUserName(string newUserName)
        {
            if (string.IsNullOrWhiteSpace(newUserName)) throw new ArgumentException("UserName");
            UserName = newUserName;
        }

        public void UpdatePasswordHash(string newHash)
        {
            if (string.IsNullOrWhiteSpace(newHash)) throw new ArgumentException("PasswordHash");
            PasswordHash = newHash;
        }
    }
}

