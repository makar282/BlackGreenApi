using BlackGreenApi.Application.Services.Interfaces;
using BlackGreenApi.Core.Models;

namespace BlackGreenApi.Application.Services
{
    public class UserService(
        ILogger<UserService> logger, IUserRepo userRepo, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, HttpClient httpClient) : IUserService

    {
        // сервисный метод регистрации
        public async Task Register(string userName, string password)
        {
            string passwordHash = passwordHasher.Hash(password);

            var user = User.Create(Guid.NewGuid(), userName, passwordHash);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            await userRepo.AddUserAsync(user);
        }

        // сервисный метод логина (возращает токен логина)
        public async Task<string> Login(string userName, string password)
        {
            var user = await userRepo.GetByUserName(userName);
            if (user == null)
            {
                logger.LogWarning("User not found: {UserName}", userName);
                throw new Exception("User not found");
            }

            var result = passwordHasher.VerifyAuth(password, user.PasswordHash);
            if (!result)
            {
                logger.LogWarning("Password verification failed for {UserName}", userName);
                throw new Exception("Invalid password");
            }

            var token = jwtProvider.GenerateToken(user);

            return token;
        }
    }
}