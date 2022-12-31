using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.Tasks;

namespace CashTrack.Common.Middleware
{
    public class IpAddressMiddleware : IMiddleware
    {
        private IOptions<AppSettingsOptions> _appSettings;
        private readonly ILogger<IpAddressMiddleware> _logger;
        public IpAddressMiddleware(ILogger<IpAddressMiddleware> logger, IOptions<AppSettingsOptions> appSettings) => (_logger, _appSettings) = (logger, appSettings);
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var validIp = _appSettings.Value.IpAddress;
            var incomingIp = context.Connection.RemoteIpAddress.ToString();
            if (incomingIp != validIp)
                _logger.LogWarning($"Incoming request for {context.Request.Path} from {incomingIp}");
            else
                _logger.LogInformation($"Request from {incomingIp} for {context.Request.Path}");

            await next(context);
        }
    }
}
