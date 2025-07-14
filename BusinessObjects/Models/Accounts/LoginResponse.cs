namespace BusinessObjects.Models.Accounts
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public string Role { get; set; } = null!;
    }

}
