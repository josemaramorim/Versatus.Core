using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Versatus.AcessoGlobal.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GloBanco",
                columns: table => new
                {
                    IdGloBanco = table.Column<int>(type: "int", nullable: false),
                    CodigoBancoCobranca = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloBanco", x => x.IdGloBanco);
                });

            migrationBuilder.CreateTable(
                name: "GloCategoria",
                columns: table => new
                {
                    IdGloCategoria = table.Column<int>(type: "int", nullable: false),
                    IdGloCategoriaPai = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdSinteticoAnalitico = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloCategoria", x => x.IdGloCategoria);
                    table.ForeignKey(
                        name: "FK_GloCategoria_GloCategoria_IdGloCategoriaPai",
                        column: x => x.IdGloCategoriaPai,
                        principalTable: "GloCategoria",
                        principalColumn: "IdGloCategoria",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloEntidade",
                columns: table => new
                {
                    IdGloEntidade = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailNFE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailFinanceiro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailVenda = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailCompra = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HomePage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InscricaoEstadual = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InscricaoMunicipal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InscricaoSuframa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Cliente = table.Column<bool>(type: "bit", nullable: false),
                    Fornecedor = table.Column<bool>(type: "bit", nullable: false),
                    Transportadora = table.Column<bool>(type: "bit", nullable: false),
                    Comissionado = table.Column<bool>(type: "bit", nullable: false),
                    AgenciaBancaria = table.Column<bool>(type: "bit", nullable: false),
                    InstituicaoFinanceira = table.Column<bool>(type: "bit", nullable: false),
                    Filial = table.Column<bool>(type: "bit", nullable: false),
                    Funcionario = table.Column<bool>(type: "bit", nullable: false),
                    Obra = table.Column<bool>(type: "bit", nullable: false),
                    Representante = table.Column<bool>(type: "bit", nullable: false),
                    Outro = table.Column<bool>(type: "bit", nullable: false),
                    Prospecto = table.Column<bool>(type: "bit", nullable: false),
                    Contador = table.Column<bool>(type: "bit", nullable: false),
                    Aluno = table.Column<bool>(type: "bit", nullable: false),
                    Professor = table.Column<bool>(type: "bit", nullable: false),
                    IntermediadorComercial = table.Column<bool>(type: "bit", nullable: false),
                    IdFisicaJuridica = table.Column<int>(type: "int", nullable: false),
                    IDINDICADORCONTRIBUINTEICMS = table.Column<int>(type: "int", nullable: false),
                    IdStatusCnpjCpf = table.Column<int>(type: "int", nullable: false),
                    IDTIPOPLATAFORMA = table.Column<int>(type: "int", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloEntidade", x => x.IdGloEntidade);
                });

            migrationBuilder.CreateTable(
                name: "GloFormaPagamento",
                columns: table => new
                {
                    IdFormaPagamento = table.Column<int>(type: "int", nullable: false),
                    SiglaForma = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdTipoFormaPagamento = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloFormaPagamento", x => x.IdFormaPagamento);
                });

            migrationBuilder.CreateTable(
                name: "GloGrupo",
                columns: table => new
                {
                    IdGloGrupo = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloGrupo", x => x.IdGloGrupo);
                });

            migrationBuilder.CreateTable(
                name: "GloPais",
                columns: table => new
                {
                    IdGloPais = table.Column<int>(type: "int", nullable: false),
                    CodigoBACEN = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Abreviacao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloPais", x => x.IdGloPais);
                });

            migrationBuilder.CreateTable(
                name: "GloParametro",
                columns: table => new
                {
                    IdGloParametro = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Objeto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IdTipoValor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloParametro", x => x.IdGloParametro);
                });

            migrationBuilder.CreateTable(
                name: "GloPerfil",
                columns: table => new
                {
                    IdGloPerfil = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Administrador = table.Column<bool>(type: "bit", nullable: false),
                    UsaDominioFinanceiro = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloPerfil", x => x.IdGloPerfil);
                });

            migrationBuilder.CreateTable(
                name: "GloSerieDocumento",
                columns: table => new
                {
                    IdSequencialSerieDocto = table.Column<int>(type: "int", nullable: false),
                    IdGloSerieDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModeloFiscal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ProximoNumero = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloSerieDocumento", x => x.IdSequencialSerieDocto);
                });

            migrationBuilder.CreateTable(
                name: "GloTipoLogradouro",
                columns: table => new
                {
                    IdTipoLogradouro = table.Column<int>(type: "int", nullable: false),
                    Abreviacao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloTipoLogradouro", x => x.IdTipoLogradouro);
                });

            migrationBuilder.CreateTable(
                name: "GloCliente",
                columns: table => new
                {
                    IdGloCliente = table.Column<int>(type: "int", nullable: false),
                    LocalTrabalho = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelefoneTrabalho = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Profissao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InscricaoProdutor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CodigoAlternativo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Bloqueado = table.Column<bool>(type: "bit", nullable: false),
                    ItemFinanceiroPadrao = table.Column<bool>(type: "bit", nullable: false),
                    EnviarCNDNFe = table.Column<bool>(type: "bit", nullable: false),
                    RendaMensal = table.Column<double>(type: "float", nullable: false),
                    LimiteCredito = table.Column<double>(type: "float", nullable: false),
                    ValorAluguel = table.Column<double>(type: "float", nullable: false),
                    DataAdmissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraCobranca = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdImovel = table.Column<int>(type: "int", nullable: false),
                    SituacaoClienteSPC = table.Column<int>(type: "int", nullable: false),
                    IdGloCategoria = table.Column<int>(type: "int", nullable: true),
                    IdGloClienteConceito = table.Column<int>(type: "int", nullable: true),
                    IdDiaSemanaCobranca = table.Column<int>(type: "int", nullable: true),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloCliente", x => x.IdGloCliente);
                    table.ForeignKey(
                        name: "FK_GloCliente_GloCategoria_IdGloCategoria",
                        column: x => x.IdGloCategoria,
                        principalTable: "GloCategoria",
                        principalColumn: "IdGloCategoria");
                    table.ForeignKey(
                        name: "FK_GloCliente_GloEntidade_IdGloCliente",
                        column: x => x.IdGloCliente,
                        principalTable: "GloEntidade",
                        principalColumn: "IdGloEntidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloEntidadeFisica",
                columns: table => new
                {
                    IdGloEntidade = table.Column<int>(type: "int", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Rg = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OrgaoEmissorRg = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DataEmissaoRg = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdSexo = table.Column<int>(type: "int", nullable: false),
                    IdEstadoCivil = table.Column<int>(type: "int", nullable: false),
                    FisicaTipoJuridica = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloEntidadeFisica", x => x.IdGloEntidade);
                    table.ForeignKey(
                        name: "FK_GloEntidadeFisica_GloEntidade_IdGloEntidade",
                        column: x => x.IdGloEntidade,
                        principalTable: "GloEntidade",
                        principalColumn: "IdGloEntidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloEntidadeJuridica",
                columns: table => new
                {
                    IdGloEntidade = table.Column<int>(type: "int", nullable: false),
                    CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdRegimeTributario = table.Column<int>(type: "int", nullable: false),
                    IdEnquadramento = table.Column<int>(type: "int", nullable: false),
                    IdGloCnaePrincipal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloEntidadeJuridica", x => x.IdGloEntidade);
                    table.ForeignKey(
                        name: "FK_GloEntidadeJuridica_GloEntidade_IdGloEntidade",
                        column: x => x.IdGloEntidade,
                        principalTable: "GloEntidade",
                        principalColumn: "IdGloEntidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloFornecedor",
                columns: table => new
                {
                    IdGloFornecedor = table.Column<int>(type: "int", nullable: false),
                    CodigoAlternativo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContaContabil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    FornecedorCotacao = table.Column<bool>(type: "bit", nullable: false),
                    IdGloCategoria = table.Column<int>(type: "int", nullable: true),
                    IdGloCondicaoPagamento = table.Column<int>(type: "int", nullable: true),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloFornecedor", x => x.IdGloFornecedor);
                    table.ForeignKey(
                        name: "FK_GloFornecedor_GloCategoria_IdGloCategoria",
                        column: x => x.IdGloCategoria,
                        principalTable: "GloCategoria",
                        principalColumn: "IdGloCategoria");
                    table.ForeignKey(
                        name: "FK_GloFornecedor_GloEntidade_IdGloFornecedor",
                        column: x => x.IdGloFornecedor,
                        principalTable: "GloEntidade",
                        principalColumn: "IdGloEntidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloFuncionario",
                columns: table => new
                {
                    IdGloFuncionario = table.Column<int>(type: "int", nullable: false),
                    Ctps = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SerieCtps = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UfCtps = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    DataEmissaoCtps = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroCnh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CategoriaCnh = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    DataVencimentoCnh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InscricaoPis = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IdGloBancoPis = table.Column<int>(type: "int", nullable: true),
                    NumeroAgenciaPis = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NomeAgenciaPis = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataInscricaoPis = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NomePai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NomeMae = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdRhRaca = table.Column<int>(type: "int", nullable: true),
                    IdRhTipoDeficiencia = table.Column<int>(type: "int", nullable: true),
                    IdGloPais = table.Column<int>(type: "int", nullable: true),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloFuncionario", x => x.IdGloFuncionario);
                    table.ForeignKey(
                        name: "FK_GloFuncionario_GloEntidade_IdGloFuncionario",
                        column: x => x.IdGloFuncionario,
                        principalTable: "GloEntidade",
                        principalColumn: "IdGloEntidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloTransportadora",
                columns: table => new
                {
                    IdGloTransportadora = table.Column<int>(type: "int", nullable: false),
                    Rntrc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdTipoProprietario = table.Column<int>(type: "int", nullable: false),
                    IdTipoTransportador = table.Column<int>(type: "int", nullable: false),
                    IdGloCategoria = table.Column<int>(type: "int", nullable: true),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloTransportadora", x => x.IdGloTransportadora);
                    table.ForeignKey(
                        name: "FK_GloTransportadora_GloCategoria_IdGloCategoria",
                        column: x => x.IdGloCategoria,
                        principalTable: "GloCategoria",
                        principalColumn: "IdGloCategoria");
                    table.ForeignKey(
                        name: "FK_GloTransportadora_GloEntidade_IdGloTransportadora",
                        column: x => x.IdGloTransportadora,
                        principalTable: "GloEntidade",
                        principalColumn: "IdGloEntidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GloEmpresa",
                columns: table => new
                {
                    IdGloEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdGloGrupo = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IdTributacaoEspecial = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    MascaraClasse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MascaraCentroCusto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MascaraProjeto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MascaraPlanoContabil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MsgInicial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MsgFinal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloEmpresa", x => x.IdGloEmpresa);
                    table.ForeignKey(
                        name: "FK_GloEmpresa_GloGrupo_IdGloGrupo",
                        column: x => x.IdGloGrupo,
                        principalTable: "GloGrupo",
                        principalColumn: "IdGloGrupo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloEstado",
                columns: table => new
                {
                    IdSequencialEstado = table.Column<int>(type: "int", nullable: false),
                    IdGloPais = table.Column<int>(type: "int", nullable: false),
                    Uf = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoIBGE = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    ExigeIdentificacaoTecnico = table.Column<bool>(type: "bit", nullable: false),
                    ExigeRegistroSistema = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloEstado", x => x.IdSequencialEstado);
                    table.UniqueConstraint("AK_GloEstado_Uf", x => x.Uf);
                    table.ForeignKey(
                        name: "FK_GloEstado_GloPais_IdGloPais",
                        column: x => x.IdGloPais,
                        principalTable: "GloPais",
                        principalColumn: "IdGloPais",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloUsuario",
                columns: table => new
                {
                    IdGloUsuario = table.Column<int>(type: "int", nullable: false),
                    IdGloFuncionario = table.Column<int>(type: "int", nullable: true),
                    NomeAcesso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdGloPerfil = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataUltimoLogon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloUsuario", x => x.IdGloUsuario);
                    table.ForeignKey(
                        name: "FK_GloUsuario_GloPerfil_IdGloPerfil",
                        column: x => x.IdGloPerfil,
                        principalTable: "GloPerfil",
                        principalColumn: "IdGloPerfil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloFilial",
                columns: table => new
                {
                    IdGloFilial = table.Column<int>(type: "int", nullable: false),
                    IdGloEmpresa = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Logomarca = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LogomarcaMedia = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LogomarcaGrande = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloFilial", x => x.IdGloFilial);
                    table.ForeignKey(
                        name: "FK_GloFilial_GloEmpresa_IdGloEmpresa",
                        column: x => x.IdGloEmpresa,
                        principalTable: "GloEmpresa",
                        principalColumn: "IdGloEmpresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloCidade",
                columns: table => new
                {
                    IdGloCidade = table.Column<int>(type: "int", nullable: false),
                    Uf = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IdGloPais = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    CodigoIBGE = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    CodigoCidade = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloCidade", x => x.IdGloCidade);
                    table.ForeignKey(
                        name: "FK_GloCidade_GloEstado_Uf",
                        column: x => x.Uf,
                        principalTable: "GloEstado",
                        principalColumn: "Uf",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloCidade_GloPais_IdGloPais",
                        column: x => x.IdGloPais,
                        principalTable: "GloPais",
                        principalColumn: "IdGloPais",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinClasse",
                columns: table => new
                {
                    IdFinClasse = table.Column<int>(type: "int", nullable: false),
                    IdGloFilial = table.Column<int>(type: "int", nullable: false),
                    IdFinClassePai = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Extenso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    IdTipoNatureza = table.Column<int>(type: "int", nullable: false),
                    IdSinteticoAnalitico = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinClasse", x => x.IdFinClasse);
                    table.ForeignKey(
                        name: "FK_FinClasse_FinClasse_IdFinClassePai",
                        column: x => x.IdFinClassePai,
                        principalTable: "FinClasse",
                        principalColumn: "IdFinClasse",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinClasse_GloFilial_IdGloFilial",
                        column: x => x.IdGloFilial,
                        principalTable: "GloFilial",
                        principalColumn: "IdGloFilial",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloCentroCusto",
                columns: table => new
                {
                    IdGloCentroCusto = table.Column<int>(type: "int", nullable: false),
                    IdGloFilial = table.Column<int>(type: "int", nullable: false),
                    IdGloCentroCustoPai = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Extenso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    IdSinteticoAnalitico = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloCentroCusto", x => x.IdGloCentroCusto);
                    table.ForeignKey(
                        name: "FK_GloCentroCusto_GloCentroCusto_IdGloCentroCustoPai",
                        column: x => x.IdGloCentroCustoPai,
                        principalTable: "GloCentroCusto",
                        principalColumn: "IdGloCentroCusto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloCentroCusto_GloFilial_IdGloFilial",
                        column: x => x.IdGloFilial,
                        principalTable: "GloFilial",
                        principalColumn: "IdGloFilial",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloSerieDocumentoFilial",
                columns: table => new
                {
                    IdGloSerieDocumentoFilial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSequencialSerieDocto = table.Column<int>(type: "int", nullable: false),
                    IdGloFilial = table.Column<int>(type: "int", nullable: false),
                    ProximoNumero = table.Column<int>(type: "int", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloSerieDocumentoFilial", x => x.IdGloSerieDocumentoFilial);
                    table.ForeignKey(
                        name: "FK_GloSerieDocumentoFilial_GloFilial_IdGloFilial",
                        column: x => x.IdGloFilial,
                        principalTable: "GloFilial",
                        principalColumn: "IdGloFilial",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloSerieDocumentoFilial_GloSerieDocumento_IdSequencialSerieDocto",
                        column: x => x.IdSequencialSerieDocto,
                        principalTable: "GloSerieDocumento",
                        principalColumn: "IdSequencialSerieDocto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloBairro",
                columns: table => new
                {
                    IdBairro = table.Column<int>(type: "int", nullable: false),
                    IdCidade = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    IdGloUsuarioInclusao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdGloUsuarioAlteracao = table.Column<int>(type: "int", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloBairro", x => x.IdBairro);
                    table.ForeignKey(
                        name: "FK_GloBairro_GloCidade_IdCidade",
                        column: x => x.IdCidade,
                        principalTable: "GloCidade",
                        principalColumn: "IdGloCidade",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloEndereco",
                columns: table => new
                {
                    IdGloEndereco = table.Column<int>(type: "int", nullable: false),
                    IdGloCidade = table.Column<int>(type: "int", nullable: false),
                    IdGloBairroInicial = table.Column<int>(type: "int", nullable: false),
                    IdGloBairroFinal = table.Column<int>(type: "int", nullable: true),
                    IdGloTipoLogradouro = table.Column<int>(type: "int", nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LATITUDE = table.Column<double>(type: "float", nullable: false),
                    LONGITUDE = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloEndereco", x => x.IdGloEndereco);
                    table.ForeignKey(
                        name: "FK_GloEndereco_GloBairro_IdGloBairroFinal",
                        column: x => x.IdGloBairroFinal,
                        principalTable: "GloBairro",
                        principalColumn: "IdBairro",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloEndereco_GloBairro_IdGloBairroInicial",
                        column: x => x.IdGloBairroInicial,
                        principalTable: "GloBairro",
                        principalColumn: "IdBairro",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloEndereco_GloCidade_IdGloCidade",
                        column: x => x.IdGloCidade,
                        principalTable: "GloCidade",
                        principalColumn: "IdGloCidade",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloEndereco_GloTipoLogradouro_IdGloTipoLogradouro",
                        column: x => x.IdGloTipoLogradouro,
                        principalTable: "GloTipoLogradouro",
                        principalColumn: "IdTipoLogradouro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GloEntidadeEndereco",
                columns: table => new
                {
                    IdGloEntidadeEndereco = table.Column<int>(type: "int", nullable: false),
                    IdGloEntidade = table.Column<int>(type: "int", nullable: false),
                    IdGloCidade = table.Column<int>(type: "int", nullable: false),
                    IdGloTipoLogradouro = table.Column<int>(type: "int", nullable: false),
                    IdGloBairro = table.Column<int>(type: "int", nullable: true),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Cep = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    CaixaPostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Padrao = table.Column<bool>(type: "bit", nullable: false),
                    IdTipoEndereco = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GloEntidadeEndereco", x => x.IdGloEntidadeEndereco);
                    table.ForeignKey(
                        name: "FK_GloEntidadeEndereco_GloBairro_IdGloBairro",
                        column: x => x.IdGloBairro,
                        principalTable: "GloBairro",
                        principalColumn: "IdBairro",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloEntidadeEndereco_GloCidade_IdGloCidade",
                        column: x => x.IdGloCidade,
                        principalTable: "GloCidade",
                        principalColumn: "IdGloCidade",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GloEntidadeEndereco_GloEntidade_IdGloEntidade",
                        column: x => x.IdGloEntidade,
                        principalTable: "GloEntidade",
                        principalColumn: "IdGloEntidade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GloEntidadeEndereco_GloTipoLogradouro_IdGloTipoLogradouro",
                        column: x => x.IdGloTipoLogradouro,
                        principalTable: "GloTipoLogradouro",
                        principalColumn: "IdTipoLogradouro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinClasse_IdFinClassePai",
                table: "FinClasse",
                column: "IdFinClassePai");

            migrationBuilder.CreateIndex(
                name: "IX_FinClasse_IdGloFilial",
                table: "FinClasse",
                column: "IdGloFilial");

            migrationBuilder.CreateIndex(
                name: "IX_GloBairro_IdCidade",
                table: "GloBairro",
                column: "IdCidade");

            migrationBuilder.CreateIndex(
                name: "IX_GloCategoria_IdGloCategoriaPai",
                table: "GloCategoria",
                column: "IdGloCategoriaPai");

            migrationBuilder.CreateIndex(
                name: "IX_GloCentroCusto_IdGloCentroCustoPai",
                table: "GloCentroCusto",
                column: "IdGloCentroCustoPai");

            migrationBuilder.CreateIndex(
                name: "IX_GloCentroCusto_IdGloFilial",
                table: "GloCentroCusto",
                column: "IdGloFilial");

            migrationBuilder.CreateIndex(
                name: "IX_GloCidade_IdGloPais",
                table: "GloCidade",
                column: "IdGloPais");

            migrationBuilder.CreateIndex(
                name: "IX_GloCidade_Uf",
                table: "GloCidade",
                column: "Uf");

            migrationBuilder.CreateIndex(
                name: "IX_GloCliente_IdGloCategoria",
                table: "GloCliente",
                column: "IdGloCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_GloEmpresa_IdGloGrupo",
                table: "GloEmpresa",
                column: "IdGloGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_GloEndereco_IdGloBairroFinal",
                table: "GloEndereco",
                column: "IdGloBairroFinal");

            migrationBuilder.CreateIndex(
                name: "IX_GloEndereco_IdGloBairroInicial",
                table: "GloEndereco",
                column: "IdGloBairroInicial");

            migrationBuilder.CreateIndex(
                name: "IX_GloEndereco_IdGloCidade",
                table: "GloEndereco",
                column: "IdGloCidade");

            migrationBuilder.CreateIndex(
                name: "IX_GloEndereco_IdGloTipoLogradouro",
                table: "GloEndereco",
                column: "IdGloTipoLogradouro");

            migrationBuilder.CreateIndex(
                name: "IX_GloEntidadeEndereco_IdGloBairro",
                table: "GloEntidadeEndereco",
                column: "IdGloBairro");

            migrationBuilder.CreateIndex(
                name: "IX_GloEntidadeEndereco_IdGloCidade",
                table: "GloEntidadeEndereco",
                column: "IdGloCidade");

            migrationBuilder.CreateIndex(
                name: "IX_GloEntidadeEndereco_IdGloEntidade",
                table: "GloEntidadeEndereco",
                column: "IdGloEntidade");

            migrationBuilder.CreateIndex(
                name: "IX_GloEntidadeEndereco_IdGloTipoLogradouro",
                table: "GloEntidadeEndereco",
                column: "IdGloTipoLogradouro");

            migrationBuilder.CreateIndex(
                name: "IX_GloEstado_IdGloPais",
                table: "GloEstado",
                column: "IdGloPais");

            migrationBuilder.CreateIndex(
                name: "IX_GloFilial_IdGloEmpresa",
                table: "GloFilial",
                column: "IdGloEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_GloFornecedor_IdGloCategoria",
                table: "GloFornecedor",
                column: "IdGloCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_GloSerieDocumentoFilial_IdGloFilial",
                table: "GloSerieDocumentoFilial",
                column: "IdGloFilial");

            migrationBuilder.CreateIndex(
                name: "IX_GloSerieDocumentoFilial_IdSequencialSerieDocto",
                table: "GloSerieDocumentoFilial",
                column: "IdSequencialSerieDocto");

            migrationBuilder.CreateIndex(
                name: "IX_GloTransportadora_IdGloCategoria",
                table: "GloTransportadora",
                column: "IdGloCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_GloUsuario_IdGloPerfil",
                table: "GloUsuario",
                column: "IdGloPerfil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinClasse");

            migrationBuilder.DropTable(
                name: "GloBanco");

            migrationBuilder.DropTable(
                name: "GloCentroCusto");

            migrationBuilder.DropTable(
                name: "GloCliente");

            migrationBuilder.DropTable(
                name: "GloEndereco");

            migrationBuilder.DropTable(
                name: "GloEntidadeEndereco");

            migrationBuilder.DropTable(
                name: "GloEntidadeFisica");

            migrationBuilder.DropTable(
                name: "GloEntidadeJuridica");

            migrationBuilder.DropTable(
                name: "GloFormaPagamento");

            migrationBuilder.DropTable(
                name: "GloFornecedor");

            migrationBuilder.DropTable(
                name: "GloFuncionario");

            migrationBuilder.DropTable(
                name: "GloParametro");

            migrationBuilder.DropTable(
                name: "GloSerieDocumentoFilial");

            migrationBuilder.DropTable(
                name: "GloTransportadora");

            migrationBuilder.DropTable(
                name: "GloUsuario");

            migrationBuilder.DropTable(
                name: "GloBairro");

            migrationBuilder.DropTable(
                name: "GloTipoLogradouro");

            migrationBuilder.DropTable(
                name: "GloFilial");

            migrationBuilder.DropTable(
                name: "GloSerieDocumento");

            migrationBuilder.DropTable(
                name: "GloCategoria");

            migrationBuilder.DropTable(
                name: "GloEntidade");

            migrationBuilder.DropTable(
                name: "GloPerfil");

            migrationBuilder.DropTable(
                name: "GloCidade");

            migrationBuilder.DropTable(
                name: "GloEmpresa");

            migrationBuilder.DropTable(
                name: "GloEstado");

            migrationBuilder.DropTable(
                name: "GloGrupo");

            migrationBuilder.DropTable(
                name: "GloPais");
        }
    }
}
