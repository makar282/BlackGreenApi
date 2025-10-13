namespace SaveNature.Infrastructure
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}