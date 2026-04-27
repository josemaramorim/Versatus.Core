using System.Collections.Generic;

namespace Versatus.Framework.Paginacao;

/// <summary>
/// Representa o resultado de uma consulta paginada.
/// Implementado como record para garantir imutabilidade e facilidade de transporte, conforme MOD-01.
/// </summary>
/// <typeparam name="T">Tipo dos itens na lista.</typeparam>
/// <param name="Items">Lista imutável contendo os itens da página atual.</param>
/// <param name="Total">Contagem total de registros disponíveis sem a paginação.</param>
public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
