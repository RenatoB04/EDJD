using System;
using System.Collections.Generic;

public class Receita
{
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Receita> ListaReceita = new List<Receita>();

    public Receita(string descricao, decimal valor, DateTime data, int unidadeCondominio)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        UnidadeCondominio = unidadeCondominio;

        ListaReceita.Add(this);
    }

    public static List<Receita> ObterTodos()
    {
        return ListaReceita;
    }
    public static void InicializarDados()
    {
        Receita receita1 = new Receita("Taxa de Limpeza", 120.0m, new DateTime(2023, 11, 1), 1);
        Receita receita2 = new Receita("Caução 2ºA", 500.0m, new DateTime(2023, 11, 15), 2);
        Receita receita3 = new Receita("Rendas Outubro", 8000.0m, new DateTime(2023, 11, 1), 1);
    }
}