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
    public record AddEditUser()
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

