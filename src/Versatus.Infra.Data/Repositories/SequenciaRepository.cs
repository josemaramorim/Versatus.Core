using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Versatus.Framework.Domain.Entities;
using Versatus.Framework.Repositories;
using Versatus.Framework.Sequences;
using Versatus.Infra.Data.Context;

namespace Versatus.Infra.Data.Repositories;

/// <summary>
/// Implementação concreta do repositório de sequenciais.
/// </summary>
public class SequenciaRepository : RepositorioBase<Sequencia>, ISequenciaRepository
{
    public SequenciaRepository(VersatusDbContext context) : base(context)
    {
    }

    public async Task<Sequencia?> GetByTabelaFilialAsync(string tabela, int idFilial, CancellationToken cancellationToken = default)
    {
        var connection = Context.Database.GetDbConnection();
        var wasClosed = connection.State == System.Data.ConnectionState.Closed;
        if (wasClosed)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            using (var command = connection.CreateCommand())
            {
                if (Context.Database.CurrentTransaction != null)
                {
                    command.Transaction = Microsoft.EntityFrameworkCore.Storage.DbContextTransactionExtensions.GetDbTransaction(Context.Database.CurrentTransaction);
                }

                command.CommandText = @"
                    SELECT item.Numero 
                    FROM GloSequencialItem item
                    JOIN GloSequencial seq ON item.IDGLOSEQUENCIAL = seq.IDGLOSEQUENCIAL
                    WHERE seq.NOMEOBJETO = @tabela
                      AND (item.IDGLOFILIAL = @idFilial OR (@idFilial = 0 AND item.IDGLOFILIAL IS NULL))";

                var pTabela = command.CreateParameter();
                pTabela.ParameterName = "@tabela";
                pTabela.Value = tabela;
                command.Parameters.Add(pTabela);

                var pFilial = command.CreateParameter();
                pFilial.ParameterName = "@idFilial";
                pFilial.Value = idFilial;
                command.Parameters.Add(pFilial);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                if (result == null || result == DBNull.Value)
                {
                    return null;
                }

                return new Sequencia
                {
                    Tabela = tabela,
                    IdFilial = idFilial,
                    ValorAtual = Convert.ToInt32(result)
                };
            }
        }
        finally
        {
            if (wasClosed)
            {
                await connection.CloseAsync();
            }
        }
    }

    public override async Task AddAsync(Sequencia entity, CancellationToken cancellationToken = default)
    {
        var connection = Context.Database.GetDbConnection();
        var wasClosed = connection.State == System.Data.ConnectionState.Closed;
        if (wasClosed)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            using (var command = connection.CreateCommand())
            {
                if (Context.Database.CurrentTransaction != null)
                {
                    command.Transaction = Microsoft.EntityFrameworkCore.Storage.DbContextTransactionExtensions.GetDbTransaction(Context.Database.CurrentTransaction);
                }

                command.CommandText = "SELECT IDGLOSEQUENCIAL FROM GloSequencial WHERE NOMEOBJETO = @tabela";
                
                var pTabela = command.CreateParameter();
                pTabela.ParameterName = "@tabela";
                pTabela.Value = entity.Tabela;
                command.Parameters.Add(pTabela);

                var idSeqObj = await command.ExecuteScalarAsync(cancellationToken);
                int idSequencial;

                if (idSeqObj == null || idSeqObj == DBNull.Value)
                {
                    command.Parameters.Clear();
                    command.CommandText = @"
                        INSERT INTO GloSequencial (NOMEOBJETO, IDTIPO, ALTERA) 
                        VALUES (@tabela, 1, 1); 
                        SELECT SCOPE_IDENTITY();";
                    
                    var pTabela2 = command.CreateParameter();
                    pTabela2.ParameterName = "@tabela";
                    pTabela2.Value = entity.Tabela;
                    command.Parameters.Add(pTabela2);

                    idSequencial = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
                }
                else
                {
                    idSequencial = Convert.ToInt32(idSeqObj);
                }

                command.Parameters.Clear();
                command.CommandText = @"
                    INSERT INTO GloSequencialItem (IDGLOSEQUENCIAL, IDGLOFILIAL, NUMERO)
                    VALUES (@idSequencial, @idFilial, @numero)";

                var pIdSeq = command.CreateParameter();
                pIdSeq.ParameterName = "@idSequencial";
                pIdSeq.Value = idSequencial;
                command.Parameters.Add(pIdSeq);

                var pFilial = command.CreateParameter();
                pFilial.ParameterName = "@idFilial";
                pFilial.Value = entity.IdFilial == 0 ? DBNull.Value : (object)entity.IdFilial;
                command.Parameters.Add(pFilial);

                var pNumero = command.CreateParameter();
                pNumero.ParameterName = "@numero";
                pNumero.Value = entity.ValorAtual;
                command.Parameters.Add(pNumero);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        finally
        {
            if (wasClosed)
            {
                await connection.CloseAsync();
            }
        }
    }

    public override async Task UpdateAsync(Sequencia entity, CancellationToken cancellationToken = default)
    {
        var connection = Context.Database.GetDbConnection();
        var wasClosed = connection.State == System.Data.ConnectionState.Closed;
        if (wasClosed)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            using (var command = connection.CreateCommand())
            {
                if (Context.Database.CurrentTransaction != null)
                {
                    command.Transaction = Microsoft.EntityFrameworkCore.Storage.DbContextTransactionExtensions.GetDbTransaction(Context.Database.CurrentTransaction);
                }

                command.CommandText = @"
                    UPDATE item 
                    SET item.Numero = @numero 
                    FROM GloSequencialItem item
                    JOIN GloSequencial seq ON item.IDGLOSEQUENCIAL = seq.IDGLOSEQUENCIAL
                    WHERE seq.NOMEOBJETO = @tabela
                      AND (item.IDGLOFILIAL = @idFilial OR (@idFilial = 0 AND item.IDGLOFILIAL IS NULL))";

                var pNumero = command.CreateParameter();
                pNumero.ParameterName = "@numero";
                pNumero.Value = entity.ValorAtual;
                command.Parameters.Add(pNumero);

                var pTabela = command.CreateParameter();
                pTabela.ParameterName = "@tabela";
                pTabela.Value = entity.Tabela;
                command.Parameters.Add(pTabela);

                var pFilial = command.CreateParameter();
                pFilial.ParameterName = "@idFilial";
                pFilial.Value = entity.IdFilial;
                command.Parameters.Add(pFilial);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        finally
        {
            if (wasClosed)
            {
                await connection.CloseAsync();
            }
        }
    }
}
