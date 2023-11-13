using System;
using System.Collections.Generic;

public class Receitas
{
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Receitas> ListaReceitas = new List<Receitas>();

    public Receitas(string descricao, decimal valor, DateTime data, int unidadeCondominio)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        UnidadeCondominio = unidadeCondominio;

        ListaReceitas.Add(this);
    }

    public static List<Receitas> ObterTodos()
    {
        return ListaReceitas;
    }
}