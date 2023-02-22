public class Account
{
    public Account(string username, string password, string firstName, string lastName, string email, string phone)
    {
        Username = username;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        CreatedDate = DateTime.Now;
        Permission = 0;
        PermitLimit = 60;
        RateLimitWindowInMinutes = 5;
    }

    [Key]
    public int Id { get; set; }
    public string? Username { get; set; }
    public string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? Phone { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime DeletedDate { get; set; }
    public int Permission { get; set; }
    public int RateLimitWindowInMinutes { get; set; }
    public int PermitLimit { get; set; }
}
