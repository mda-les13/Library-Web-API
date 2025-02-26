namespace Library.BusinessLogic.Models
{
    public class AuthenticateResponseModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
