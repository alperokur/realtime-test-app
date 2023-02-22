[PrimaryKey(nameof(HubConnectionId))]
public class AccountConnection
{
    public AccountConnection(string hubConnectionId)
    {
        ConnectionDate = DateTime.Now;
        HubConnectionId = hubConnectionId;
    }

    [Required]
    public DateTime ConnectionDate { get; set; }
    public Guid ClientConnectionId { get; set; }
    [Required]
    public string HubConnectionId { get; set; }
    public int AccountId { get; set; }
}