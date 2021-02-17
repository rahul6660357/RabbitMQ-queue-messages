using EventBus.MessageQueues;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Consumequeue.Servicehealth;
using Consumequeue.Services;
using Consumequeue.Services.Implementation;
using QueueConsumer;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Consumequeue.ViewModels;

namespace Consumequeue
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

            services.AddHealthChecks()
                .AddCheck<MessageQueueHealth>("Message_Queue_health");
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
       .WithOrigins("http://localhost:4200")
       .AllowAnyMethod()
       .AllowAnyHeader()
       .AllowCredentials());
               /* options.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod().Build();
                });*/
            });
            services.AddSignalR();
            services.AddSingleton(serviceProvider =>
            {
                IConnection _confactory;
                string exchangename = Configuration.GetSection("MessageQuque").GetSection("NotificationQueue").GetSection("exchangene").Value;
                var factory = new ConnectionFactory();
                factory.UserName = Configuration.GetSection("MessageQuqueCredentials").GetSection("username").Value;
                factory.Password = Configuration.GetSection("MessageQuqueCredentials").GetSection("password").Value;
                factory.HostName = Configuration.GetSection("MessageQuqueCredentials").GetSection("HostName").Value;
                factory.VirtualHost = Configuration.GetSection("MessageQuqueCredentials").GetSection("VirtualHost").Value;

                try
                {
                    _confactory = factory.CreateConnection();

                    using (var channel = _confactory.CreateModel())
                    {
                        channel.ExchangeDeclare(exchangename, ExchangeType.Topic);
                    }

                }
                catch(Exception e)
                {
                    // Nototfy to notofication service
                    _confactory = null;
                }
                return _confactory;
            });

            services.AddHostedService<ConsumeNotificationQueue>();

            services.AddSingleton<IQueueProvider, Queueprovider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<INotification, Notification>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseStatusCodePages("text/plain", "status code: {0}");
            app.UseCors("CorsPolicy");

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = WriteResponse
                });
                endpoints.MapHub<DataViewModel>("/consume");
            });
        }

        private static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteStartObject("results");
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteString("description", entry.Value.Description);
                        writer.WriteStartObject("data");
                        foreach (var item in entry.Value.Data)
                        {
                            writer.WritePropertyName(item.Key);
                            JsonSerializer.Serialize(
                                writer, item.Value, item.Value?.GetType() ??
                                typeof(object));
                        }
                        writer.WriteEndObject();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());

                return context.Response.WriteAsync(json);
            }
        }
    }
}
