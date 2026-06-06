using Microsoft.EntityFrameworkCore;
using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.Rules;
using Versatus.GestaoTributo.Domain.ICMS;
using Versatus.GestaoTributo.Infrastructure;

namespace Versatus.GestaoTributo.Tests;

public class RealDatabaseSchemaTests
{
    private const string ConnectionString = "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;";

    [Fact]
    public async Task GestaoTributo_ShouldMaterializeAllDbSetsFromRealDatabase()
    {
        var options = new DbContextOptionsBuilder<TributoDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new TributoDbContext(options);

        // Valida que as consultas no banco de dados real executam sem erros de esquema (colunas inválidas, tipos incorretos)
        await context.ClassificacoesFiscais.FirstOrDefaultAsync();
        await context.ClassificacoesFiscaisTributos.FirstOrDefaultAsync();
        await context.ClassificacoesFiscaisTributosFiliais.FirstOrDefaultAsync();
        await context.Cfops.FirstOrDefaultAsync();
        await context.Cests.FirstOrDefaultAsync();
        await context.CestNcms.FirstOrDefaultAsync();
        await context.CestSegmentos.FirstOrDefaultAsync();
        await context.TributosFiscais.FirstOrDefaultAsync();
        await context.UnidadesFiscais.FirstOrDefaultAsync();
        
        await context.SituacoesTributarias.FirstOrDefaultAsync();
        await context.Tributacoes.FirstOrDefaultAsync();
        await context.RegrasTributos.FirstOrDefaultAsync();
        await context.RegrasTributosConfiguracoes.FirstOrDefaultAsync();
        await context.RegrasTributacoesEspeciais.FirstOrDefaultAsync();
        await context.AplicacoesProdutos.FirstOrDefaultAsync();
        await context.AplicacoesProdutosTributos.FirstOrDefaultAsync();
        await context.AplicacoesNaturezasOperacoes.FirstOrDefaultAsync();
        await context.AplicacoesEspeciais.FirstOrDefaultAsync();

        await context.GruposTributariosICMS.FirstOrDefaultAsync();
        await context.GruposTributariosInventarioICMS.FirstOrDefaultAsync();
        await context.TributosIcmsSubstituicoesEstoque.FirstOrDefaultAsync();
        await context.DetalhesUfTributacao.FirstOrDefaultAsync();
        await context.DetalhesCidadeTributacao.FirstOrDefaultAsync();
        await context.RegimesTributariosVigencias.FirstOrDefaultAsync();
        await context.SimplesNacional.FirstOrDefaultAsync();
        await context.SimplesNacionalTributos.FirstOrDefaultAsync();
        await context.PartilhasICMSVigencias.FirstOrDefaultAsync();
    }
}
