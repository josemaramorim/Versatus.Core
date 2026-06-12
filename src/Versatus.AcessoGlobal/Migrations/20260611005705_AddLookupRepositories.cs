using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Versatus.AcessoGlobal.Migrations
{
    /// <inheritdoc />
    public partial class AddLookupRepositories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GloBairro_GloCidade_IdCidade",
                table: "GloBairro");

            migrationBuilder.DropForeignKey(
                name: "FK_GloEstado_GloPais_IdGloPais",
                table: "GloEstado");

            migrationBuilder.DropForeignKey(
                name: "FK_GloSerieDocumentoFilial_GloSerieDocumento_IdSequencialSerieDocto",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropForeignKey(
                name: "FK_GloUsuario_GloPerfil_IdGloPerfil",
                table: "GloUsuario");

            migrationBuilder.DropIndex(
                name: "IX_GloUsuario_IdGloPerfil",
                table: "GloUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GloSerieDocumentoFilial",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropIndex(
                name: "IX_GloSerieDocumentoFilial_IdSequencialSerieDocto",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropIndex(
                name: "IX_GloEstado_IdGloPais",
                table: "GloEstado");

            migrationBuilder.DropColumn(
                name: "DataUltimoLogon",
                table: "GloUsuario");

            migrationBuilder.DropColumn(
                name: "IdGloFuncionario",
                table: "GloUsuario");

            migrationBuilder.DropColumn(
                name: "IdGloPerfil",
                table: "GloUsuario");

            migrationBuilder.DropColumn(
                name: "IdGloSerieDocumentoFilial",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropColumn(
                name: "IdSequencialSerieDocto",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropColumn(
                name: "ProximoNumero",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropColumn(
                name: "ProximoNumero",
                table: "GloSerieDocumento");

            migrationBuilder.DropColumn(
                name: "DataAlteracao",
                table: "GloParametro");

            migrationBuilder.DropColumn(
                name: "DataInclusao",
                table: "GloParametro");

            migrationBuilder.DropColumn(
                name: "HoraAlteracao",
                table: "GloParametro");

            migrationBuilder.DropColumn(
                name: "HoraInclusao",
                table: "GloParametro");

            migrationBuilder.DropColumn(
                name: "IdGloUsuarioAlteracao",
                table: "GloParametro");

            migrationBuilder.DropColumn(
                name: "IdGloUsuarioInclusao",
                table: "GloParametro");

            migrationBuilder.DropColumn(
                name: "IdGloPais",
                table: "GloEstado");

            migrationBuilder.RenameColumn(
                name: "IdTipoLogradouro",
                table: "GloTipoLogradouro",
                newName: "IdGloTipoLogradouro");

            migrationBuilder.RenameColumn(
                name: "IdFormaPagamento",
                table: "GloFormaPagamento",
                newName: "IdGloFormaPagamento");

            migrationBuilder.RenameColumn(
                name: "IdSinteticoAnalitico",
                table: "GloCentroCusto",
                newName: "IdTipo");

            migrationBuilder.RenameColumn(
                name: "IdSinteticoAnalitico",
                table: "GloCategoria",
                newName: "IdAnaliticoSintetico");

            migrationBuilder.RenameColumn(
                name: "IdCidade",
                table: "GloBairro",
                newName: "IdGloCidade");

            migrationBuilder.RenameColumn(
                name: "IdBairro",
                table: "GloBairro",
                newName: "IdGloBairro");

            migrationBuilder.RenameIndex(
                name: "IX_GloBairro_IdCidade",
                table: "GloBairro",
                newName: "IX_GloBairro_IdGloCidade");

            migrationBuilder.RenameColumn(
                name: "IdTipoNatureza",
                table: "FinClasse",
                newName: "IdNatureza");

            migrationBuilder.RenameColumn(
                name: "IdSinteticoAnalitico",
                table: "FinClasse",
                newName: "IdTipo");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Senha",
                table: "GloUsuario",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloUsuario",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloUsuario",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloUsuario",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloUsuario",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloTransportadora",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloTransportadora",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloTransportadora",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloTransportadora",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloTipoLogradouro",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloTipoLogradouro",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloTipoLogradouro",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloTipoLogradouro",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Abreviacao",
                table: "GloTipoLogradouro",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloSerieDocumentoFilial",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloSerieDocumentoFilial",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloSerieDocumentoFilial",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "IdGloSerieDocumento",
                table: "GloSerieDocumentoFilial",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloSerieDocumento",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloSerieDocumento",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloSerieDocumento",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloSerieDocumento",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "UsaDominioFinanceiro",
                table: "GloPerfil",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloPerfil",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloPerfil",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloPerfil",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Administrador",
                table: "GloPerfil",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Objeto",
                table: "GloParametro",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoValor",
                table: "GloParametro",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "GloParametro",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloPais",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloPais",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloPais",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloPais",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloGrupo",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloGrupo",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloGrupo",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloGrupo",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloFuncionario",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloFuncionario",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloFuncionario",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloFuncionario",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloFornecedor",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloFornecedor",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "FornecedorCotacao",
                table: "GloFornecedor",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloFornecedor",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloFornecedor",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloFormaPagamento",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloFormaPagamento",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloFormaPagamento",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloFormaPagamento",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloFilial",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloEstado",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloEstado",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "ExigeRegistroSistema",
                table: "GloEstado",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "ExigeIdentificacaoTecnico",
                table: "GloEstado",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloEstado",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloEstado",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdRegimeTributario",
                table: "GloEntidadeJuridica",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdEnquadramento",
                table: "GloEntidadeJuridica",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CNPJ",
                table: "GloEntidadeJuridica",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);

            migrationBuilder.AlterColumn<int>(
                name: "IdSexo",
                table: "GloEntidadeFisica",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdEstadoCivil",
                table: "GloEntidadeFisica",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<short>(
                name: "FisicaTipoJuridica",
                table: "GloEntidadeFisica",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "GloEntidadeFisica",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<short>(
                name: "Padrao",
                table: "GloEntidadeEndereco",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Numero",
                table: "GloEntidadeEndereco",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloEntidadeEndereco",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Transportadora",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Representante",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Prospecto",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Professor",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Outro",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Obra",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "IntermediadorComercial",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "InstituicaoFinanceira",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Funcionario",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Fornecedor",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Filial",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Contador",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Comissionado",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Cliente",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Aluno",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "AgenciaBancaria",
                table: "GloEntidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Logradouro",
                table: "GloEndereco",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<decimal>(
                name: "LONGITUDE",
                table: "GloEndereco",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "LATITUDE",
                table: "GloEndereco",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloTipoLogradouro",
                table: "GloEndereco",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloCidade",
                table: "GloEndereco",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloBairroInicial",
                table: "GloEndereco",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MsgInicial",
                table: "GloEmpresa",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MsgFinal",
                table: "GloEmpresa",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MascaraProjeto",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MascaraPlanoContabil",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MascaraClasse",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MascaraCentroCusto",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloEmpresa",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloEmpresa",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloEmpresa",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloEmpresa",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorAluguel",
                table: "GloCliente",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "RendaMensal",
                table: "GloCliente",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "LimiteCredito",
                table: "GloCliente",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<short>(
                name: "ItemFinanceiroPadrao",
                table: "GloCliente",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCliente",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCliente",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "EnviarCNDNFe",
                table: "GloCliente",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCliente",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Bloqueado",
                table: "GloCliente",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloCliente",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "GloCidade",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "GloCidade",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCidade",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCidade",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCidade",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloCidade",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCentroCusto",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCentroCusto",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCentroCusto",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloCentroCusto",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCategoria",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCategoria",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCategoria",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloCategoria",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloBanco",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloBanco",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloBanco",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CodigoBancoCobranca",
                table: "GloBanco",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloBanco",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloBairro",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloBairro",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloBairro",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "GloBairro",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "FinClasse",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "FinClasse",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "FinClasse",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<short>(
                name: "Ativo",
                table: "FinClasse",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GloSerieDocumentoFilial",
                table: "GloSerieDocumentoFilial",
                columns: new[] { "IdGloSerieDocumento", "IdGloFilial" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GloSerieDocumento_IdGloSerieDocumento",
                table: "GloSerieDocumento",
                column: "IdGloSerieDocumento");

            migrationBuilder.CreateTable(
                name: "GloParametroValor",
                columns: table => new
                {
                    IDGLOPARAMETROVALOR = table.Column<int>(type: "int", nullable: false),
                    IDGLOPARAMETRO = table.Column<int>(type: "int", nullable: true),
                    IDGLOFILIAL = table.Column<int>(type: "int", nullable: true),
                    IDGLOPERFIL = table.Column<int>(type: "int", nullable: true),
                    VALOR = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IDGLOEMPRESA = table.Column<int>(type: "int", nullable: true),
                    IDGLOGRUPO = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloParametroValor", x => x.IDGLOPARAMETROVALOR);
                    table.ForeignKey(
                        name: "FK_GloParametroValor_GloParametro_IDGLOPARAMETRO",
                        column: x => x.IDGLOPARAMETRO,
                        principalTable: "GloParametro",
                        principalColumn: "IdGloParametro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloPerfilUsuario",
                columns: table => new
                {
                    IdGloPerfil = table.Column<int>(type: "int", nullable: false),
                    IdGloUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloPerfilUsuario", x => new { x.IdGloPerfil, x.IdGloUsuario });
                    table.ForeignKey(
                        name: "FK_GloPerfilUsuario_GloPerfil_IdGloPerfil",
                        column: x => x.IdGloPerfil,
                        principalTable: "GloPerfil",
                        principalColumn: "IdGloPerfil",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloPerfilUsuario_GloUsuario_IdGloUsuario",
                        column: x => x.IdGloUsuario,
                        principalTable: "GloUsuario",
                        principalColumn: "IdGloUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GloParametroValor_IDGLOPARAMETRO",
                table: "GloParametroValor",
                column: "IDGLOPARAMETRO");

            migrationBuilder.CreateIndex(
                name: "IX_GloPerfilUsuario_IdGloUsuario",
                table: "GloPerfilUsuario",
                column: "IdGloUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_GloBairro_GloCidade_IdGloCidade",
                table: "GloBairro",
                column: "IdGloCidade",
                principalTable: "GloCidade",
                principalColumn: "IdGloCidade",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GloSerieDocumentoFilial_GloSerieDocumento_IdGloSerieDocumento",
                table: "GloSerieDocumentoFilial",
                column: "IdGloSerieDocumento",
                principalTable: "GloSerieDocumento",
                principalColumn: "IdGloSerieDocumento",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GloBairro_GloCidade_IdGloCidade",
                table: "GloBairro");

            migrationBuilder.DropForeignKey(
                name: "FK_GloSerieDocumentoFilial_GloSerieDocumento_IdGloSerieDocumento",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropTable(
                name: "GloParametroValor");

            migrationBuilder.DropTable(
                name: "GloPerfilUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GloSerieDocumentoFilial",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GloSerieDocumento_IdGloSerieDocumento",
                table: "GloSerieDocumento");

            migrationBuilder.DropColumn(
                name: "IdGloSerieDocumento",
                table: "GloSerieDocumentoFilial");

            migrationBuilder.RenameColumn(
                name: "IdGloTipoLogradouro",
                table: "GloTipoLogradouro",
                newName: "IdTipoLogradouro");

            migrationBuilder.RenameColumn(
                name: "IdGloFormaPagamento",
                table: "GloFormaPagamento",
                newName: "IdFormaPagamento");

            migrationBuilder.RenameColumn(
                name: "IdTipo",
                table: "GloCentroCusto",
                newName: "IdSinteticoAnalitico");

            migrationBuilder.RenameColumn(
                name: "IdAnaliticoSintetico",
                table: "GloCategoria",
                newName: "IdSinteticoAnalitico");

            migrationBuilder.RenameColumn(
                name: "IdGloCidade",
                table: "GloBairro",
                newName: "IdCidade");

            migrationBuilder.RenameColumn(
                name: "IdGloBairro",
                table: "GloBairro",
                newName: "IdBairro");

            migrationBuilder.RenameIndex(
                name: "IX_GloBairro_IdGloCidade",
                table: "GloBairro",
                newName: "IX_GloBairro_IdCidade");

            migrationBuilder.RenameColumn(
                name: "IdTipo",
                table: "FinClasse",
                newName: "IdSinteticoAnalitico");

            migrationBuilder.RenameColumn(
                name: "IdNatureza",
                table: "FinClasse",
                newName: "IdTipoNatureza");

            migrationBuilder.AlterColumn<string>(
                name: "Senha",
                table: "GloUsuario",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloUsuario",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloUsuario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloUsuario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloUsuario",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimoLogon",
                table: "GloUsuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdGloFuncionario",
                table: "GloUsuario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdGloPerfil",
                table: "GloUsuario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloTransportadora",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloTransportadora",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloTransportadora",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloTransportadora",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloTipoLogradouro",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloTipoLogradouro",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloTipoLogradouro",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloTipoLogradouro",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "Abreviacao",
                table: "GloTipoLogradouro",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloSerieDocumentoFilial",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloSerieDocumentoFilial",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloSerieDocumentoFilial",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdGloSerieDocumentoFilial",
                table: "GloSerieDocumentoFilial",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "IdSequencialSerieDocto",
                table: "GloSerieDocumentoFilial",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProximoNumero",
                table: "GloSerieDocumentoFilial",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloSerieDocumento",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloSerieDocumento",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloSerieDocumento",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloSerieDocumento",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<int>(
                name: "ProximoNumero",
                table: "GloSerieDocumento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "UsaDominioFinanceiro",
                table: "GloPerfil",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloPerfil",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloPerfil",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloPerfil",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Administrador",
                table: "GloPerfil",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "Objeto",
                table: "GloParametro",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdTipoValor",
                table: "GloParametro",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "GloParametro",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAlteracao",
                table: "GloParametro",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInclusao",
                table: "GloParametro",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraAlteracao",
                table: "GloParametro",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloParametro",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdGloUsuarioAlteracao",
                table: "GloParametro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloParametro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloPais",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloPais",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloPais",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloPais",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloGrupo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloGrupo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloGrupo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloGrupo",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloFuncionario",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloFuncionario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloFuncionario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloFuncionario",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloFornecedor",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloFornecedor",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FornecedorCotacao",
                table: "GloFornecedor",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloFornecedor",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloFornecedor",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloFormaPagamento",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloFormaPagamento",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloFormaPagamento",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloFormaPagamento",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloFilial",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloEstado",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloEstado",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ExigeRegistroSistema",
                table: "GloEstado",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "ExigeIdentificacaoTecnico",
                table: "GloEstado",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloEstado",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloEstado",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddColumn<int>(
                name: "IdGloPais",
                table: "GloEstado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "IdRegimeTributario",
                table: "GloEntidadeJuridica",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdEnquadramento",
                table: "GloEntidadeJuridica",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CNPJ",
                table: "GloEntidadeJuridica",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdSexo",
                table: "GloEntidadeFisica",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdEstadoCivil",
                table: "GloEntidadeFisica",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "FisicaTipoJuridica",
                table: "GloEntidadeFisica",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "GloEntidadeFisica",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Padrao",
                table: "GloEntidadeEndereco",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "Numero",
                table: "GloEntidadeEndereco",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloEntidadeEndereco",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Transportadora",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Representante",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Prospecto",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Professor",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Outro",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Obra",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "IntermediadorComercial",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "InstituicaoFinanceira",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Funcionario",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Fornecedor",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Filial",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Contador",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Comissionado",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Cliente",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "Aluno",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "AgenciaBancaria",
                table: "GloEntidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "Logradouro",
                table: "GloEndereco",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LONGITUDE",
                table: "GloEndereco",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LATITUDE",
                table: "GloEndereco",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloTipoLogradouro",
                table: "GloEndereco",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloCidade",
                table: "GloEndereco",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloBairroInicial",
                table: "GloEndereco",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MsgInicial",
                table: "GloEmpresa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MsgFinal",
                table: "GloEmpresa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MascaraProjeto",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MascaraPlanoContabil",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MascaraClasse",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MascaraCentroCusto",
                table: "GloEmpresa",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloEmpresa",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloEmpresa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloEmpresa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloEmpresa",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<double>(
                name: "ValorAluguel",
                table: "GloCliente",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RendaMensal",
                table: "GloCliente",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LimiteCredito",
                table: "GloCliente",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ItemFinanceiroPadrao",
                table: "GloCliente",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCliente",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCliente",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EnviarCNDNFe",
                table: "GloCliente",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCliente",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Bloqueado",
                table: "GloCliente",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloCliente",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "GloCidade",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "GloCidade",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCidade",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCidade",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCidade",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloCidade",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCentroCusto",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCentroCusto",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCentroCusto",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloCentroCusto",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloCategoria",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloCategoria",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloCategoria",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloCategoria",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloBanco",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloBanco",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloBanco",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CodigoBancoCobranca",
                table: "GloBanco",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloBanco",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "GloBairro",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "GloBairro",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "GloBairro",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "GloBairro",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "IdGloUsuarioInclusao",
                table: "FinClasse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInclusao",
                table: "FinClasse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInclusao",
                table: "FinClasse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ativo",
                table: "FinClasse",
                type: "bit",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GloSerieDocumentoFilial",
                table: "GloSerieDocumentoFilial",
                column: "IdGloSerieDocumentoFilial");

            migrationBuilder.CreateIndex(
                name: "IX_GloUsuario_IdGloPerfil",
                table: "GloUsuario",
                column: "IdGloPerfil");

            migrationBuilder.CreateIndex(
                name: "IX_GloSerieDocumentoFilial_IdSequencialSerieDocto",
                table: "GloSerieDocumentoFilial",
                column: "IdSequencialSerieDocto");

            migrationBuilder.CreateIndex(
                name: "IX_GloEstado_IdGloPais",
                table: "GloEstado",
                column: "IdGloPais");

            migrationBuilder.AddForeignKey(
                name: "FK_GloBairro_GloCidade_IdCidade",
                table: "GloBairro",
                column: "IdCidade",
                principalTable: "GloCidade",
                principalColumn: "IdGloCidade",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GloEstado_GloPais_IdGloPais",
                table: "GloEstado",
                column: "IdGloPais",
                principalTable: "GloPais",
                principalColumn: "IdGloPais",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GloSerieDocumentoFilial_GloSerieDocumento_IdSequencialSerieDocto",
                table: "GloSerieDocumentoFilial",
                column: "IdSequencialSerieDocto",
                principalTable: "GloSerieDocumento",
                principalColumn: "IdSequencialSerieDocto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GloUsuario_GloPerfil_IdGloPerfil",
                table: "GloUsuario",
                column: "IdGloPerfil",
                principalTable: "GloPerfil",
                principalColumn: "IdGloPerfil",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
