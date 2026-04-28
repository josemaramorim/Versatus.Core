namespace Versatus.Framework.Contexto;

/// <summary>
/// Contexto de execução da aplicação, substitui IAmbiente do legado.
/// Fornece informações sobre filial, usuário, empresa e perfis correntes.
/// </summary>
public interface IContextoExecucao
{
    int IdFilial { get; set; }
    int IdUsuario { get; set; }
    int IdEmpresa { get; set; }
    string[] Perfis { get; set; }
}