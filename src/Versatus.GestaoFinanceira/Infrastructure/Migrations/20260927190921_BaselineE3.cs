using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Versatus.GestaoFinanceira.Infrastructure.Migrations
{
    /// <summary>
    /// E3-T08 — baseline <b>vazia</b> do GestaoFinanceiraDbContext (decisão do usuário em 2026-09-27).
    /// As tabelas FIN* do épico E3 já existem no banco legado (schema-first): esta migration não cria
    /// nem altera nada — só fixa o modelo do E3 no snapshot como ponto de partida. As próximas
    /// migrations mostram apenas o que o modelo mudar a partir daqui.
    /// Modelo conferido contra INFORMATION_SCHEMA em 2026-09-27: 88 colunas mapeadas em
    /// FINCAIXABANCO, FINCAIXABANCOUSUARIO, FINCONTABANCARIA, FINSALDOCAIXABANCO, FINSALDORATEIO e
    /// FINCOBRADOR, sem divergência de nome, tipo ou nulidade; as 27 colunas fora do modelo são as
    /// [E14] de FINCONTABANCARIA, todas anuláveis.
    /// Histórico: __EFMigrationsHistory_GestaoFinanceira (não registrada no banco nesta tarefa).
    /// </summary>
    public partial class BaselineE3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intencionalmente vazio — tabelas do legado já existem (schema-first).
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intencionalmente vazio — nunca remover tabelas do legado.
        }
    }
}
