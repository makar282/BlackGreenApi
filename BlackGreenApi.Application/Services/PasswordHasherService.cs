using BCrypt.Net;
using BlackGreenApi.Application.Services.Interfaces;

namespace BlackGreenApi.Application.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        /// Возвращает сгенерированный хэш
        public string Hash(string password)
        {
            ArgumentNullException.ThrowIfNull(password);

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            return passwordHash;
        }

        /// Проверяет авторизацию
        public bool VerifyAuth(string password, string passwordHash)
        {
            ArgumentNullException.ThrowIfNull(password);

            if (string.IsNullOrEmpty(passwordHash))
            {
                Console.WriteLine("Password hash is empty or null");
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
            }
            catch (SaltParseException ex)
            {
                Console.WriteLine($"BCrypt verification failed: {ex.Message}");
                return false;
            }
        }
    }
}