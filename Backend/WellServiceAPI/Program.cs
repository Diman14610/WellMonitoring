using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using WellServiceAPI.Data;
using WellServiceAPI.Services;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Services.Abstractions.SignalR;
using WellServiceAPI.Services.Implementations.DB.SqlLite.Command;
using WellServiceAPI.Services.Implementations.SignalR;
using WellServiceAPI.Shared.Actions.Command;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllers()
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            builder.Services.AddSignalR();

            builder.Services.AddDbContext<WellDBContext>(option =>
            {
                option.UseSqlite("Data Source=WellDatabase.db");
            });

            builder.Services.AddHostedService<WellActivityService>();

            builder.Services.AddDecorator<ICommandService<SaveTelemetryData>, SqlLiteCommandServiceSaveTelemetryAndNotifyHub>(decorateeServices =>
            {
                decorateeServices.AddScoped<ICommandService<SaveTelemetryData>, SqlLiteCommandServiceSaveTelemetry>();
            });

            builder.Services.AddSingleton<IMessagesHub, MessagesHubService>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsAbstract) continue;

                    var interfaces = type.GetInterfaces();

                    foreach (var interfaceType in interfaces)
                    {
                        var genericArguments = interfaceType.GetGenericArguments();

                        if (genericArguments.Length == 0) continue;
                        if (type.FullName != null && type.FullName.Contains("SqlLiteCommandServiceSaveTelemetry")) continue;

                        if (interfaceType?.GetGenericTypeDefinition() == typeof(ICommandService<>))
                        {
                            var serviceType = typeof(ICommandService<>).MakeGenericType(genericArguments[0]);
                            builder.Services.AddTransient(serviceType, type);
                        }
                        else if (interfaceType?.GetGenericTypeDefinition() == typeof(IQueryService<>))
                        {
                            var serviceType = typeof(IQueryService<>).MakeGenericType(genericArguments[0]);
                            builder.Services.AddTransient(serviceType, type);
                        }
                        else if (interfaceType?.GetGenericTypeDefinition() == typeof(IQueryService<,>))
                        {
                            var serviceType = typeof(IQueryService<,>).MakeGenericType(genericArguments[0], genericArguments[1]);
                            builder.Services.AddTransient(serviceType, type);
                        }
                    }
                }
            }

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(con =>
            {
                con
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials(); // allow credentials
            });

            app.UseHttpsRedirection();

            app.MapHub<MessagesHub>("/messages");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}