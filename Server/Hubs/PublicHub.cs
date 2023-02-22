using Microsoft.Extensions.Configuration;

public class PublicHub : Hub
{
    DatabaseContext dbContext;
    IHubContext<PrivateHub> privateHub;


    public PublicHub(IDbContextFactory<DatabaseContext> dbContextFactory, IHubContext<PrivateHub> _privateHub)
    {
        dbContext = dbContextFactory.CreateDbContext();
        privateHub = _privateHub;
    }

    public override Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        var connection = new ClientConnection(connectionId);
        dbContext.ClientConnections.Add(connection);
        dbContext.SaveChangesAsync();
        return base.OnConnectedAsync();
    }

    [HubMethodName("ClientConnectionInfo")]
    public async Task ConnectionInfo(
        Guid clientConnectionId, 
        ClientInput clientInput, 
        IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        if (await dbContext.ClientConnections.FirstOrDefaultAsync(
            x => x.HubConnectionId == Context.ConnectionId) is ClientConnection connection)
        {
            connection.ClientConnectionId = clientConnectionId;
            connection.IpAddress = clientInput.IpAddress;
            connection.HostName = clientInput.HostName;
            connection.MacAddress = clientInput.MacAddress;
            string type = "Connected";
            var log = new ClientConnectionLog(
                connection.ClientConnectionId, 
                connection.HubConnectionId, 
                connection.IpAddress!, 
                connection.HostName!, 
                connection.MacAddress!, type);
            dbContext.ClientConnectionsLog.Add(log);
            await dbContext.SaveChangesAsync();
            await privateHub.Clients.Group("Admins").SendAsync(type, log);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (dbContext.ClientConnections.FirstOrDefault(
            x => x.HubConnectionId == Context.ConnectionId)
            is ClientConnection connection)
        {
            string type = "Disconnected";
            var log = new ClientConnectionLog(
                connection.ClientConnectionId, 
                connection.HubConnectionId, 
                connection.IpAddress!, 
                connection.HostName!, 
                connection.MacAddress!, type);
            dbContext.ClientConnectionsLog.Add(log);
            dbContext.ClientConnections.Remove(connection);
            dbContext.SaveChangesAsync();
            privateHub.Clients.Group("Admins").SendAsync(type, log);
        }

        return base.OnDisconnectedAsync(exception);
    }
}