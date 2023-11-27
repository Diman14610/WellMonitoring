using MediatR;
using Microsoft.AspNetCore.Mvc;
using WellServiceAPI.Domain.Queries;

namespace WellServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllCompanies(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<string> companies =
                    (await _mediator.Send(new GetAllCompaniesQuery(), cancellationToken).ConfigureAwait(false)).Select(company => company.Name);

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
