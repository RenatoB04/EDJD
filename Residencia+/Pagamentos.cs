public class Pagamento
{
    public string Descricao { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime DataPagamento { get; set; }
    public int UnidadeCondominio { get; set; }

    public Pagamento(string descricao, decimal valorPago, DateTime dataPagamento, int unidadeCondominio)
    {
        Descricao = descricao;
        ValorPago = valorPago;
        DataPagamento = dataPagamento;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void Adicionar(Pagamento pagamento)
    {
        // Adiciona o pagamento a uma lista
        Pagamento.ListaPagamentos.Add(pagamento);
    }

    private static List<Pagamento> ListaPagamentos = new List<Pagamento>();
}