namespace ControleFinanceiro_REST.BLL.Utils.Interfaces;

public interface IEncriptador
{
    string Encriptar(string entrada);
    string Desencriptar(string entrada);
}
