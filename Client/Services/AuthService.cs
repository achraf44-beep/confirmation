using System.Net.Http.Json;

namespace Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string? _accessToken;
    private AuthenticatedUser? _currentUser;

    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        
        var apiBaseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);
    public AuthenticatedUser? CurrentUser => _currentUser;
    public string? AccessToken => _accessToken;

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { email, password });
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    _accessToken = result.AccessToken;
                    _currentUser = result.User;
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                    return new AuthResult { Success = true };
                }
            }
            
            return new AuthResult { Success = false, Error = "Invalid email or password" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Error = ex.Message };
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string role = "Agent")
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { email, password, role });
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    _accessToken = result.AccessToken;
                    _currentUser = result.User;
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                    return new AuthResult { Success = true };
                }
            }
            
            var error = await response.Content.ReadAsStringAsync();
            return new AuthResult { Success = false, Error = error };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, Error = ex.Message };
        }
    }

    public void Logout()
    {
        _accessToken = null;
        _currentUser = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public AuthenticatedUser User { get; set; } = null!;
}

public class AuthenticatedUser
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
