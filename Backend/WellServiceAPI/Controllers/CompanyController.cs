using Microsoft.AspNetCore.Mvc;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;

namespace WellServiceAPI.Controllers
{
    [ApiController]
    [Route("api/v1/company")]
    public class CompanyController : Controller
    {
        private readonly IQueryService<IEnumerable<Company>> _getAllCompanies;

        public CompanyController(
            IQueryService<IEnumerable<Company>> getAllCompanies
            )
        {
            _getAllCompanies = getAllCompanies ?? throw new ArgumentNullException(nameof(getAllCompanies));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllCompanies()
        {
            try
            {
                var companies = (await _getAllCompanies.ExecuteAsync()).Select(w => w.Name);

                return Ok(companies);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return StatusCode(500, $"Произошла ошибка при получении списка компаний.");
            }
        }
    }
}
