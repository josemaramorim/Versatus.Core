using System.Threading;
using System.Threading.Tasks;
using Versatus.Framework.Sequences;

namespace Versatus.WebAPI.Services;

public class FakeGeradorSequencial : IGeradorSequencial
{
    private int _current = 10000;
    public Task<int> ProximoAsync(string nomeObjeto, SequencialTipo tipo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Interlocked.Increment(ref _current));
    }

    public Task SetValorAsync(string nomeObjeto, SequencialTipo tipo, int novoValor, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
