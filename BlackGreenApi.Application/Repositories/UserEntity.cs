using BlackGreenApi.Core.Models;

namespace BlackGreenApi.Application.Repositories
{
    internal class UserEntity(Guid id, string userName, string passwordHash) : User(id, userName, passwordHash)
    {
        public Guid Id { get; set; } = id;
        public string UserName { get; set; } = userName;
        public string PasswordHash { get; set; } = passwordHash;
    }
}