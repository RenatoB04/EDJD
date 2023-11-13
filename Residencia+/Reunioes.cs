using System;
using System.Collections.Generic;

public class Reunioes
{
    public string Assunto { get; set; }
    public DateTime DataHora { get; set; }
    public string Local { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Reunioes> ListaReunioes = new List<Reunioes>();

    public Reunioes(string assunto, DateTime dataHora, string local, int unidadeCondominio)
    {
        Assunto = assunto;
        DataHora = dataHora;
        Local = local;
        UnidadeCondominio = unidadeCondominio;

        ListaReunioes.Add(this);
    }

    public static List<Reunioes> ObterTodos()
    {
        return ListaReunioes;
    }
}