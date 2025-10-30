using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlackGreenApi.Core.Models;

namespace BlackGreenApi.Application.Repositories
{
	 [Table("Users")] // имя таблицы в базе, можно опустить, если совпадает с DbSet
	 public class UserEntity : User
	 {
		  [Key] // первичный ключ
		  public new Guid Id { get; set; } // "new" потому что базовый User.Id private

		  [Required]
		  [MaxLength(100)]
		  public new string UserName { get; set; }

		  [Required]
		  public new string PasswordHash { get; set; }

		  public string ApiToken { get; set; }

		  public UserEntity() { } // нужен для EF

		  public UserEntity(Guid id, string userName, string passwordHash) : base(id, userName, passwordHash)
		  {
				Id = id;
				UserName = userName;
				PasswordHash = passwordHash;
		  }
	 }
}
