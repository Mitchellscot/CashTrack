namespace CashTrack.Models.UserModels;

public class UserModels
{
    public record Response()
    {
        public int id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
    }
}

