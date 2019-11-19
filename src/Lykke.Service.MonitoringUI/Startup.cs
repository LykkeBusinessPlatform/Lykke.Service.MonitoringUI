using System;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.Sdk.Middleware;
using Lykke.Service.MonitoringUI.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.MonitoringUI
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "MonitoringUI API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "MonitoringUILog";
                    logs.AzureTableConnectionStringResolver = settings => settings.MonitoringUIService.Db.LogsConnString;

                    options.Extend = (collection, manager) => { collection.AddHttpClient(); };
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            try
            {
                app.UseMiddleware<UnhandledExceptionLoggingMiddleware>();

                app.UseStaticFiles();
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=MonitoringUi}/{action=Index}");
                });
            }
            catch (Exception ex)
            {
                try
                {
                    var log = app.ApplicationServices.GetService<ILogFactory>().CreateLog(typeof(LykkeApplicationBuilderExtensions).FullName);

                    log.Critical(ex);
                }
                catch (Exception ex1)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(ex1);
                }

                throw;
            }
        }
    }
}
