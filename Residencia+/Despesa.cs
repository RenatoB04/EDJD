using System;
using System.Collections.Generic;

public class Despesa
{
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Despesa> ListaDespesa = new List<Despesa>();

    public Despesa(string descricao, decimal valor, DateTime data, int unidadeCondominio)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        UnidadeCondominio = unidadeCondominio;

        ListaDespesa.Add(this);
    }

    public static List<Despesa> ObterTodos()
    {
        return ListaDespesa;
    }
    public static void InicializarDados()
    {
        Despesa despesa1 = new Despesa("Manutenção", 250.0m,new DateTime(2023, 11, 1), 1);
        Despesa despesa2 = new Despesa("Limpeza", 500.0m,new DateTime(2023, 11, 15), 2);
        Despesa despesa3 = new Despesa("Segurança", 750.0m,new DateTime(2023, 11, 1), 1);
    }
}