namespace CashTrack.Models.AuthenticationModels;

public class AuthenticationModels
{
    public record Request(string UserName, string Password);

}