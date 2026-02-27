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
        CreateMap<Pessoa, PessoaDTO>().ReverseMap();
        CreateMap<Transacao, TransacaoDTO>().ReverseMap();
        CreateMap<Tipousuario, TipoUsuarioDTO>().ReverseMap();

        CreateMap<Usuario, UsuarioDTO>()
            .ForMember(dest => dest.TipoUsuario,
                opt => opt.MapFrom(src => src.TipoUsuario.Descricao))
            .ReverseMap()
            .ForMember(dest => dest.TipoUsuario, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Pessoas, opt => opt.Ignore())
            .ForMember(dest => dest.Categorias, opt => opt.Ignore())
            .ForMember(dest => dest.Transacoes, opt => opt.Ignore());

        CreateMap<IdentityRole, RoleDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        CreateMap<RoleDTO, IdentityRole>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.NormalizedName,
                opt => opt.MapFrom(src => src.Name.ToUpper()));
    }
}
