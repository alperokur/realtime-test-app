namespace Server.Routes
{
    public static class PublicRoute
    {
        internal static RouteGroupBuilder PublicMapApiEndpoints(this RouteGroupBuilder groups)
        {
            groups.MapPost("/login", Login).Accepts<LoginInput>("application/json").Produces(201).ProducesProblem(401).ProducesProblem(400).Produces(429);
            groups.MapPost("/register", Register).Accepts<RegisterInput>("application/json").Produces(201).ProducesProblem(401).ProducesProblem(400).Produces(429);
            return groups;
        }

        internal static async Task<IResult> Login(LoginInput loginInput, IDbContextFactory<DatabaseContext> dbContextFactory, IConfiguration configuration)
        {
            LoginInputValidator loginInputValidator = new LoginInputValidator();
            var validationResult = loginInputValidator.Validate(loginInput);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            using var dbContext = dbContextFactory.CreateDbContext();
            if (await dbContext.Accounts.FirstOrDefaultAsync(x => x.Username == loginInput.Username) is Account account)
            {
                if (!await dbContext.AccountConnections.AnyAsync(x => x.AccountId == account.Id))
                {
                    if (Security.VerifyHashedPasswordV3(Convert.FromBase64String(account.Password), loginInput.Password!))
                    {
                        Token token = Security.CreatePrivateToken(loginInput, configuration);
                        return TypedResults.Ok(token);
                    }
                    else
                    {
                        return TypedResults.Problem("Username or password do not match!");
                    }
                }
                else
                {
                    return TypedResults.Problem("User is already logged in!");
                }

            }
            else
            {
                return TypedResults.Problem("Username not founded!");
            }
        }

        internal static async Task<IResult> Register(
            RegisterInput registerInput, 
            IDbContextFactory<DatabaseContext> dbContextFactory, 
            IConfiguration configuration, 
            IHubContext<PublicHub> publicHub)
        {
            RegisterInputValidator registerInputValidator = new(dbContextFactory);
            var validationResult = registerInputValidator.Validate(registerInput);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            using var dbContext = dbContextFactory.CreateDbContext();
            dbContext.Accounts.Add(
                new Account(registerInput.Username!, 
                Security.GetHashPasswordV3(registerInput.Password!), 
                registerInput.FirstName!, registerInput.LastName!,
                registerInput.Email!, registerInput.Phone!));
            await dbContext.SaveChangesAsync();
            return TypedResults.Ok(registerInput);
        }
    }
}