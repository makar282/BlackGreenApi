namespace BlackGreenApi.Application.Services.Interfaces
{
    public interface IPasswordHasher
    {
        bool VerifyAuth(string password, string hash);
        string Hash(string password);
    }
}