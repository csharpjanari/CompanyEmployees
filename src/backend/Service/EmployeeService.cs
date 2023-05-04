using AutoMapper;
using Contracts;
using Entities.Exceptions.NotFound;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger,
        IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }


    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employee = _repository.Employee.GetEmployee(companyId, id, trackChanges);
        if (employee is null)
            throw new EmployeeNotFoundException(id);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employees = _repository.Employee.GetEmployees(companyId, trackChanges);

        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto,
        bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreationDto);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        _repository.Save();
    
        return _mapper.Map<EmployeeDto>(employeeEntity);
    }

    public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employee = _repository.Employee.GetEmployee(companyId, id, trackChanges);
        if (employee is null)
            throw new EmployeeNotFoundException(id);

        _repository.Employee.DeleteEmployeeForCompany(employee);
        _repository.Save();
    }

    public void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate,
        bool compTrackChanges, bool empTrackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, compTrackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employee = _repository.Employee.GetEmployee(companyId, id, empTrackChanges);
        if (employee is null)
            throw new EmployeeNotFoundException(id);

        _mapper.Map(employeeForUpdate, employee);
        _repository.Save();
    }
}