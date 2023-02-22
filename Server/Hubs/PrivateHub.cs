

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PrivateHub : Hub
{
    DatabaseContext dbContext;

    public PrivateHub(IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        dbContext = dbContextFactory.CreateDbContext();
    }

    public override Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        var connection = new AccountConnection(connectionId);
        dbContext.AccountConnections.Add(connection);
        dbContext.SaveChangesAsync();
        return base.OnConnectedAsync();
    }

    [HubMethodName("AccountConnectionInfo")]
    public async Task UserConnectionInfo(Guid clientConnectionId, string tokenValue, IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        if (await dbContext.AccountConnections.FirstOrDefaultAsync(x => x.HubConnectionId == Context.ConnectionId) is AccountConnection connection)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenValue);
            string username = token.Claims.First(claim => claim.Type == "Username").Value;
            if (await dbContext.Accounts.FirstOrDefaultAsync(x => x.Username == username) is Account account)
            {
                connection.ClientConnectionId = clientConnectionId;
                connection.AccountId = account.Id;
                string type = "Login";
                var log = new AccountConnectionLog(connection.ClientConnectionId, connection.HubConnectionId, type, connection.AccountId);
                dbContext.AccountConnectionsLog.Add(log);
                await dbContext.SaveChangesAsync();
                await Clients.Group("Admins").SendAsync(type, log);
            }
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (dbContext.AccountConnections.FirstOrDefault(x => x.HubConnectionId == Context.ConnectionId) is AccountConnection connection)
        {
            string type = "Logout";
            var log = new AccountConnectionLog(connection.ClientConnectionId, connection.HubConnectionId, type, connection.AccountId);
            dbContext.AccountConnectionsLog.Add(log);
            dbContext.AccountConnections.Remove(connection);
            dbContext.SaveChangesAsync();
            Clients.Group("Admins").SendAsync(type, log);
        }

        return base.OnDisconnectedAsync(exception);
    }
}