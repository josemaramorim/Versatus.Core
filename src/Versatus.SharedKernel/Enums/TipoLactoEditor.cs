namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Origem do lançamento no editor de domínio (avulso/cadastro) — checar necessidade real
/// no backend novo (é conceito de editor de UI do legado; pode não sobreviver ao mapeamento
/// da tela nova).
/// Origem: Projeto.Geral.EnumeradoObjeto.TipoLactoEditor (legado) — não-persistido
/// (comportamento em memória, nunca vira coluna).
/// </summary>
public enum TipoLactoEditor
{
    Avulso = 1,
    Cadastro = 2
}
