public class ClientConnectionLog
{
    public ClientConnectionLog(Guid clientConnectionId, string hubConnectionId, string ipAddress, string hostName, string macAddress, string type)
    {
        ConnectionDate = DateTime.Now;
        ClientConnectionId = clientConnectionId;
        HubConnectionId = hubConnectionId;
        IpAddress = ipAddress;
        HostName = hostName;
        MacAddress = macAddress;
        Type = type;
    }

    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime ConnectionDate { get; set; }
    public Guid ClientConnectionId { get; set; }
    [Required]
    public string HubConnectionId { get; set; }
    public string? IpAddress { get; set; }
    public string? HostName { get; set; }
    public string? MacAddress { get; set; }
    public string? Type { get; set; }
}