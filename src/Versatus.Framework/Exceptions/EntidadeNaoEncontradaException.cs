using System;

namespace Versatus.Framework.Exceptions;

/// <summary>
/// Exceção utilizada quando uma pesquisa por uma entidade específica no banco de dados falha (ex: busca por ID).
/// Herdando de VersatusException, ela assegura que o sistema não use exceções genéricas para entidades não encontradas.
/// </summary>
public class EntidadeNaoEncontradaException : VersatusException
{
    public string TipoEntidade { get; }
    public object ChaveBusca { get; }

    public EntidadeNaoEncontradaException(Type tipoEntidade, object chaveBusca) 
        : base($"Entidade do tipo '{tipoEntidade.Name}' com a chave '{chaveBusca}' não foi encontrada.")
    {
        TipoEntidade = tipoEntidade.Name;
        ChaveBusca = chaveBusca;
        ErrorCode = "ENTITY_NOT_FOUND";
    }
}
