using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectStone.Extensions;
using ProjectStone_DataAccess.Initializer;

namespace ProjectStone;

public class Startup
{
    // Make the IConfiguration instance private instead of the provided public one.
    private readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
        _config = config;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        #region Controller Service

        services.AddControllersWithViews();

        #endregion

        #region Other Services

        services.AddApplicationServices(_config);

        #endregion
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
    {
        // Configure HTTP Request pipeline.
        app.AddApplicationMiddleware(env, _config, dbInitializer);
    }
}