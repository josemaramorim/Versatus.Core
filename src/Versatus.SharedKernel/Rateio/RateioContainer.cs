using Versatus.SharedKernel.Enums;

namespace Versatus.SharedKernel.Rateio;

/// <summary>
/// Um lançamento no acumulador de rateio: um valor a ratear, associado a uma dimensão
/// (classe / centro de custo / projeto) e à sua posição na hierarquia.
/// Origem: <c>struct</c> interna <c>AcumuladorItem</c> de
/// <c>Projeto.Servidor.ObjetosNegocio.AcessoGlobal.RateioMovto</c> (legado). <c>double</c> → <c>decimal</c>.
/// </summary>
/// <param name="Tipo">Dimensão do rateio.</param>
/// <param name="Id">Id do item de rateio (id da classe / centro de custo / projeto).</param>
/// <param name="IdPai">Id do item pai imediato na hierarquia (0 quando não há).</param>
/// <param name="IdPaiMaster">Id do ancestral "master"; -1 quando se trabalha só com centro de custo/projeto (convenção do legado).</param>
/// <param name="IdFilialOrigem">Filial de origem do valor.</param>
/// <param name="Valor">Valor a ratear; sinal preservado (negativo = natureza devedora).</param>
public readonly record struct AcumuladorItem(
    TipoRateioItem Tipo,
    int Id,
    int IdPai,
    int IdPaiMaster,
    int IdFilialOrigem,
    decimal Valor);

/// <summary>
/// Container de acumulação de rateio: pilha em memória de <see cref="AcumuladorItem"/> que
/// documento/movimento vão preenchendo antes do rateio ser efetivamente aplicado.
///
/// <para><b>Escopo (E0-T03 · CLR-01/CLR-02):</b> só a ESTRUTURA de acumulação e as somas —
/// <see cref="Add(TipoRateioItem,int,int,decimal,int)"/>, <see cref="ValorBase"/>,
/// <see cref="ValorPorId"/>, <see cref="Percentual"/>, <see cref="Limpar"/>. O <b>motor</b> de
/// rateio (regras de origem, contra-partida, persistência de <c>RateioMovtoItem</c>,
/// parâmetros <c>GloParametro</c> — UsaClasse / ProjetoPrecedeCentroCusto —, arredondamento e
/// recursão em dimensões aninhadas) é do MOD-02 (<c>RateioMovto</c>/<c>ManutencaoRateio</c>,
/// CLR-01) e é consumido pelo MOD-05 no épico E5. Não replicar aqui.</para>
///
/// Origem: região "Métodos para acumular rateio" de <c>RateioMovto</c> (legado).
/// </summary>
public sealed class RateioContainer
{
    private readonly List<AcumuladorItem> _itens = new();

    /// <summary>Itens acumulados, na ordem de inserção.</summary>
    public IReadOnlyList<AcumuladorItem> Itens => _itens;

    /// <summary>True quando nada foi acumulado.</summary>
    public bool Vazio => _itens.Count == 0;

    /// <summary>
    /// Adiciona um valor trabalhando só com centro de custo / projeto
    /// (<see cref="AcumuladorItem.IdPaiMaster"/> = -1).
    /// </summary>
    public void Add(TipoRateioItem tipo, int id, int idPai, decimal valor, int idFilialOrigem)
        => _itens.Add(new AcumuladorItem(tipo, id, idPai, IdPaiMaster: -1, idFilialOrigem, valor));

    /// <summary>
    /// Adiciona um valor com a hierarquia completa classe / centro de custo / projeto.
    /// </summary>
    public void Add(TipoRateioItem tipo, int id, int idPaiMaster, int idPai, decimal valor, int idFilialOrigem)
        => _itens.Add(new AcumuladorItem(tipo, id, idPai, idPaiMaster, idFilialOrigem, valor));

    /// <summary>
    /// Soma dos valores acumulados para uma dimensão e um nó da hierarquia — o "valor base"
    /// sobre o qual os percentuais de rateio são calculados.
    /// (<c>AcumuladorValorBase</c> do legado.)
    /// </summary>
    public decimal ValorBase(TipoRateioItem tipo, int idPai, int idPaiMaster)
    {
        decimal total = 0m;
        foreach (var i in _itens)
        {
            if (Corresponde(i, tipo, idPai, idPaiMaster))
                total += i.Valor;
        }
        return total;
    }

    /// <summary>
    /// Soma dos valores acumulados para um item de rateio específico (mesmo <c>Id</c>)
    /// dentro de uma dimensão/nó da hierarquia.
    /// </summary>
    public decimal ValorPorId(TipoRateioItem tipo, int id, int idPai, int idPaiMaster)
    {
        decimal total = 0m;
        foreach (var i in _itens)
        {
            if (i.Id == id && Corresponde(i, tipo, idPai, idPaiMaster))
                total += i.Valor;
        }
        return total;
    }

    /// <summary>
    /// Ids distintos de itens de rateio presentes numa dimensão/nó da hierarquia,
    /// na ordem de primeira ocorrência.
    /// </summary>
    public IReadOnlyList<int> IdsDistintos(TipoRateioItem tipo, int idPai, int idPaiMaster)
    {
        var ids = new List<int>();
        foreach (var i in _itens)
        {
            if (Corresponde(i, tipo, idPai, idPaiMaster) && !ids.Contains(i.Id))
                ids.Add(i.Id);
        }
        return ids;
    }

    /// <summary>
    /// Percentual de rateio de um item = (valor do item / valor base) * 100.
    /// Réplica direta do cálculo do legado (<c>RateioMovto.AcumuladorAplicar</c>).
    /// Retorna 0 quando o valor base é 0.
    /// </summary>
    public decimal Percentual(TipoRateioItem tipo, int id, int idPai, int idPaiMaster)
    {
        var valorBase = ValorBase(tipo, idPai, idPaiMaster);
        if (valorBase == 0m)
            return 0m;
        return ValorPorId(tipo, id, idPai, idPaiMaster) / valorBase * 100m;
    }

    /// <summary>Esvazia o acumulador. (<c>LimparAcumulador</c> do legado.)</summary>
    public void Limpar() => _itens.Clear();

    /// <summary>
    /// Regra de correspondência do legado (<c>ValidarRateioAcumulado</c>, sem a checagem de
    /// "item já processado"): mesmo <c>Tipo</c>; <c>IdPaiMaster</c> ignorado quando &lt; 0;
    /// mesmo <c>IdPai</c>.
    /// </summary>
    private static bool Corresponde(AcumuladorItem i, TipoRateioItem tipo, int idPai, int idPaiMaster)
    {
        if (i.Tipo != tipo)
            return false;
        if (i.IdPaiMaster >= 0 && i.IdPaiMaster != idPaiMaster)
            return false;
        return i.IdPai == idPai;
    }
}
