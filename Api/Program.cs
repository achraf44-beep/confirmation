using System.Text;
using Api.Authentication;
using Api.Authorization;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\MSSQLLocalDB;Database=WasselDb;Trusted_Connection=True;TrustServerCertificate=True";

builder.Services.AddInfrastructure(connectionString);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddSingleton<JwtTokenFactory>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ClockSkew = TimeSpan.FromMinutes(1)
                };
        });

builder.Services.AddAuthorization(options =>
{
        options.AddPolicy(AuthPolicyNames.OrdersRead, policy =>
        {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                        HasAnyRole(context.User, "Owner", "Agent", "DeliveryCoordinator") ||
                        context.User.HasClaim("permissions", "orders.read"));
        });

        options.AddPolicy(AuthPolicyNames.OrdersWrite, policy =>
        {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                        HasAnyRole(context.User, "Owner", "Agent") ||
                        context.User.HasClaim("permissions", "orders.write"));
        });
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://localhost:5001",
            "http://localhost:5000",
            "https://localhost:7001",
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await AppDbContextSeeder.SeedAsync(dbContext);
}

app.MapGet("/", () => Results.Ok(new
{
        service = "Wassel API",
        status = "running",
        version = "1.0.0",
        description = "E-commerce Order Management Platform for Algeria"
}));

app.Run();

static bool HasAnyRole(ClaimsPrincipal principal, params string[] roles)
{
        return roles.Any(principal.IsInRole);
}
