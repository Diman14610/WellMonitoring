using MediatR;
using Microsoft.AspNetCore.Mvc;
using WellServiceAPI.Domain.Commands;
using WellServiceAPI.Domain.Queries;
using WellServiceAPI.Shared.Request;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : Controller
    {
        private readonly IMediator _mediator;

        public TelemetryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TelemetryInfo>>> GetAllTelemetryAsync(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<TelemetryInfo> telemetry = await _mediator.Send(new GetAllTelemetryQuery(), cancellationToken).ConfigureAwait(false);
                return Ok(telemetry);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при добавлении телеметрии.");
            }
        }

        [HttpPost("all")]
        public async Task<ActionResult> ReceiveTelemetryDataAsync(IEnumerable<TelemetryData> telemetries, CancellationToken cancellationToken)
        {
            if (!telemetries.Any())
            {
                return BadRequest();
            }

            try
            {
                await _mediator.Send(new SaveTelemetryDataCommand(telemetries), cancellationToken).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при добавлении телеметрии.");
            }
        }
    }
}
