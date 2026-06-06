namespace Versatus.GestaoTributo.Domain.Rules;

public enum DefinicaoTributaria
{
    IncidenciaNormal = 357,
    Isencao = 358,
    Suspenso = 359,
    SubstituicaoTributaria = 361,
    Diferido = 370,
    Garantido = 415
}

public enum TipoPauta
{
    Minima = 363,
    Fixa = 364
}

public enum BaseSubstituicao
{
    MVARegra = 366,
    CargaMedia = 886,
    MVACest = 1668,
    ValorOperacao = 1900
}

public enum ValorNaoTributado
{
    Isento = 368,
    Outras = 369,
    Nenhum = 1324
}

public enum ComportamentoTributo
{
    IncideSobreTotalItem = 417,
    RetidoFonte = 418,
    Destacado = 419,
    CalcularSemDestacar = 857,
    NaoCalcular = 858
}

public enum TipoBaseCalculoCargaMedia
{
    ValorOperacao = 1135,
    ValorAgregado = 1136
}

public enum TipoExigibilidadeISS
{
    Exigivel = 1866,
    Naoincidencia = 1867,
    Isencao = 1868,
    Exportacao = 1869,
    Imunidade = 1870,
    SuspensaJudicial = 1871,
    SuspensaAdministrativo = 1872
}

public enum Operadores
{
    Igual = 271,
    Diferente = 272,
    MaiorQue = 273,
    MaiorIgualQue = 274,
    MenorQue = 275,
    MenorIgualQue = 276
}

public enum TipoRegraTributacao
{
    Nenhuma = 534,
    Estado = 535,
    Municipio = 536
}

public enum TipoBaseCalculo
{
    ValorNota = 735,
    DifValorVendaCompra = 736,
    DifCustoVendaCompra = 737
}
