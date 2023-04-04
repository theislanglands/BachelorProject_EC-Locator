using EC_locator.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EC_locator;

namespace EC_locator.Core;

public class ServiceDirectory
{
    private static IHost _host;
    private ServiceCollection services;
    
    public ServiceDirectory()
    {
        services = new ServiceCollection();
        
        /*
        
        _host = Host.CreateDefaultBuilder().ConfigureServices(
                services =>
                {
                    services.AddSingleton<ISettings, Settings>();
                })
            .Build();
            */
    }

    public IHost getHost()
    {
        return _host;
    }
}