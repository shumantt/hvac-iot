using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Scrutor;
using ServiceLayerApi.DataProcessing;
using ServiceLayerApi.DeviceNetwork;
using ServiceLayerApi.DeviceNetwork.Description;
using ServiceLayerApi.DeviceNetwork.Sensors;
using ServiceLayerApi.MQTT.Server;

namespace ServiceLayerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging(s => s.AddNLog());
            services.Scan(scan => scan     
                .FromCallingAssembly() // 1. Find the concrete classes
                .AddClasses()                        //    to register
                .UsingRegistrationStrategy(RegistrationStrategy.Append) // 2. Define how to handle duplicates
                .AsSelf()// 2. Specify which services they are registered as
                .WithSingletonLifetime()); // 3. Set the lifetime for the services

            //TODO разобраться со сркутором
            services.AddSingleton<IDeviceBuilder, CustomTemperatureSensorBuilder>();
            services.AddSingleton<IParameterAggregator, MeanTemperatureAggregator>();
            
            services.AddHostedService<MqttServer>();
            services.AddHostedService<DeviceInfoProcessingService>();
            services.AddHostedService<SensorProcessingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}