public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    public DbSet<ClientConnection> ClientConnections => Set<ClientConnection>();
    public DbSet<ClientConnectionLog> ClientConnectionsLog => Set<ClientConnectionLog>();

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountConnection> AccountConnections => Set<AccountConnection>();
    public DbSet<AccountConnectionLog> AccountConnectionsLog => Set<AccountConnectionLog>();
}