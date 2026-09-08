using Versatus.SharedKernel.Enums;

namespace Versatus.SharedKernel.Abstractions;

/// <summary>
/// Contrato de uma classe que fornece os dados-base para o cálculo de comissão
/// (documento, liquidação, reversão...).
///
/// Mora no <c>Versatus.SharedKernel</c> — e não no MOD-05 — para que um futuro módulo de
/// Faturamento consuma o cálculo de comissão sem depender da Gestão Financeira (CLR-05).
///
/// Origem: <c>Projeto.Servidor.Interface.AcessoGlobal.IDadosComissao</c> (legado).
/// Modernizado: removida a herança de <c>IGeneratorDataSource</c> (plumbing de
/// Remoting/Gentle); <c>double</c> → <c>decimal</c>.
/// </summary>
public interface IDadosComissao
{
    /// <summary>Filial de origem do processo que gera a comissão.</summary>
    int IdFilialOrigemComissao { get; }

    /// <summary>Histórico textual do movimento de comissão.</summary>
    string HistoricoMovtoComissao { get; }

    /// <summary>Valor líquido — base de cálculo da comissão.</summary>
    decimal ValorBase { get; }

    /// <summary>Valor bruto / total parcelado.</summary>
    decimal ValorTotalParcelado { get; }

    /// <summary>Data de referência da comissão.</summary>
    DateTime DataComissao { get; }

    /// <summary>Natureza (credora/devedora) do processo que gera a comissão.</summary>
    NaturezaTipo TipoNatureza { get; }

    /// <summary>Origem do processo (venda, documento, liquidação, reversão...).</summary>
    TipoProcessoComissao TipoProcesso { get; }

    // DÚVIDA (E0-T03): o legado também expõe ComissionadosComissao e ParcelasComissao como
    // IList não-tipada, mas os elementos são entidades de domínio da Gestão Financeira
    // (IDocumentoComissionado / IDocumentoParcela). Tipá-los aqui faria o SharedKernel
    // depender do MOD-05 (proibido). A abstração desses dois conjuntos — quais campos o
    // cálculo de comissão realmente lê — deve ser desenhada junto com a migração do
    // cálculo de comissão (Faturamento ou MOD-05 E4/E8), não inventada agora.
}
