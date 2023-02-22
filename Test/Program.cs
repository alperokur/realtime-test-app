HttpClient client = new HttpClient();
using HttpResponseMessage response = await client.GetAsync("http://checkip.dyndns.org");
using HttpContent content = response.Content;
var json = await content.ReadAsStringAsync();
var ipAddress = MyRegex().Match(json).Value;
var guid = Guid.NewGuid();

string 
    serverAddress = "http://localhost:5224";
var 
    publicHub = 
    new HubConnectionBuilder()
    .WithUrl(serverAddress + "/connect/publichub")
    .Build();

string macAddress = NetworkInterface
    .GetAllNetworkInterfaces()
    .Where( 
    nic => nic.OperationalStatus == OperationalStatus.Up &&
    nic.NetworkInterfaceType != NetworkInterfaceType.Loopback )
    .Select( nic => nic.GetPhysicalAddress().ToString() )
    .FirstOrDefault()!;

try
{
    await publicHub.StartAsync();
}
catch (Exception ex)
{
    Debug.WriteLine(ex.ToString());
}

switch (publicHub.State)
{
    case HubConnectionState.Connected:
        {
            Console.WriteLine("\nPublicHub bağlantısı sağlandı.");

            await publicHub.SendAsync("ClientConnectionInfo", guid, ipAddress, Dns.GetHostName(), macAddress);

            Console.Write("\nUsername: ");
            string username = Console.ReadLine()!;
            Console.Write("Password: ");
            string password = Console.ReadLine()!;

            await UserLogin(username, password);

            Console.Read();
            break;
        }

    case HubConnectionState.Disconnected:
        Console.WriteLine("\nPublicHub bağlantısı kesildi/kurulamadı.");
        break;
    case HubConnectionState.Connecting:
        Console.WriteLine("\nPublicHub'a bağlanılıyor'..");
        break;
    case HubConnectionState.Reconnecting:
        Console.WriteLine("\nPublicHub'a tekrardan bağlanılıyor...");
        break;
    default:
        break;
}

Token token;

async Task<bool> UserLogin(string username, string password)
{
    try
    {
        HttpClient client = new();
        using var response = await client.PostAsync(
            serverAddress + "/login",
            new StringContent(
                JsonConvert.SerializeObject(
                    new Login { Username = username, Password = password }),
        Encoding.UTF8, "application/json"));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine("Giriş başarılı!");
            token = JsonConvert.DeserializeObject<Token>(responseContent)!;
            var privateHub = 
                new HubConnectionBuilder().WithUrl(
                    serverAddress + "/connect/privatehub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token.AccessToken);
            }).Build();

            try
            {
                await privateHub.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            switch (privateHub.State)
            {
                case HubConnectionState.Connected:
                    {
                        //Console.WriteLine("\nUserHub bağlantısı sağlandı.");
                        await privateHub.SendAsync("UserConnectionInfo", guid, token.AccessToken);

                        privateHub.Closed += async (error) =>
                        {
                            await privateHub.StartAsync();
                        };

                        Console.Read();
                        break;
                    }

                case HubConnectionState.Disconnected:
                    Console.WriteLine("nPublicHub bağlantısı kesildi/kurulamadı.");
                    break;
                case HubConnectionState.Connecting:
                    Console.WriteLine("nPublicHub'a bağlanılıyor'..");
                    break;
                case HubConnectionState.Reconnecting:
                    Console.WriteLine("nPublicHub'a tekrardan bağlanılıyor...");
                    break;
                default:
                    break;
            }
            return true;
        }
        else{
            var error = JsonConvert.DeserializeObject<InternalServerError>(responseContent)!;
            Console.WriteLine(error.Detail);
        }
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("Sunucu ile bağlantı kurulamadı.");
        Debug.WriteLine(e.InnerException!.Message);
    }
    return false;
}

public class Login
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class Token
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
}

public class InternalServerError
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int Status { get; set; }
    public string? Detail { get; set; }
}

partial class Program
{
    [GeneratedRegex("\\b\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\b")]
    private static partial Regex MyRegex();
}