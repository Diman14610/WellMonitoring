using MediatR;
using WellServiceAPI.Domain.Commands;
using WellServiceAPI.Domain.Queries;
using WellServiceAPI.Models;

namespace WellServiceAPI.Services
{
    public class WellActivityBackgroundService : BackgroundService
    {
        private const int DAYS_AGO = -5;
        private const int ACTIVE = 1;
        private const int NOT_ACTIVE = 0;

        private readonly IServiceProvider _services;

        private readonly TimeSpan delay = TimeSpan.FromMinutes(1);

        public WellActivityBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckWellActivityAsync(stoppingToken);
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task CheckWellActivityAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _services.CreateScope();

            IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            DateTime daysAgo = DateTime.UtcNow.AddDays(DAYS_AGO);

            var wells = await mediator.Send(new GetAllWellsQuery(), cancellationToken).ConfigureAwait(false);

            foreach (Well well in wells)
            {
                if (well == null) continue;

                if (well.Telemetries.All(t => t.DateTime < daysAgo))
                {
                    await mediator.Send(new ChangeActivityWellCommand(well.Id, NOT_ACTIVE), cancellationToken).ConfigureAwait(false);
                    await Console.Out.WriteLineAsync($"Деактивация скважины {well.Name} (id: {well.Id}).");
                }
                else if (well.Active != ACTIVE)
                {
                    await mediator.Send(new ChangeActivityWellCommand(well.Id, ACTIVE), cancellationToken).ConfigureAwait(false);
                    await Console.Out.WriteLineAsync($"Активация скважины {well.Name} (id: {well.Id}).");
                }
            }
        }
    }
}
