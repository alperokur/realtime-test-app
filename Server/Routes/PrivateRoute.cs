public static class PrivateRoute
{
    internal static RouteGroupBuilder PrivateMapApiEndpoints(this RouteGroupBuilder groups)
    {
        groups.MapGet("/Accounts", GetAccounts)
            .Produces(200, typeof(List<Account>))
            .ProducesProblem(401)
            .Produces(429);
        groups.MapGet("/Accounts/id={id}", GetAccountById)
            .Produces(200, typeof(PrivateRoute))
            .ProducesProblem(401)
            .Produces(429);
        groups.MapGet("/Accounts/username={username}", GetAccountByUsername)
            .Produces(200, typeof(PrivateRoute))
            .ProducesProblem(401)
            .Produces(429);
        groups.MapPut("/Accounts/id={id}", UpdateAccountById)
            .Accepts<UpdateInput>("application/json")
            .Produces(201)
            .ProducesProblem(404)
            .ProducesProblem(401)
            .Produces(429);
        groups.MapPut("/Accounts/username={username}", GetAccountByUsername)
            .Accepts<UpdateInput>("application/json")
            .Produces(201)
            .ProducesProblem(404)
            .ProducesProblem(401)
            .Produces(429);
        groups.MapDelete("/Accounts/id={id}", DeleteAccountById)
            .Produces(204).ProducesProblem(404)
            .ProducesProblem(401)
            .Produces(429);
        groups.MapDelete("/Accounts/username={username}", DeleteAccountByAccountname)
            .Produces(204)
            .ProducesProblem(404)
            .ProducesProblem(401)
            .Produces(429);
        return groups;
    }

    internal static async Task<IResult> GetAccounts(
        IDbContextFactory<DatabaseContext> dbContextFactory)
    {
        using var dbContext = 
            dbContextFactory.CreateDbContext();

        return TypedResults.Ok(
            await dbContext.Accounts.ToListAsync());
    }

    internal static async Task<IResult> GetAccountById(
        IDbContextFactory<DatabaseContext> dbContextFactory, 
        int id)
    {
        using var dbContext = 
            dbContextFactory.CreateDbContext();
        return await dbContext.Accounts.FirstOrDefaultAsync(
            t => t.Id == id) is 
            Account user ? TypedResults.Ok(user) : TypedResults.NotFound();
    }

    internal static async Task<IResult> GetAccountByUsername(IDbContextFactory<DatabaseContext> dbContextFactory, string username)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        return await dbContext.Accounts.FirstOrDefaultAsync(t => t.Username == username) is Account user ? TypedResults.Ok(user) : TypedResults.NotFound();
    }

    internal static async Task<IResult> UpdateAccountById(IDbContextFactory<DatabaseContext> dbContextFactory, int id, UpdateInput updateAccount)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        if (await dbContext.Accounts.FirstOrDefaultAsync(t => t.Id == id) is Account user)
        {
            user.Username = updateAccount.Username!;
            user.Password = Security.GetHashPasswordV3(updateAccount.Password!);
            user.FirstName = updateAccount.FirstName!;
            user.LastName = updateAccount.LastName!;
            user.Email = updateAccount.Email!;
            user.Phone = updateAccount.Phone!;
            user.UpdatedDate = DateTime.Now;
            await dbContext.SaveChangesAsync();
            return TypedResults.Ok(user);
        }

        return TypedResults.Problem("Account is not found!");
    }

    internal static async Task<IResult> UpdateAccountByUsername(
        IDbContextFactory<DatabaseContext> dbContextFactory, 
        string username, 
        UpdateInput updateAccount)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        if (await dbContext.Accounts.FirstOrDefaultAsync(
            t => t.Username == username) is Account user)
        {
            user.Username = updateAccount.Username!;
            user.Password = Security.GetHashPasswordV3(
                updateAccount.Password!);
            user.FirstName = updateAccount.FirstName!;
            user.LastName = updateAccount.LastName!;
            user.Email = updateAccount.Email!;
            user.Phone = updateAccount.Phone!;
            user.UpdatedDate = DateTime.Now;
            await dbContext.SaveChangesAsync();
            return TypedResults.Ok(user);
        }

        return TypedResults.Problem("Account is not found!");
    }

    internal static async Task<IResult> DeleteAccountById(
        IDbContextFactory<DatabaseContext> dbContextFactory, int id)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        if (await dbContext.Accounts.FirstOrDefaultAsync(
            t => t.Id == id) is Account user)
        {
            dbContext.Accounts.Remove(user);
            await dbContext.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.Problem("Account is not found!");
    }

    internal static async Task<IResult> DeleteAccountByAccountname(IDbContextFactory<DatabaseContext> dbContextFactory, string username)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        if (await dbContext.Accounts.FirstOrDefaultAsync(t => t.Username == username) is Account user)
        {
            dbContext.Accounts.Remove(user);
            await dbContext.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.Problem("Account is not found!");
    }
}