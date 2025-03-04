using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace OuiAI.Microservices.Gateway.Services
{
    public class ApiGatewayService : IApiGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiGatewayService> _logger;

        public ApiGatewayService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<ApiGatewayService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> ForwardRequest(string serviceName, string path, HttpMethod method, object requestBody = null)
        {
            var serviceUrl = GetServiceUrl(serviceName);
            
            if (string.IsNullOrEmpty(serviceUrl))
            {
                return new NotFoundObjectResult($"Service {serviceName} not found");
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"{serviceUrl}/{path.TrimStart('/')}"),
            };

            if (requestBody != null && (method == HttpMethod.Post || method == HttpMethod.Put))
            {
                var json = JsonSerializer.Serialize(requestBody);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            try
            {
                var response = await client.SendAsync(request);
                
                var contentString = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(contentString))
                    {
                        return new OkResult();
                    }
                    
                    try
                    {
                        var content = JsonSerializer.Deserialize<object>(contentString);
                        return new OkObjectResult(content);
                    }
                    catch
                    {
                        return new OkObjectResult(contentString);
                    }
                }
                
                return new ObjectResult(contentString)
                {
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error forwarding request to {serviceName}");
                return new StatusCodeResult(StatusCodes.Status502BadGateway);
            }
        }

        public async Task<T> GetFromService<T>(string serviceName, string path) where T : class
        {
            var serviceUrl = GetServiceUrl(serviceName);
            
            if (string.IsNullOrEmpty(serviceUrl))
            {
                return null;
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{serviceUrl}/{path.TrimStart('/')}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            return null;
        }

        public async Task<IActionResult> PostToService(string serviceName, string path, object data)
        {
            return await ForwardRequest(serviceName, path, HttpMethod.Post, data);
        }

        public async Task<IActionResult> PutToService(string serviceName, string path, object data)
        {
            return await ForwardRequest(serviceName, path, HttpMethod.Put, data);
        }

        public async Task<IActionResult> DeleteFromService(string serviceName, string path)
        {
            return await ForwardRequest(serviceName, path, HttpMethod.Delete);
        }

        private string GetServiceUrl(string serviceName)
        {
            var serviceUrlKey = $"Services:{serviceName}:Url";
            return _configuration[serviceUrlKey];
        }
    }
}
