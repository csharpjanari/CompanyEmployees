using AutoMapper;
using Contracts;
using Entities.Exceptions.BadRequest;
using Entities.Exceptions.NotFound;
using Entities.Models;
using Entities.Responses;
using Entities.Responses.NotFound;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryManager repository, ILoggerManager logger,
        IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }


    public async Task<ApiBaseResponse> GetAllCompaniesAsync(bool trackChanges)
    {
        var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);
        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        return new ApiOkResponse<IEnumerable<CompanyDto>>(companiesDto);
    }

    public async Task<ApiBaseResponse> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            return new CompanyNotFoundResponse(companyId);

        var companyDto = _mapper.Map<CompanyDto>(company);

        return new ApiOkResponse<CompanyDto>(companyDto);
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
    {
        var companyEntity = _mapper.Map<Company>(company);

        _repository.Company.CreateCompany(companyEntity);
        await _repository.SaveAsync();
        return _mapper.Map<CompanyDto>(companyEntity);
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

        _mapper.Map(companyForUpdate, company);
        await _repository.SaveAsync();
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);

        _repository.Company.DeleteCompany(company);
        await _repository.SaveAsync();
    }

    // Collections
    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new IdParameterBadRequestException();

        var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges);
        if (ids.Count() != companyEntities.Count())
            throw new CollectionByIdsBadRequestException();

        return _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
    }

    public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
            throw new CompanyCollectionBadRequest();

        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }

        await _repository.SaveAsync();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

        return (companies: companyCollectionToReturn, ids: ids);
    }

    // Refactoring code repetition
    private async Task<Company> GetCompanyAndCheckIfItExists(Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(id);

        return company;
    }
}