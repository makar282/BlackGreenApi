namespace SaveNature.Services.Interfaces
{
    public interface IUserService
    {
        Task<int> GetEcoRatingAsync(string userName);
        Task<int> CalculateAndSaveEcoRatingAsync(string userName);
        Task Register(string _userName, string _passwordHash);
        Task<string> Login(string userName, string passwordHash);
    }
}
