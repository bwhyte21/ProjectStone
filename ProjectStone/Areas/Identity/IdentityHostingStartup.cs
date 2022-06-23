using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ProjectStone.Areas.Identity.IdentityHostingStartup))]

namespace ProjectStone.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => { });
    }
}