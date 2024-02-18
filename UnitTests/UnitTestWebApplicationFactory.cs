using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UnitTestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly IPolicyEvaluator _policyEvaluator;

        public UnitTestWebApplicationFactory(IPolicyEvaluator policyEvaluator)
        {
            _policyEvaluator = policyEvaluator;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole().AddDebug());
                services.AddSingleton<IPolicyEvaluator>(_policyEvaluator);
            });           
        }
    }
}
