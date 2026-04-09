using Application.Customers;
using Application.Customers.Abstractions;
using Application.Dashboard;
using Application.Delivery;
using Application.Delivery.Abstractions;
using Application.Identity.Abstractions;
using Application.Orders.Abstractions;
using Application.Products;
using Application.Products.Abstractions;
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
        
        // Repositories
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<ICustomerRepository, EfCustomerRepository>();
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<IDeliveryCompanyRepository, EfDeliveryCompanyRepository>();
        
        // Services
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<ICustomerAppService, CustomerAppService>();
        services.AddScoped<IProductAppService, ProductAppService>();
        services.AddScoped<IDeliveryAppService, DeliveryAppService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}