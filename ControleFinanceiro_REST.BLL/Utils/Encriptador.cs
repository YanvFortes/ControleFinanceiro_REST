using ControleFinanceiro_REST.BLL.Utils.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ControleFinanceiro_REST.BLL.Utils;

public class Encriptador : IEncriptador
{
    private readonly byte[] _chave;
    private readonly byte[] _iv;
    public Encriptador(IConfiguration configuration)
    {
        var chaveSecreta = configuration["Encriptador:ChaveSecreta"];

        _chave = Encoding.UTF8.GetBytes(chaveSecreta);
        _iv = new byte[16];
    }

    public string Desencriptar(string entrada)
    {
        var buffer = Convert.FromBase64String(entrada);

        using var aes = Aes.Create();
        aes.Key = _chave;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }

    public string Encriptar(string entrada)
    {
        using var aes = Aes.Create();
        aes.Key = _chave;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(entrada);

        sw.Flush();
        cs.FlushFinalBlock();

        return Convert.ToBase64String(ms.ToArray());
    }
}
