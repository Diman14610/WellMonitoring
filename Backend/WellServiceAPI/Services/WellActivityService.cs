using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Command;

namespace WellServiceAPI.Services
{
    public class WellActivityService : BackgroundService
    {
        private const int DAYS_AGO = -5;
        private const int ACTIVE = 1;
        private const int NOT_ACTIVE = 0;

        private readonly TimeSpan delay = TimeSpan.FromMinutes(1);
        private readonly IServiceProvider _services;

        public WellActivityService(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckWellActivityAsync();
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task CheckWellActivityAsync()
        {
            using IServiceScope scope = _services.CreateScope();

            IQueryService<IEnumerable<Well>> getAllWells = scope.ServiceProvider.GetRequiredService<IQueryService<IEnumerable<Well>>>();
            ICommandService<ChangeActiveWell> changeActiveWell = scope.ServiceProvider.GetRequiredService<ICommandService<ChangeActiveWell>>();

            DateTime daysAgo = DateTime.UtcNow.AddDays(DAYS_AGO);

            foreach (Well well in await getAllWells.ExecuteAsync())
            {
                if (well == null) continue;

                if (well.Telemetries.All(t => t.DateTime < daysAgo))
                {
                    await changeActiveWell.ExecuteAsync(new ChangeActiveWell(well.Id, NOT_ACTIVE));
                    await Console.Out.WriteLineAsync($"Деактивация скважины {well.Name} (id: {well.Id}).");
                }
                else if (well.Active != ACTIVE)
                {
                    await changeActiveWell.ExecuteAsync(new ChangeActiveWell(well.Id, ACTIVE));
                    await Console.Out.WriteLineAsync($"Активация скважины {well.Name} (id: {well.Id}).");
                }
            }
        }
    }
}
