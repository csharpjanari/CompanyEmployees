using Presentation.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Presentation.ActionFilters;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Entities.Responses;
using Presentation.Extensions;

namespace Presentation.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/companies")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
public class CompaniesV1Controller : ApiControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesV1Controller(IServiceManager service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var baseResult = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
        var companies = baseResult.GetResult<IEnumerable<CompanyDto>>();

        return Ok(companies);
    }


    [HttpGet("{id:guid}", Name = "CompanyById")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
    [HttpCacheValidation(MustRevalidate = false)]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var baseResult = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
        if (!baseResult.Success)
            return ProcessError(baseResult);

        var company = baseResult.GetResult<CompanyDto>();
        return Ok(company);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
    {
        var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
        return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
    }


    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto companyForUpdateDto)
    {
        await _service.CompanyService.UpdateCompanyAsync(id, companyForUpdateDto, trackChanges: true);
        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
        return NoContent();
    }

    // Collection Endpoits

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> CompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]
        IEnumerable<Guid> ids)
    {
        var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(companies);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection([FromBody]
        IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
        return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
    }

    // OPTIONS
    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");
        return Ok();
    }
}