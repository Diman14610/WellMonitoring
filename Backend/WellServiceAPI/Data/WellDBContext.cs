using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Models;

namespace WellServiceAPI.Data
{
    public class WellDBContext : DbContext
    {
        public WellDBContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Well> Wells { get; set; }

        public DbSet<Company> Companys { get; set; }

        public DbSet<Telemetry> Telemetrys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Telemetry>(t =>
            {
                t.Property(p => p.DateTime).HasColumnType("datetime");
            });

            var companyId = 1;
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = companyId++, Name = "Black Gold" },
                new Company { Id = companyId++, Name = "Energy Max" },
                new Company { Id = companyId++, Name = "Fuel Tech" },
                new Company { Id = companyId++, Name = "Drill Well" },
                new Company { Id = companyId++, Name = "Ecofuel" },
                new Company { Id = companyId++, Name = "Power Jet" },
                new Company { Id = companyId++, Name = "Drill Pro" },
                new Company { Id = companyId++, Name = "Qucik Flow" },
                new Company { Id = companyId++, Name = "Oil Up" },
                new Company { Id = companyId++, Name = "Petro Max" }
                );

            var wellId = 1;
            modelBuilder.Entity<Well>().HasData(
                new Well { Id = wellId++, Active = 0, CompanyId = 1, Name = "Deep" },
                new Well { Id = wellId++, Active = 1, CompanyId = 1, Name = "Bryl" },
                new Well { Id = wellId++, Active = 1, CompanyId = 1, Name = "Kirovskaya borehole" },
                new Well { Id = wellId++, Active = 1, CompanyId = 4, Name = "Nord Stream" },
                new Well { Id = wellId++, Active = 1, CompanyId = 5, Name = "Baikal" },
                new Well { Id = wellId++, Active = 0, CompanyId = 4, Name = "South Stream" },
                new Well { Id = wellId++, Active = 1, CompanyId = 6, Name = "The Arctic" }
                );

            var random = new Random();
            var telemetries = new List<Telemetry>();
            for (int j = 1; j < 1001; j++)
            {
                telemetries.Add(new Telemetry
                {
                    Id = j,
                    WellId = random.Next(1, wellId),
                    DateTime = new DateTime(
                        2023,
                        random.Next(1, 12),
                        random.Next(1, 20),
                        random.Next(1, 24),
                        random.Next(1, 60),
                        random.Next(1, 60)
                        ),
                    Depth = random.Next(0, j * random.Next(1, 100)),
                });
            }
            modelBuilder.Entity<Telemetry>().HasData(telemetries);

            base.OnModelCreating(modelBuilder);
        }
    }
}
