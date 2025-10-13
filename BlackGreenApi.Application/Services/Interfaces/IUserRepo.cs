namespace SaveNature.Services.Interfaces
{
    public interface IUserRepo
    {
        Task Add(User user);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetByUserName(string userName);
        public async Task<User> ResetPasswordAsync(string password)   }
}