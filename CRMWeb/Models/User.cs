namespace CRMWeb.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }

        public ICollection<UserProduct>? UserProducts { get; set; }
    }
}
