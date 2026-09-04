namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Subtipo de conta bancária (corrente/investimento) — só se aplica quando ContaTipo = Banco.
/// Origem: Projeto.Geral.Enumerado.TipoContaBancaria (legado, [TipoEnumerado(1478)];
/// membro legado chamado ContaBancariaTipo) — persistido em
/// FINCONTABANCARIA.IDTIPOCONTABANCARIA. Não confundir com ContaFinanceiroTipo (177).
/// </summary>
public enum TipoContaBancaria
{
    ContaCorrente = 1479,
    Investimento = 1480
}
