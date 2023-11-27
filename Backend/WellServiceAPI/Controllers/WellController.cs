using MediatR;
using Microsoft.AspNetCore.Mvc;
using WellServiceAPI.Domain.Queries;
using WellServiceAPI.Models;
using WellServiceAPI.Shared.Response.Well;

namespace WellServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WellController : ControllerBase
    {
        private const int ACTIVE = 1;

        private readonly IMediator _mediator;

        public WellController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{wellId:int}")]
        public async Task<ActionResult<string>> GetWellByIdAsync(int wellId, CancellationToken cancellationToken)
        {
            try
            {
                var foundWell = await _mediator.Send(new GetWellByIdQuery(wellId), cancellationToken).ConfigureAwait(false);

                if (foundWell == null)
                {
                    return NotFound();
                }

                return Ok(foundWell.Name);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при извлечении скважины с идентификатором: {wellId}.");
            }
        }

        [HttpGet("company/{companyName}")]
        public async Task<ActionResult<IEnumerable<string>>> GetWellsByCompanyNameAsync(string companyName, CancellationToken cancellationToken)
        {
            try
            {
                var foundCompany = await _mediator.Send(new GetCompanyByNameQuery(companyName), cancellationToken).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<string> wellsNames =
                    (await _mediator.Send(new GetAllWellsQuery(), cancellationToken).ConfigureAwait(false))
                    .Where(w => IsEquals(w.Company.Name, companyName))
                    .Select(w => w.Name);

                return Ok(wellsNames);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при извлечении скважин по названию компании: {companyName}.");
            }
        }

        [HttpGet("active/all")]
        public async Task<ActionResult<IEnumerable<WellsWithContractors>>> GetActiveWellsWithContractorsAsync(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<WellsWithContractors> wellInfos =
                    (await _mediator.Send(new GetAllWellsByActivityParamQuery(ACTIVE), cancellationToken).ConfigureAwait(false))
                    .Select(w => new WellsWithContractors { WellName = w.Name, Contractor = w.Company.Name });

                return Ok(wellInfos);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, "Произошла ошибка при извлечении активных скважин подрядчиков.");
            }
        }

        [HttpGet("active/{wellId:int}")]
        public async Task<ActionResult<string>> GetActiveWellsByIdAsync(int wellId, CancellationToken cancellationToken)
        {
            try
            {
                var foundWell = await _mediator.Send(new GetWellByIdQuery(wellId), cancellationToken).ConfigureAwait(false);

                if (foundWell == null || !IsActive(foundWell))
                {
                    return NotFound();
                }

                return Ok(foundWell.Name);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при извлечении активной скважины с идентификатором: {wellId}.");
            }
        }

        [HttpGet("active/company/{companyName}")]
        public async Task<ActionResult<IEnumerable<string>>> GetActiveWellsByCompanyNameAsync(string companyName, CancellationToken cancellationToken)
        {
            try
            {
                var foundCompany = await _mediator.Send(new GetCompanyByNameQuery(companyName), cancellationToken).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<string> wellsNames =
                    (await _mediator.Send(new GetAllWellsByActivityParamQuery(ACTIVE), cancellationToken).ConfigureAwait(false))
                    .Where(w => IsEquals(w.Company.Name, companyName))
                    .Select(w => w.Name);

                return Ok(wellsNames);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при извлечении активных скважин по названию компании: {companyName}.");
            }
        }

        [HttpGet("depth/{wellId:int}")]
        public async Task<ActionResult<double>> GetTotalDepthByWellIdAsync(int wellId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            try
            {
                var foundWell = await _mediator.Send(new GetWellByIdQuery(wellId), cancellationToken).ConfigureAwait(false);

                if (foundWell == null)
                {
                    return NotFound();
                }

                float totalDepth =
                    (await _mediator.Send(new GetTelemetryByWellIdQuery(wellId), cancellationToken).ConfigureAwait(false))
                    .Where(t => t.DateTime >= fromDate && t.DateTime <= toDate)
                    .Sum(t => t.Depth);

                return Ok(totalDepth);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при получении общей глубины для скважины с идентификатором: {wellId}.");
            }
        }

        [HttpGet("depth/company/{companyId:int}")]
        public async Task<ActionResult<IEnumerable<TotalDepthWells>>> GetTotalDepthByCompanyIdAsync(int companyId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            try
            {
                var foundCompany = await _mediator.Send(new GetCompanyByIdQuery(companyId), cancellationToken).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<TotalDepthWells> activeWells =
                    (await _mediator.Send(new GetAllWellsByActivityParamQuery(ACTIVE), cancellationToken).ConfigureAwait(false))
                    .Where(w => w.CompanyId == companyId)
                    .Select(w => new TotalDepthWells()
                    {
                        WellName = w.Name,
                        Score = w.Telemetries.Where(t => t.DateTime >= fromDate && t.DateTime <= toDate).Sum(r => r.Depth)
                    });

                return Ok(activeWells);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при получении общей глубины для каждой скважины с идентификатором компании: {companyId}.");
            }
        }

        [HttpGet("depth/company/{companyName}")]
        public async Task<ActionResult<IEnumerable<TotalDepthWells>>> GetTotalDepthByCompanyNameAsync(string companyName, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            try
            {
                var foundCompany = await _mediator.Send(new GetCompanyByNameQuery(companyName), cancellationToken).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<TotalDepthWells> activeWells =
                    (await _mediator.Send(new GetAllWellsByActivityParamQuery(ACTIVE), cancellationToken).ConfigureAwait(false))
                    .Where(w => IsEquals(w.Company.Name, companyName))
                    .Select(w => new TotalDepthWells()
                    {
                        WellName = w.Name,
                        Score = w.Telemetries.Where(t => t.DateTime >= fromDate && t.DateTime <= toDate).Sum(r => r.Depth)
                    });

                return Ok(activeWells);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при получении общей глубины для каждой скважины с именем компании: {companyName}.");
            }
        }

        private bool IsActive(Well well) => well.Active == ACTIVE;

        private bool IsEquals(string a, string b) => a.ToUpper() == b.ToUpper();
    }
}
