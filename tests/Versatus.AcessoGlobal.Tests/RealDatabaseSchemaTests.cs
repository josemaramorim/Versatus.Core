using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Security;
using Versatus.AcessoGlobal.Domain.Finance;
using Versatus.AcessoGlobal.Domain.Configuration;
using Versatus.AcessoGlobal.Domain.Location;
using Versatus.AcessoGlobal.Domain.Organization;
using Versatus.AcessoGlobal.Domain.Classification;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Tests;

public class RealDatabaseSchemaTests
{
    private const string ConnectionString = "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;";

    [Fact]
    public async Task AcessoGlobal_ShouldMaterializeAllDbSetsFromRealDatabase()
    {
        var options = new DbContextOptionsBuilder<AcessoGlobalDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var context = new AcessoGlobalDbContext(options);

        // Valida que as consultas no banco de dados real executam sem erros de esquema (colunas inválidas, tipos incorretos)
        await context.Paises.FirstOrDefaultAsync();
        await context.Estados.FirstOrDefaultAsync();
        await context.Cidades.FirstOrDefaultAsync();
        await context.Bairros.FirstOrDefaultAsync();
        await context.TiposLogradouro.FirstOrDefaultAsync();
        await context.Enderecos.FirstOrDefaultAsync();
        
        await context.Grupos.FirstOrDefaultAsync();
        await context.Empresas.FirstOrDefaultAsync();
        await context.Filiais.FirstOrDefaultAsync();
        
        await context.CentrosCusto.FirstOrDefaultAsync();
        await context.Categorias.FirstOrDefaultAsync();
        await context.Classes.FirstOrDefaultAsync();
        
        await context.Entidades.FirstOrDefaultAsync();
        await context.PessoasFisicas.FirstOrDefaultAsync();
        await context.PessoasJuridicas.FirstOrDefaultAsync();
        await context.EntidadeEnderecos.FirstOrDefaultAsync();
        await context.Clientes.FirstOrDefaultAsync();
        await context.Fornecedores.FirstOrDefaultAsync();
        await context.Funcionarios.FirstOrDefaultAsync();
        await context.Transportadoras.FirstOrDefaultAsync();
        
        await context.Perfis.FirstOrDefaultAsync();
        await context.Usuarios.FirstOrDefaultAsync();
        
        await context.Bancos.FirstOrDefaultAsync();
        await context.FormasPagamento.FirstOrDefaultAsync();
        
        await context.SeriesDocumento.FirstOrDefaultAsync();
        await context.SeriesDocumentoFilial.FirstOrDefaultAsync();
        await context.Parametros.FirstOrDefaultAsync();
        await context.ParametroValores.FirstOrDefaultAsync();
    }
}
