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
