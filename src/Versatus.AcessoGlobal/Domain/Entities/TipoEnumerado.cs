using System;

namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Representa as opções de enumerados localizados persistidos no banco de dados.
/// Origem: servidor/objeto de negócio/acesso.global/TipoEnumerado.cs (legado)
/// Tabela: GloTipoEnumerado
/// </summary>
public class TipoEnumerado
{
    /// <summary>
    /// Identificador do item enumerado (valor numérico real do enum).
    /// </summary>
    public int IdTipoEnumerado { get; set; }

    /// <summary>
    /// Identificador do tipo enumerado pai (grupo).
    /// </summary>
    public int? IdTipoEnumeradoPai { get; set; }

    /// <summary>
    /// Descrição/Label exibida para o usuário final.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Ordem de exibição do item nos seletores.
    /// </summary>
    public int Ordem { get; set; }
}
