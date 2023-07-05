using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers.V2;

[ApiVersion("2.0", Deprecated = true)]
[Route("api/companies")]
[ApiController]
[ApiExplorerSettings(GroupName = "v2")]
public class CompaniesV2Controller : ApiControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesV2Controller(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var baseResult = await _service.CompanyService
            .GetAllCompaniesAsync(trackChanges: false);

        var companiesV2 = baseResult.GetResult<IEnumerable<CompanyDto>>().Select(c => $"{c.Name} V2");

        return Ok(companiesV2);
    }
}