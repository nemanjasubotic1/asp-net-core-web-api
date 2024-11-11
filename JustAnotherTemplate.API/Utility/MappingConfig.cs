using AutoMapper;
using JustAnother.Model.Entity;
using JustAnother.Model.Entity.DTO;

namespace JustAnother.API.Utility;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Category, CategoryCreateDTO>().ReverseMap();
        CreateMap<Category, CategoryUpdateDTO>().ReverseMap();
        CreateMap<Category, CategoryDeleteDTO>().ReverseMap();
    }

}
