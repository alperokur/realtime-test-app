using Server.Routes;

var builder = WebApplication.CreateBuilder(args);
var jwtPolicyName = "jwt";

builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    limiterOptions.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
            .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
            .LogWarning("OnRejected: {GetUserEndPoint}", GetUserEndPoint(context.HttpContext));

        return new ValueTask();
    };

    limiterOptions.AddPolicy(policyName: jwtPolicyName, partitioner: httpContext =>
    {
        var tokenValue = string.Empty;
        if (AuthenticationHeaderValue.TryParse(httpContext.Request.Headers["Authorization"], out var authHeader))
        {
            tokenValue = authHeader.Parameter;
        }

        var username = string.Empty;
        var rateLimitWindowInMinutes = 5;
        var permitLimitAuthorized = 60;
        var permitLimitAnonymous = 30;
        if (!string.IsNullOrEmpty(tokenValue))
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenValue);
            Debug.WriteLine(token.Claims.Count());
            if (token.Claims.Any(claim => claim.Type == "Username"))
            {
                username = token.Claims.First(claim => claim.Type == "Username").Value;
                var dbContext = httpContext.RequestServices.GetRequiredService<DatabaseContext>();
                if (Queryable.FirstOrDefault(dbContext.Accounts, x => x.Username == username) is Account user)
                {
                    permitLimitAuthorized = user.PermitLimit;
                    rateLimitWindowInMinutes = user.RateLimitWindowInMinutes;
                }
            }
        }

        return RateLimitPartition.GetFixedWindowLimiter(username, _ => new FixedWindowRateLimiterOptions()
        {
            PermitLimit = string.IsNullOrEmpty(username) ? permitLimitAnonymous : permitLimitAuthorized,
            Window = TimeSpan.FromMinutes(rateLimitWindowInMinutes),
            QueueLimit = 0
        });
    });
});

static string GetUserEndPoint(HttpContext context)
{
    var tokenValue = string.Empty;
    if (AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out var authHeader))
    {
        tokenValue = authHeader.Parameter;
    }
    var username = "";
    if (!string.IsNullOrEmpty(tokenValue))
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenValue);
        username = token.Claims.First(claim => claim.Type == "Username").Value;
    }

    return $"User {username ?? "Anonymous"} endpoint:{context.Request.Path}"
   + $" {context.Connection.RemoteIpAddress}";
}


builder.WebHost.UseKestrel(options => options.AddServerHeader = false);
builder.Services.AddHttpContextAccessor();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});

builder.Services.AddSignalR();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(cfg =>
{
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidAudience = builder.Configuration["Token:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Token:SigningKey"]!))
    };
});

builder.Services.AddDbContextFactory<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Server",
        Version = "v1",
    });

    setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. " +
        "Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    setup.OperationFilter<AuthorizationHeaderOperationHeader>();
    setup.OperationFilter<ApiVersionOperationFilter>();
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHealthChecks().AddDbContextCheck<DatabaseContext>();

builder.Services.AddScoped<IValidator<LoginInput>, LoginInputValidator>();
builder.Services.AddScoped<IValidator<RegisterInput>, RegisterInputValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(new ApiVersion(1, 0))
                    .HasApiVersion(new ApiVersion(2, 0))
                    .ReportApiVersions()
                    .Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1");
});

app.UseWebSockets();

var scope = app.Services.CreateScope();
var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>();
databaseContext?.Database.EnsureCreated();

app.MapGroup("/")
    .PublicMapApiEndpoints()
    .WithTags("Public")
    .RequireRateLimiting(jwtPolicyName)
    .WithOpenApi()
    .WithMetadata()
    .WithApiVersionSet(versionSet)
    .AddEndpointFilter(async (efiContext, next) =>
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await next(efiContext);
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;
        var response = efiContext.HttpContext.Response;
        response.Headers.Add("X-Response-Time", $"{elapsed} milliseconds");
        return result;
    });

app.MapGroup("/api")
    .PrivateMapApiEndpoints()
    .WithTags("Private")
    .RequireAuthorization()
    .RequireRateLimiting(jwtPolicyName)
    .WithOpenApi()
    .WithMetadata()
    .WithApiVersionSet(versionSet)
    .AddEndpointFilter(async (efiContext, next) =>
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await next(efiContext);
        stopwatch.Stop();
        var elapsed = stopwatch.ElapsedMilliseconds;
        var response = efiContext.HttpContext.Response;
        response.Headers.Add("X-Response-Time", $"{elapsed} milliseconds");
        return result;
    });

app.MapGet("/health", async (HealthCheckService healthCheckService) =>
{
    var report = await healthCheckService.CheckHealthAsync();
    return report.Status == HealthStatus.Healthy ?
        Results.Ok(report) : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
}).WithOpenApi()
.WithTags(new[] { "Health" })
.RequireRateLimiting(jwtPolicyName)
.Produces(200)
.ProducesProblem(503)
.Produces(429);

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PublicHub>("/connect/publichub");
app.MapHub<PrivateHub>("/connect/privatehub");

app.Run();