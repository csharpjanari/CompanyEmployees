using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(c => c.FullAddress,
                opt => opt.MapFrom(x => string.Join(' ', $"{x.Address}. Country - {x.Country}")));
        CreateMap<Employee, EmployeeDto>();

        CreateMap<CompanyForCreationDto, Company>();
        CreateMap<EmployeeForCreationDto, Employee>();

        CreateMap<CompanyForUpdateDto, Company>();
        CreateMap<EmployeeForUpdateDto, Employee>();

        CreateMap<UserForRegistrationDto, User>();
    }
}