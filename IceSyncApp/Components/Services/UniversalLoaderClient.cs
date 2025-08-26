using IceSyncApp.Components.Interfaces;
using IceSyncApp.Components.Models;
using IceSyncApp.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IceSyncApp.Components.Services
{
    public class UniversalLoaderClient : IUniversalLoaderClient
    {
        private readonly HttpClient _httpClient;
        private readonly UniversalLoaderOptions _options;
        private readonly ILogger<UniversalLoaderClient> _logger;

        private string? _token;
        private DateTime _tokenExpiry;

        public UniversalLoaderClient(HttpClient httpClient, IOptions<UniversalLoaderOptions> options, ILogger<UniversalLoaderClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token) && DateTime.UtcNow < _tokenExpiry)
                return _token;

            var requestBody = new
            {
                apiCompanyId = _options.CompanyId,
                apiUserId = _options.UserId,
                apiUserSecret = _options.UserSecret
            };

            var response = await _httpClient.PostAsync(
                $"{_options.BaseUrl}/v2/authenticate",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to authenticate with Universal Loader. Status: {StatusCode}", response.StatusCode);
                throw new Exception("Authentication failed.");
            }

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            _token = doc.RootElement.GetProperty("access_token").GetString();
            int expiresInSeconds = doc.RootElement.GetProperty("expires_in").GetInt32();
            _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresInSeconds - 60); // subtract 1 min safety margin

            return _token;
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<Workflow>> GetWorkflowsAsync()
        {
            await AddAuthHeaderAsync();

            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/workflows");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch workflows. Status: {StatusCode}", response.StatusCode);
                throw new Exception("Failed to fetch workflows.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var workflows = JsonSerializer.Deserialize<List<Workflow>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return workflows ?? new List<Workflow>();
        }

        public async Task<bool> RunWorkflowAsync(string workflowId)
        {
            await AddAuthHeaderAsync();

            var response = await _httpClient.PostAsync($"{_options.BaseUrl}/workflows/{workflowId}/run", null);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            _logger.LogWarning("Failed to run workflow {WorkflowId}. Status: {StatusCode}", workflowId, response.StatusCode);
            return false;
        }
    }
}
