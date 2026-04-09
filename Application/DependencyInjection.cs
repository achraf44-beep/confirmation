using Application.Auth;
using Application.Auth.Abstractions;
using Application.Orders;
using Application.Orders.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrderAppService, OrderAppService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}