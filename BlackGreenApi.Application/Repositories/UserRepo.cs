using BlackGreenApi.Application.Services.Interfaces;
using BlackGreenApi.Core.Models;
using BlackGreenApi.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace BlackGreenApi.Application.Repositories
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

        public async Task AddUserAsync(User user)
        {
            var userEntity = new UserEntity()
            {
                Id = user.Id,
                UserName = user.UserName,
                PasswordHash = _passwordHasher.Hash(user.PasswordHash),
                //ApiToken = user.ApiToken
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
            {
                throw new KeyNotFoundException($"User with username '{userName}' not found.");
            }

				return new User(
					 userEntity.Id,
					 userEntity.UserName,
					 userEntity.PasswordHash
				// если нужно, можно добавить ApiToken через отдельный метод или свойство
				);

		  }

		  public async Task<List<User>> GetAllUsersAsync()
        {
            var usersEntity = await _context.Users
                 .AsNoTracking()
                 .ToListAsync();

				return usersEntity.Select(u => new User(u.Id, u.UserName, u.PasswordHash)).ToList();
		  }

		  public async Task ResetPasswordAsync(int userId, string newPassword)
        {
            var userEntity = await _context.Users.FindAsync(userId);
            if (userEntity == null)
            {
                throw new KeyNotFoundException("Пользователь не найден");
            }

				userEntity.UpdatePasswordHash(_passwordHasher.Hash(newPassword));
				await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var userEntity = await _context.Users.FindAsync(user.Id);
            if (userEntity == null)
            {
                throw new KeyNotFoundException("Пользователь не найден");
            }

				userEntity.UpdateUser(user);
				//userEntity.ApiToken = user.ApiToken;

				await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var userEntity = await _context.Users.FindAsync(userId);
            if (userEntity == null)
            {
                throw new KeyNotFoundException("Пользователь не найден");
            }

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync();
        }

        Task<User> IUserRepo.ResetPasswordAsync(int userId, string password)
        {
            throw new NotImplementedException();
        }
    }
}