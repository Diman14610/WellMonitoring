using Microsoft.AspNetCore.Mvc;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;
using WellServiceAPI.Shared.Response.Well;

namespace WellServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/well")]
    public class WellController : ControllerBase
    {
        private const int ACTIVE = 1;

        private readonly IQueryService<GetWellById, Well> _getWellById;
        private readonly IQueryService<GetAllWellsByActivityParam, IEnumerable<Well>> _getAllWellsByActive;
        private readonly IQueryService<IEnumerable<Well>> _getAllWells;
        private readonly IQueryService<GetCompanyByName, Company> _getCompanyByName;
        private readonly IQueryService<GetTelemetryByWellId, IEnumerable<Telemetry>> _getTelemetryByWellId;

        public WellController(
            IQueryService<GetWellById, Well> getWellById,
            IQueryService<GetCompanyByName, Company> getCompanyByName,
            IQueryService<IEnumerable<Well>> getAllWells,
            IQueryService<GetAllWellsByActivityParam, IEnumerable<Well>> getAllWellsByActive,
            IQueryService<GetTelemetryByWellId, IEnumerable<Telemetry>> getTelemetryByWellId)
        {
            _getWellById = getWellById ?? throw new ArgumentNullException(nameof(getWellById));
            _getCompanyByName = getCompanyByName ?? throw new ArgumentNullException(nameof(getCompanyByName));
            _getAllWells = getAllWells ?? throw new ArgumentNullException(nameof(getAllWells));
            _getAllWellsByActive = getAllWellsByActive ?? throw new ArgumentNullException(nameof(getAllWellsByActive));
            _getTelemetryByWellId = getTelemetryByWellId ?? throw new ArgumentNullException(nameof(getTelemetryByWellId));
        }

        [HttpGet("{wellId:int}")]
        public async Task<ActionResult<string>> GetWellByIdAsync(int wellId)
        {
            try
            {
                var well = await _getWellById.ExecuteAsync(new GetWellById(wellId)).ConfigureAwait(false);

                if (well == null)
                {
                    return NotFound();
                }

                return Ok(well.Name);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при извлечении скважины с идентификатором: {wellId}.");
            }
        }

        [HttpGet("company/{companyName}")]
        public async Task<ActionResult<IEnumerable<string>>> GetWellsByCompanyNameAsync(string companyName)
        {
            try
            {
                var foundCompany = await _getCompanyByName.ExecuteAsync(new GetCompanyByName(companyName)).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<string> wellsNames = (await _getAllWells.ExecuteAsync().ConfigureAwait(false))
                    .Where(w => IsEquals(w.Company.Name, companyName))
                    .Select(w => w.Name);

                return Ok(wellsNames);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при извлечении скважин по названию компании: {companyName}.");
            }
        }

        [HttpGet("active/all")]
        public async Task<ActionResult<IEnumerable<WellsWithContractors>>> GetActiveWellsWithContractorsAsync()
        {
            try
            {
                IEnumerable<WellsWithContractors> wellInfos =
                    (await _getAllWellsByActive.ExecuteAsync(new GetAllWellsByActivityParam(1)).ConfigureAwait(false))
                    .Select(w => new WellsWithContractors { WellName = w.Name, Contractor = w.Company.Name });

                return Ok(wellInfos);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, "Произошла ошибка при извлечении активных скважин подрядчиков.");
            }
        }

        [HttpGet("active/{wellId:int}")]
        public async Task<ActionResult<string>> GetActiveWellsByIdAsync(int wellId)
        {
            try
            {
                var well = await _getWellById.ExecuteAsync(new GetWellById(wellId)).ConfigureAwait(false);

                if (well == null || !IsActive(well))
                {
                    return NotFound();
                }

                return Ok(well.Name);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при извлечении активной скважины с идентификатором: {wellId}.");
            }
        }

        [HttpGet("active/company/{companyName}")]
        public async Task<ActionResult<IEnumerable<string>>> GetActiveWellsByCompanyNameAsync(string companyName)
        {
            try
            {
                var foundCompany = await _getCompanyByName.ExecuteAsync(new GetCompanyByName(companyName)).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<string> wellsNames =
                    (await _getAllWellsByActive.ExecuteAsync(new GetAllWellsByActivityParam(1)).ConfigureAwait(false))
                    .Where(w => IsEquals(w.Company.Name, companyName))
                    .Select(w => w.Name);

                return Ok(wellsNames);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при извлечении активных скважин по названию компании: {companyName}.");
            }
        }

        [HttpGet("depth/{wellId:int}")]
        public async Task<ActionResult<double>> GetTotalDepthByWellIdAsync(int wellId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var foundWell = await _getWellById.ExecuteAsync(new GetWellById(wellId)).ConfigureAwait(false);

                if (foundWell == null)
                {
                    return NotFound();
                }

                float totalDepth =
                    (await _getTelemetryByWellId.ExecuteAsync(new GetTelemetryByWellId(wellId)).ConfigureAwait(false))
                    .Where(t => t.DateTime >= fromDate && t.DateTime <= toDate)
                    .Sum(t => t.Depth);

                return Ok(totalDepth);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при получении общей глубины для скважины с идентификатором: {wellId}.");
            }
        }

        [HttpGet("depth/company/{companyId:int}")]
        public async Task<ActionResult<IEnumerable<TotalDepthWells>>> GetTotalDepthByCompanyIdAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var foundCompany = await _getWellById.ExecuteAsync(new GetWellById(companyId)).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<TotalDepthWells> activeWells =
                    (await _getAllWellsByActive.ExecuteAsync(new GetAllWellsByActivityParam(1)).ConfigureAwait(false))
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
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при получении общей глубины для каждой скважины с идентификатором компании: {companyId}.");
            }
        }

        [HttpGet("depth/company/{companyName}")]
        public async Task<ActionResult<IEnumerable<TotalDepthWells>>> GetTotalDepthByCompanyNameAsync(string companyName, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var foundCompany = await _getCompanyByName.ExecuteAsync(new GetCompanyByName(companyName)).ConfigureAwait(false);

                if (foundCompany == null)
                {
                    return NotFound();
                }

                IEnumerable<TotalDepthWells> activeWells =
                    (await _getAllWellsByActive.ExecuteAsync(new GetAllWellsByActivityParam(1)).ConfigureAwait(false))
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
                await Console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return StatusCode(500, $"Произошла ошибка при получении общей глубины для каждой скважины с именем компании: {companyName}.");
            }
        }

        private bool IsActive(Well well) => well.Active == ACTIVE;

        private bool IsEquals(string a, string b) => a.ToUpper() == b.ToUpper();
    }
}
