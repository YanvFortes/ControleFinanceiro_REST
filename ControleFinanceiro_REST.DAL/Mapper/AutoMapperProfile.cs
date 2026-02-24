using AutoMapper;
using ControleFinanceiro_REST.DAO.Entities;
using ControleFinanceiro_REST.DTO.Entities;

namespace ControleFinanceiro_REST.DAL.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Transacao, TransacaoDTO>().ReverseMap();
        CreateMap<Tipousuario, TipoUsuarioDTO>().ReverseMap();
        CreateMap<Usuario, UsuarioDTO>().ReverseMap();
    }
}
