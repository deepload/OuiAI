using Microsoft.AspNetCore.Mvc;

namespace OuiAI.Microservices.Gateway.Services
{
    public interface IApiGatewayService
    {
        Task<IActionResult> ForwardRequest(string serviceName, string path, HttpMethod method, object requestBody = null);
        Task<T> GetFromService<T>(string serviceName, string path) where T : class;
        Task<IActionResult> PostToService(string serviceName, string path, object data);
        Task<IActionResult> PutToService(string serviceName, string path, object data);
        Task<IActionResult> DeleteFromService(string serviceName, string path);
    }
}
