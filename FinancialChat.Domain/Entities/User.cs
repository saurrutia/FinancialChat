namespace FinancialChat.Domain.Entities
{
    public class User : Entity
    {
        public User(int id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }

        public int Id { get; protected set; }
        public string Username { get; protected set; }
        public string Password { get; protected set; }
    }
}
