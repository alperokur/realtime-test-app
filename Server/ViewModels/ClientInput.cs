public class ClientInput
{
    public ClientInput(string ipAddress, string hostName, string macAddress)
    {
        IpAddress = ipAddress;
        HostName = hostName;
        MacAddress = macAddress;
    }

    [Required]
    public string? IpAddress { get; set; }
    [Required]
    public string? HostName { get; set; }
    [Required]
    public string? MacAddress { get; set; }
}