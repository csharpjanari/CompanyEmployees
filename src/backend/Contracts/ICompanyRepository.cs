using Entities.Models;

namespace Contracts;

public interface ICompanyRepository
{
    // CRUD
    Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
    Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);
    void CreateCompany(Company company);
    void DeleteCompany(Company company);

    // Collection
    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
}