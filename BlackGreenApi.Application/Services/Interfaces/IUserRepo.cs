using BlackGreenApi.Core.Models;

namespace BlackGreenApi.Application.Services.Interfaces
{
    public interface IUserRepo
    {
        Task Add(User user);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetByUserName(string userName);
        Task<User> ResetPasswordAsync(string password);
    }
}