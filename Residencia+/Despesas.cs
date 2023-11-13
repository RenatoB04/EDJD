using System;
using System.Collections.Generic;

public class Despesas
{
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Despesas> ListaDespesas = new List<Despesas>();

    public Despesas(string descricao, decimal valor, DateTime data, int unidadeCondominio)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        UnidadeCondominio = unidadeCondominio;

        ListaDespesas.Add(this);
    }

    public static List<Despesas> ObterTodos()
    {
        return ListaDespesas;
    }
}