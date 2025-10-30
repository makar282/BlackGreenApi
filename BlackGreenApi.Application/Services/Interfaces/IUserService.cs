namespace BlackGreenApi.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task Register(string _userName, string _passwordHash);
        Task<string> Login(string userName, string passwordHash);
    }
}
