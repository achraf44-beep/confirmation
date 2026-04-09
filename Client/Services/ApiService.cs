using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Client.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ApiService(HttpClient httpClient, AuthService authService, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _authService = authService;
        
        var apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    private void SetAuthHeader()
    {
        if (!string.IsNullOrEmpty(_authService.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _authService.AccessToken);
        }
    }

    // Orders
    public async Task<List<OrderListItem>> GetOrdersAsync()
    {
        SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<OrderListItem>>("api/orders") ?? [];
    }

    public async Task<OrderDetails?> GetOrderAsync(Guid id)
    {
        SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<OrderDetails>($"api/orders/{id}");
    }

    public async Task<Guid?> CreateOrderAsync(CreateOrderRequest request)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/orders", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<IdResponse>();
            return result?.Id;
        }
        return null;
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid id, string status, string? note = null)
    {
        SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}/status", new { status, note });
        return response.IsSuccessStatusCode;
    }

    // Customers
    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/customers") ?? [];
    }

    public async Task<Guid?> CreateCustomerAsync(CreateCustomerRequest request)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/customers", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<IdResponse>();
            return result?.Id;
        }
        return null;
    }

    // Products
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/products") ?? [];
    }

    public async Task<Guid?> CreateProductAsync(CreateProductRequest request)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/products", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<IdResponse>();
            return result?.Id;
        }
        return null;
    }

    // Delivery Companies
    public async Task<List<DeliveryCompanyDto>> GetDeliveryCompaniesAsync()
    {
        SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<DeliveryCompanyDto>>("api/deliverycompanies") ?? [];
    }

    // Locations
    public async Task<List<WilayaDto>> GetWilayasAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<WilayaDto>>("api/locations/wilayas") ?? [];
    }

    // Dashboard
    public async Task<DashboardStats?> GetDashboardStatsAsync()
    {
        SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<DashboardStats>("api/dashboard/stats");
    }
}

// DTOs
public record IdResponse(Guid Id);

public record OrderListItem(
    Guid Id,
    string OrderNumber,
    string Source,
    string CustomerName,
    string Wilaya,
    string PhoneNumber,
    string Status,
    decimal Subtotal,
    DateTime CreatedAtUtc);

public record OrderDetails(
    Guid Id,
    string OrderNumber,
    string Source,
    string CustomerName,
    string PhoneNumber,
    string Wilaya,
    string Commune,
    string DeliveryAddress,
    string Status,
    DateTime CreatedAtUtc,
    string? Note,
    List<OrderItemDto> Items,
    decimal Subtotal);

public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);

public record CreateOrderRequest(
    string OrderNumber,
    string Source,
    string CustomerName,
    string PhoneNumber,
    string Wilaya,
    string Commune,
    string DeliveryAddress,
    List<CreateOrderItemRequest> Items,
    string? Note = null);

public record CreateOrderItemRequest(string ProductName, int Quantity, decimal UnitPrice);

public record CustomerDto(
    Guid Id,
    string Name,
    string PhoneNumber,
    string? Email,
    string Wilaya,
    string Commune,
    string Address,
    int TotalOrders,
    decimal TotalSpent,
    DateTime CreatedAtUtc);

public record CreateCustomerRequest(
    string Name,
    string PhoneNumber,
    string Wilaya,
    string Commune,
    string Address,
    string? Email = null);

public record ProductDto(
    Guid Id,
    string Name,
    string? Sku,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? Category,
    bool IsActive,
    DateTime CreatedAtUtc);

public record CreateProductRequest(
    string Name,
    decimal Price,
    int StockQuantity = 0,
    string? Sku = null,
    string? Description = null,
    string? Category = null);

public record DeliveryCompanyDto(Guid Id, string Name, string Code, bool IsActive, DateTime CreatedAtUtc);

public record WilayaDto(int Code, string Name, string NameAr);

public record DashboardStats(
    int TotalOrders,
    int PendingOrders,
    int ConfirmedOrders,
    int ShippedOrders,
    int DeliveredOrders,
    int ReturnedOrders,
    int CancelledOrders,
    decimal TotalRevenue,
    decimal ConfirmationRate,
    decimal DeliveryRate,
    int TotalCustomers,
    int TotalProducts,
    List<OrdersBySource> OrdersBySource,
    List<OrdersByWilaya> TopWilayas);

public record OrdersBySource(string Source, int Count);
public record OrdersByWilaya(string Wilaya, int Count);
