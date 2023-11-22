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
    public static void InicializarDados()
    {
        Despesas despesa1 = new Despesas("Manutenção", 250.0m,new DateTime(2023, 11, 1), 1);
        Despesas despesa2 = new Despesas("Limpeza", 500.0m,new DateTime(2023, 11, 15), 2);
        Despesas despesa3 = new Despesas("Segurança", 750.0m,new DateTime(2023, 11, 1), 1);
    }
}