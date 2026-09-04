namespace Versatus.AcessoGlobal.Domain.Entities;

// DEC-007: FormaPagtoTipo, CondicaoPagtoTipo, ParcelamentoArredondamento, Disponibilidade,
// VencimentoTipo e ParcelamentoTipo foram movidos para Versatus.SharedKernel.Enums (também
// usados pelo MOD-05 GestaoFinanceira). Os enums abaixo continuam aqui por serem exclusivos
// do Acesso Global.

/// <summary>
/// Origem: Projeto.Geral.Enumerado.EntidadeFisicaJuridica (legado, [TipoEnumerado(1)]) —
/// persistido em GLOENTIDADE.IDFISICAJURIDICA. Corrigido em DEC-007: valores eram 1/2
/// (divergentes do legado, que grava 2/3) — renumerado para bater com o legado.
/// </summary>
public enum EntidadeTipoPessoa
{
    Fisica = 2,
    Juridica = 3
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

