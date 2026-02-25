using AutoMapper;
using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;
using Microsoft.AspNetCore.Identity;

namespace ControleFinanceiro_REST.DAL.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Transacao, TransacaoDTO>().ReverseMap();
        CreateMap<Tipousuario, TipoUsuarioDTO>().ReverseMap();
        CreateMap<Usuario, UsuarioDTO>().ReverseMap();
        CreateMap<IdentityRole, RoleDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        CreateMap<RoleDTO, IdentityRole>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.NormalizedName,
                opt => opt.MapFrom(src => src.Name.ToUpper()));
    }
}
