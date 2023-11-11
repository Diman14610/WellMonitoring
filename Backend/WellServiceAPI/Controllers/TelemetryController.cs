﻿using Microsoft.AspNetCore.Mvc;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Command;
using WellServiceAPI.Shared.Request;
using WellServiceAPI.Shared.Response.Telemetry;

namespace WellServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/telemetry")]
    public class TelemetryController : Controller
    {
        private readonly ICommandService<SaveTelemetryData> _saveTelemetryData;
        private readonly IQueryService<IEnumerable<TelemetryInfo>> _getAllTelemetry;

        public TelemetryController(
            ICommandService<SaveTelemetryData> saveTelemetryData,
            IQueryService<IEnumerable<TelemetryInfo>> getAllTelemetry)
        {
            _saveTelemetryData = saveTelemetryData ?? throw new ArgumentNullException(nameof(saveTelemetryData));
            _getAllTelemetry = getAllTelemetry;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TelemetryInfo>>> GetAllTelemetryAsync()
        {
            try
            {
                IEnumerable<TelemetryInfo> telemetry = await _getAllTelemetry.ExecuteAsync().ConfigureAwait(false);
                return Ok(telemetry);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при добавлении телеметрии.");
            }
        }

        [HttpPost("all")]
        public async Task<ActionResult> ReceiveTelemetryDataAsync(IEnumerable<TelemetryData> telemetries)
        {
            if (!telemetries.Any())
            {
                return BadRequest();
            }

            try
            {
                await _saveTelemetryData.ExecuteAsync(new SaveTelemetryData(telemetries)).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при добавлении телеметрии.");
            }
        }
    }
}
