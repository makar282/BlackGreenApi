using SaveNature.Services.Interfaces;

namespace SaveNature.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepo(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task Add(User user)
        {
            var userEntity = new UserEntity
            {
                UserName = user.UserName,
                PasswordHash = _passwordHasher.Hash(user.PasswordHash),
                Email = user.Email,
                Role = user.Role,
                ApiToken = user.ApiToken
            };

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByUserName(string userName)
        {
            var userEntity = await _context.Users
                 .AsNoTracking()
                 .FirstOrDefaultAsync(u => u.UserName == userName);

            if (userEntity == null)
                throw new KeyNotFoundException($"User with username '{userName}' not found.");

            return new User
            {
                Id = userEntity.Id,
                UserName = userEntity.UserName,
                PasswordHash = userEntity.PasswordHash
            };
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var usersEntity = await _context.Users
                 .AsNoTracking()
                 .ToListAsync();

            return usersEntity.Select(u => new User
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role,
                ApiToken = u.ApiToken,
                PasswordHash = u.PasswordHash
            }).ToList();
        }

        public async Task ResetPasswordAsync(int userId, string newPassword)
        {
            var userEntity = await _context.Users.FindAsync(userId);
            if (userEntity == null)
                throw new KeyNotFoundException("Пользователь не найден");

            userEntity.PasswordHash = _passwordHasher.Hash(newPassword);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var userEntity = await _context.Users.FindAsync(user.Id);
            if (userEntity == null) throw new KeyNotFoundException("Пользователь не найден");

            userEntity.UserName = user.UserName;
            userEntity.Email = user.Email;
            userEntity.Role = user.Role;
            userEntity.ApiToken = user.ApiToken;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var userEntity = await _context.Users.FindAsync(userId);
            if (userEntity == null) throw new KeyNotFoundException("Пользователь не найден");

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();
        }
    }
}