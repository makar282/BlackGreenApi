using BlackGreenApi.Application.Services.Interfaces;

namespace BlackGreenApi.Application.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Возвращает сгенерированный хэш
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string Hash(string password)
        {
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            return passwordHash;
        }

        /// <summary>
        /// Проверяет авторизацию
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Verify(string password, string passwordHash)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

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