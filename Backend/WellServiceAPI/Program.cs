using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WellServiceAPI.Data;
using WellServiceAPI.Domain.Commands;
using WellServiceAPI.Services;
using WellServiceAPI.Services.Abstractions;
using WellServiceAPI.Services.Implementations;

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
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
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

            builder.Services.AddHostedService<WellActivityBackgroundService>();

            builder.Services.AddSingleton<IMessagesHub, MessagesHubService>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.AddRequestPreProcessor<SaveTelemetryAndNotifyHubCommand>();
            });

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
    }
}