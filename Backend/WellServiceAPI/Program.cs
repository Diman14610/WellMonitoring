using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;
using WellServiceAPI.Data;
using WellServiceAPI.Services;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Services.Abstractions.SignalR;
using WellServiceAPI.Services.Implementations.DB.Command;
using WellServiceAPI.Services.Implementations.SignalR;
using WellServiceAPI.Shared.Actions.Command;

namespace WellServiceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            builder.Services.AddSignalR();

            builder.Services.AddDbContext<WellDBContext>(option =>
            {
                option.UseSqlite(builder.Configuration.GetConnectionString("WellsDatabase"));
            });

            builder.Services.AddHostedService<WellActivityService>();

            builder.Services.AddDecorator<ICommandService<SaveTelemetryData>, SaveTelemetryAndNotifyHubCommandService>(decorateeServices =>
            {
                decorateeServices.AddScoped<ICommandService<SaveTelemetryData>, SaveTelemetryCommandService>();
            });

            builder.Services.AddSingleton<IMessagesHub, MessagesHubService>();

            InitializationCommandAndQueryRervices(builder.Services);

            var app = builder.Build();

#if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(con =>
            {
                con
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials();
            });
            app.UseCors("CorsPolicy");
#endif

            app.UseHttpsRedirection();

            app.MapHub<MessagesHub>("/messages");

            app.UseAuthorization();

            app.MapControllers();

            app.UseAuthentication();

            app.Run();
        }

        private static void InitializationCommandAndQueryRervices(IServiceCollection services)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.IsAbstract) continue;

                var interfaces = type.GetInterfaces();

                foreach (var interfaceType in interfaces)
                {
                    var genericArguments = interfaceType.GetGenericArguments();

                    if (genericArguments.Length == 0) continue;

                    Type? serviceType = null;

                    if (interfaceType?.GetGenericTypeDefinition() == typeof(ICommandService<>))
                    {
                        serviceType = typeof(ICommandService<>).MakeGenericType(genericArguments[0]);
                    }
                    else if (interfaceType?.GetGenericTypeDefinition() == typeof(IQueryService<>))
                    {
                        serviceType = typeof(IQueryService<>).MakeGenericType(genericArguments[0]);
                    }
                    else if (interfaceType?.GetGenericTypeDefinition() == typeof(IQueryService<,>))
                    {
                        serviceType = typeof(IQueryService<,>).MakeGenericType(genericArguments[0], genericArguments[1]);
                    }

                    if (serviceType != null && !services.Any(x => x.ServiceType == serviceType))
                    {
                        services.AddTransient(serviceType, type);
                    }
                }
            }
        }
    }
}