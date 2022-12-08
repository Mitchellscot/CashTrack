namespace CashTrack.Models.UserModels;


public record User()
{
    public int Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
}
public record AddEditUser()
{
    public int? Id { get; init; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
public record ChangePassword()
{
    public string Username { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}