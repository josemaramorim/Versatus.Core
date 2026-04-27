namespace Versatus.Framework.Contexto;

/// <summary>
/// Implementação concreta do contexto de execução.
/// Esta classe é preenchida a partir dos dados da requisição (JWT) e injetada via DI.
/// </summary>
public class ContextoExecucao : IContextoExecucao
{
    public int IdFilial { get; set; }
    public int IdUsuario { get; set; }
    public int IdEmpresa { get; set; }
    public string[] Perfis { get; set; } = [];

    // Construtor vazio para ser usado pelo inicializador/extensions
    public ContextoExecucao()
    {
    }

    // Facilita a criação manual em cenários específicos
    public ContextoExecucao(int idFilial, int idUsuario, int idEmpresa, string[] perfis)
    {
        IdFilial = idFilial;
        IdUsuario = idUsuario;
        IdEmpresa = idEmpresa;
        Perfis = perfis ?? [];
    }
}
