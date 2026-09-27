using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Versatus.GestaoFinanceira.Infrastructure;
using Versatus.GestaoFinanceira.Infrastructure.Migrations;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T08 — a baseline do GestaoFinanceiraDbContext é vazia (schema-first: as tabelas FIN* do legado
/// já existem) e o snapshot fixa o modelo do E3.
/// </summary>
public class MigrationBaselineE3Tests
{
    [Fact]
    public void Baseline_NaoCriaNemRemoveTabelas()
    {
        var baseline = new BaselineE3();

        baseline.UpOperations.Should().BeEmpty("as tabelas do legado já existem — nada pode ser criado/alterado");
        baseline.DownOperations.Should().BeEmpty("nunca remover tabelas do legado");
    }

    [Fact]
    public void Snapshot_ContemAsSeisTabelasDoE3()
    {
        // O snapshot gerado pelo EF é internal: instancia pelo tipo.
        var tipoSnapshot = typeof(BaselineE3).Assembly.GetTypes().Single(t => t.IsSubclassOf(typeof(ModelSnapshot)));
        var snapshot = (ModelSnapshot)Activator.CreateInstance(tipoSnapshot, nonPublic: true)!;

        var tabelas = snapshot.Model.GetEntityTypes()
            .Select(e => e.GetTableName())
            .ToList();

        tabelas.Should().BeEquivalentTo(
            ["FINCAIXABANCO", "FINCAIXABANCOUSUARIO", "FINCONTABANCARIA", "FINSALDOCAIXABANCO", "FINSALDORATEIO", "FINCOBRADOR"]);
    }

    [Fact]
    public void HistoricoDeMigrations_EhProprioDoModulo()
    {
        GestaoFinanceiraDbContext.TabelaHistoricoMigrations.Should().Be("__EFMigrationsHistory_GestaoFinanceira");
    }
}
