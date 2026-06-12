namespace Projeto.Servidor.Strangler.GestaoFinanceira.DTOs
{
    public class ClienteDto
    {
        public int IdCliente { get; set; }
        public bool? ItemFinanceiroPadrao { get; set; }
        public int? IdCondicaoPagamento { get; set; }
    }
}
