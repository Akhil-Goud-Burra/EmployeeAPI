using AutoMapper;
using Employee_API.Models;
using Employee_API.Models.Dto;

namespace Employee_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Employee, EmployeeCreateDTO>().ReverseMap();
        }
    }
}
