namespace CashTrack.Models.AuthenticationModels;

public class AuthenticationModels
{
    public record Request
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    };

}