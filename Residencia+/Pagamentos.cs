using System;
using System.Collections.Generic;

public class Pagamentos
{
    public string Descricao { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }
    public int NumeroIdentificacao { get; set; }

    private static List<Pagamentos> ListaPagamento = new List<Pagamentos>();

    public Pagamentos(string descricao, decimal valorPago, DateTime data, int unidadeCondominio, int numeroIdentificacao)
    {
        Descricao = descricao;
        ValorPago = valorPago;
        Data = data;
        UnidadeCondominio = unidadeCondominio;
        NumeroIdentificacao = numeroIdentificacao;

        ListaPagamento.Add(this);
    }

    public static List<Pagamentos> ObterTodos()
    {
        return ListaPagamento;
    }

    public static void InicializarDados()
    {
        Pagamentos pagamento1 = new Pagamentos("Taxa Mensal", 100.0m, new DateTime(2023, 11, 5), 1, 197);
        Pagamentos pagamento2 = new Pagamentos("Taxa Mensal", 150.0m, new DateTime(2023, 11, 10), 2, 214);
        Pagamentos pagamento3 = new Pagamentos("Taxa Extra", 50.0m, new DateTime(2023, 11, 15), 1, 320);
    }
}