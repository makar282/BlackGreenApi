namespace BlackGreenApi.Core.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string PasswordHash { get; private set; }

        public User() { }

        public User(Guid id, string userName, string passwordHash)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
        }


        //string ApiToken { get; set; }

        public static User Create(Guid id, string userName, string passwordHash)
        {
            return new User(id, userName, passwordHash);
        }

        // Методы для изменения — они могут валидировать данные
        public void UpdateUserName(string newUserName)
        {
            if (string.IsNullOrWhiteSpace(newUserName))
            {
                throw new ArgumentException("UserName");
            }

            UserName = newUserName;
        }

        public void UpdatePasswordHash(string newHash)
        {
            if (string.IsNullOrWhiteSpace(newHash))
            {
                throw new ArgumentException("PasswordHash");
            }

            PasswordHash = newHash;
        }

		  public void UpdateUser(User user)
		  {
				if (user == null) throw new ArgumentNullException(nameof(user));

				UpdateUserName(user.UserName);
				UpdatePasswordHash(user.PasswordHash);
		  }
	 }
}