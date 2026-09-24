using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Versatus.Framework.Sequences;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Services;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T05 — CobradorService: CRUD simples (sem VAL na matriz-rtv.md#E3) — sequencial por
/// filial, auditoria e paginação materializada.
/// </summary>
public class CobradorServiceTests
{
    private static Cobrador Novo(int id = 0, string nome = "Cobrador") => new()
    {
        IdCobrador = id,
        IdFilial = CenarioE3.Filial,
        IdEntidade = 50,
        Nome = nome,
        Ativo = true,
    };

    [Fact]
    public async Task Criar_GeraSequencialPorFilialEAuditoria()
    {
        using var c = new CenarioE3();

        var r = await c.CobradorService().CriarAsync(Novo());

        r.IsSuccess.Should().BeTrue();
        c.Sequencial.Verify(x => x.ProximoAsync(CobradorService.NomeSequencial, SequencialTipo.Filial, It.IsAny<CancellationToken>()));
        var lido = await c.ReadContext.Cobradores.SingleAsync();
        lido.IdCobrador.Should().Be(100);
        lido.IdFilial.Should().Be(CenarioE3.Filial);
        lido.IdUsuarioInclusao.Should().Be(CenarioE3.Usuario);
    }

    [Fact]
    public async Task Atualizar_AlteraCamposEAuditoria()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(Novo(1));

        var r = await c.CobradorService().AtualizarAsync(Novo(1, "Outro nome"));

        r.IsSuccess.Should().BeTrue();
        var lido = await c.ReadContext.Cobradores.SingleAsync();
        lido.Nome.Should().Be("Outro nome");
        lido.IdUsuarioAlteracao.Should().Be(CenarioE3.Usuario);
    }

    [Fact]
    public async Task AtualizarInexistente_Falha()
    {
        using var c = new CenarioE3();

        var r = await c.CobradorService().AtualizarAsync(Novo(9));

        r.Errors.Single().Mensagem.Should().Be(CobradorService.MsgNaoEncontrado);
    }

    [Fact]
    public async Task Excluir_RemoveCobrador()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(Novo(1));

        (await c.CobradorService().ExcluirAsync(1, CenarioE3.Filial)).IsValid.Should().BeTrue();

        (await c.ReadContext.Cobradores.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task ListarPaginado_FiltraPorTextoEPagina()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(Novo(1, "Ana"), Novo(2, "Bruno"), Novo(3, "Ana Paula"));

        var r = await c.CobradorService().ListarPaginadoAsync("Ana", null, page: 1, limit: 1);

        r.Total.Should().Be(2);
        r.Items.Single().IdCobrador.Should().Be(1);
    }
}
