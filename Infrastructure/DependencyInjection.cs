using Application.Identity.Abstractions;
using Application.Orders.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();

        return services;
    }
}