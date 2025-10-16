using BlackGreenApi.Core.Models;

namespace BlackGreenApi.Application.Services.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}