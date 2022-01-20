using GrpcService1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GrpcStopAsyncTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// web应用构建器
        /// </summary>
        private volatile WebApplicationBuilder _webAppBuilder;

        /// <summary>
        /// web服务器
        /// </summary>
        private volatile WebApplication _webApp;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                WebApplicationBuilder builder = WebApplication.CreateBuilder();
                this._webAppBuilder = builder;


                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 22222, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                });


                // Add services to the container.
                builder.Services.AddGrpc();


                WebApplication app = builder.Build();
                this._webApp = app;
                app.MapGrpcService<GreeterService>();

                app.StartAsync().Wait();
            }
            catch
            {
                //ingored
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if(this._webApp != null)
                {
                    //this method never return
                    this._webApp.StopAsync().Wait();
                }
            }
            catch
            {
                //ignored
            }
        }
    }
}
