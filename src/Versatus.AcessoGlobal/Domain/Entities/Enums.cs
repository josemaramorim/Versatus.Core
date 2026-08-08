namespace Versatus.AcessoGlobal.Domain.Entities;

public enum EntidadeTipoPessoa
{
    Fisica = 1,
    Juridica = 2
}

public enum IndicadorContribuinteICMS
{
    Contribuinte = 1,
    ContribuinteIsento = 2,
    NaoContribuinte = 9
}

public enum StatusCnpjCpf
{
    Regular = 1,
    PendenteRegularizacao = 2,
    Suspensa = 3,
    Cancelada = 4,
    Nula = 5
}

public enum SexoTipo
{
    Masculino = 1,
    Feminino = 2,
    Outro = 3
}

public enum EstadoCivilTipo
{
    Solteiro = 1,
    Casado = 2,
    Divorciado = 3,
    Viuvo = 4,
    Separado = 5,
    UniaoEstavel = 6
}

public enum RegimeTributarioTipo
{
    SimplesNacional = 1,
    SimplesNacionalExcesso = 2,
    RegimeNormal = 3
}

public enum EnquadramentoTipo
{
    MEI = 1,
    ME = 2,
    EPP = 3,
    Demais = 4
}

public enum SituacaoClienteSPC
{
    Normal = 892,
    Restricao = 893,
    Negativado = 894
}

public enum TipoImovel
{
    Proprio = 544,
    Aluguel = 545,
    Outros = 546
}

public enum TipoProprietario
{
    TACAgregado = 1645,
    TACIndependente = 1646,
    Outros = 1647
}

public enum TipoTransportador
{
    Nenhum = 1655
}

public enum FormaPagtoTipo
{
    Dinheiro = 122,
    ChequeEmpresa = 123,
    ChequeCliente = 124,
    CartaoCredito = 125,
    CartaoDebito = 126,
    ParcelamentoProprio = 127,
    ParcelamentoFinanceira = 128,
    Credito = 236,
    CreditoPortador = 237,
    Deposito = 256,
    Outros = 293,
    Abatimento = 483,
    PixEstatico = 1962,
    PixDinamico = 1963
}

public enum ParametroValorTipo
{
    Int = 153,
    Numeric = 154,
    String = 155,
    Smallint = 156,
    DateTime = 157,
    Lookup = 233,
    Enumerado = 234,
    Automatico = 374,
    LookupMulti = 1325
}

public enum ParametroTipo
{
    Sistema = 159,
    Filial = 160,
    Perfil = 161,
    Grupo = 350,
    Empresa = 351
}

public enum CondicaoPagtoTipo
{
    Parcelada = 36,
    FaixaDias = 37,
    Semanal = 38
}

public enum ParcelamentoArredondamento
{
    Primeira = 46,
    Ultima = 47
}

public enum Disponibilidade
{
    Pagamento = 56,
    Recebimento = 57,
    Ambas = 101
}

public enum VencimentoTipo
{
    Normal = 59,
    AntecipaDiaUtil = 60,
    ProrrogaDiaUtil = 61
}

public enum ParcelamentoTipo
{
    DiaFixo = 119,
    DiasEntreParcela = 120,
    DiasUteis = 693
}

public enum DiaSemana
{
    Domingo = 164,
    Segunda = 165,
    Terca = 166,
    Quarta = 167,
    Quinta = 168,
    Sexta = 169,
    Sabado = 170
}

public enum DivisaoParcelamentoTipo
{
    Percentual = 603,
    Quantidade = 604
}

public enum TipoValidacaoCampo
{
    PermitirSemValidacao = 0,
    Avisar = 1,
    BloquearSalvar = 2
}

