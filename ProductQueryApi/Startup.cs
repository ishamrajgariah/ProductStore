using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductQueryApi.Cache;
using ProductQueryApi.Cache.RedisProductCache;
using ProductQueryApi.Events;
using ProductQueryApi.Models;
using ProductQueryApi.Queues;
using ProductQueryApi.Queues.AMQP;
using ProductQueryApi.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProductQueryApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();

            services.AddSingleton<IProductRepository, ProductMemoryRepository>();

            services.Configure<QueueOptions>(Configuration.GetSection("QueueOptions"));
            services.Configure<AMQPOptions>(Configuration.GetSection("amqp"));
            services.AddTransient(typeof(IConnectionFactory), typeof(AMQPConnectionFactory));
            services.AddTransient(typeof(EventingBasicConsumer), typeof(AMQPEventingConsumer));
            services.AddSingleton(typeof(IEventSubscriber), typeof(AMQPEventSubscriber));
            services.AddSingleton(typeof(IEventProcessor), typeof(NewProductEventProcessor));
            //services.AddRedisConnectionMultiplexer(Configuration);
            services.AddMemoryCache();
            services.AddSingleton<IProductCache, MemoryProductCache>();

        }

        // Singletons are lazy instantiation.. so if we don't ask for an instance during startup,
        // they'll never get used.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IEventProcessor eventProcessor)
        {
            app.UseMvc();

            eventProcessor.Start();
        }
    }
}
