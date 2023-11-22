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
    public static void InicializarDados()
    {
        Reunioes reuniao1 = new Reunioes("Assembleia Geral", new DateTime(2023, 11, 10, 15, 30, 0), "Salão de Festas", 1);
        Reunioes reuniao2 = new Reunioes("Conselho de Administração", new DateTime(2023, 11, 15, 18, 0, 0), "Sala de Reuniões", 2);
        Reunioes reuniao3 = new Reunioes("Comissão de Obras", new DateTime(2023, 11, 5, 14, 0, 0), "Área Comum", 1);
    }
}