using ControleFinanceiro_REST.DTO.Utils;

namespace ControleFinanceiro_REST.BLL.Entities.Interfaces;

public interface IAutenticacaoBLL
{
    Task<RetornoDTO<string>> Login(AutenticacaoDTO entrada);
}
