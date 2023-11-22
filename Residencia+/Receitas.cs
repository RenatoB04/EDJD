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
    public static void InicializarDados()
    {
        Receitas receita1 = new Receitas("Taxa de Limpeza", 120.0m, new DateTime(2023, 11, 1), 1);
        Receitas receita2 = new Receitas("Caução 2ºA", 500.0m, new DateTime(2023, 11, 15), 2);
        Receitas receita3 = new Receitas("Rendas Outubro", 8000.0m, new DateTime(2023, 11, 1), 1);
    }
}