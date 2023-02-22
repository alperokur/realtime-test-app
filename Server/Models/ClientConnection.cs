[PrimaryKey(nameof(HubConnectionId))]
public class ClientConnection
{
    public ClientConnection(string hubConnectionId)
    {
        ConnectionDate = DateTime.Now;
        HubConnectionId = hubConnectionId;
    }

    [Required]
    public DateTime ConnectionDate { get; set; }
    public Guid ClientConnectionId { get; set; }
    [Required]
    public string HubConnectionId { get; set; }
    public string? IpAddress { get; set; }
    public string? HostName { get; set; }
    public string? MacAddress { get; set; }
}