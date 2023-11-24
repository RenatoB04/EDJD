using System;
using System.Collections.Generic;

public class Pagamento
{
    public string Descricao { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }
    public int NumeroIdentificacao { get; set; }

    private static List<Pagamento> ListaPagamento = new List<Pagamento>();

    public Pagamento(string descricao, decimal valorPago, DateTime data, int unidadeCondominio, int numeroIdentificacao)
    {
        Descricao = descricao;
        ValorPago = valorPago;
        Data = data;
        UnidadeCondominio = unidadeCondominio;
        NumeroIdentificacao = numeroIdentificacao;

        ListaPagamento.Add(this);
    }

    public static List<Pagamento> ObterTodos()
    {
        return ListaPagamento;
    }

    public static void InicializarDados()
    {
        Pagamento pagamento1 = new Pagamento("Taxa Mensal", 100.0m, new DateTime(2023, 11, 5), 1, 197);
        Pagamento pagamento2 = new Pagamento("Taxa Mensal", 150.0m, new DateTime(2023, 11, 10), 2, 214);
        Pagamento pagamento3 = new Pagamento("Taxa Extra", 50.0m, new DateTime(2023, 11, 15), 1, 320);
    }
}