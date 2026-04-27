namespace Versatus.Framework.Contexto;

/// <summary>
/// Contexto de execução da aplicação, substitui IAmbiente do legado.
/// Fornece informações sobre filial, usuário, empresa e perfis correntes.
/// </summary>
public interface IContextoExecucao
{
    int IdFilial { get; }
    int IdUsuario { get; }
    int IdEmpresa { get; }
    string[] Perfis { get; }
}