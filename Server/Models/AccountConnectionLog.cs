public class AccountConnectionLog
{
    public AccountConnectionLog(Guid clientConnectionId, string hubConnectionId, string type, int accountId)
    {
        ConnectionDate = DateTime.Now;
        ClientConnectionId = clientConnectionId;
        HubConnectionId = hubConnectionId;
        Type = type;
        AccountId = accountId;
    }

    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime ConnectionDate { get; set; }
    public Guid ClientConnectionId { get; set; }
    [Required]
    public string HubConnectionId { get; set; }
    [Required]
    public int AccountId { get; set; }
    [Required]
    public string? Type { get; set; }
}