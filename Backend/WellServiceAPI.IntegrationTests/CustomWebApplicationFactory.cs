using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WellServiceAPI.Data;

namespace WellServiceAPI.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var dbContext = services
                    .SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<WellDBContext>));

                services.Remove(dbContext!);

                services.AddDbContext<WellDBContext>(option =>
                {
                    option.UseInMemoryDatabase("WellDB");
                });
            });

            base.ConfigureWebHost(builder);
        }
    }
}
