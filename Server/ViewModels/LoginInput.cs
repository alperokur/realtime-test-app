public class LoginInput
{
    public LoginInput(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}