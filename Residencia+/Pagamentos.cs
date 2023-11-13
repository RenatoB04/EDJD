using System;
using System.Collections.Generic;

public class Pagamentos
{
    public string Descricao { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Pagamentos> ListaPagamento = new List<Pagamentos>();

    public Pagamentos(string descricao, decimal valorPago, DateTime data, int unidadeCondominio)
    {
        Descricao = descricao;
        ValorPago = valorPago;
        Data = data;
        UnidadeCondominio = unidadeCondominio;

        ListaPagamento.Add(this);
    }

    public static List<Pagamentos> ObterTodos()
    {
        return ListaPagamento;
    }
}