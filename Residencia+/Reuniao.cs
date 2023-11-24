using System;
using System.Collections.Generic;

public class Reuniao
{
    public string Assunto { get; set; }
    public DateTime DataHora { get; set; }
    public string Local { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Reuniao> ListaReuniao = new List<Reuniao>();

    public Reuniao(string assunto, DateTime dataHora, string local, int unidadeCondominio)
    {
        Assunto = assunto;
        DataHora = dataHora;
        Local = local;
        UnidadeCondominio = unidadeCondominio;

        ListaReuniao.Add(this);
    }

    public static List<Reuniao> ObterTodos()
    {
        return ListaReuniao;
    }
    public static void InicializarDados()
    {
        Reuniao reuniao1 = new Reuniao("Assembleia Geral", new DateTime(2023, 11, 10, 15, 30, 0), "Salão de Festas", 1);
        Reuniao reuniao2 = new Reuniao("Conselho de Administração", new DateTime(2023, 11, 15, 18, 0, 0), "Sala de Reuniões", 2);
        Reuniao reuniao3 = new Reuniao("Comissão de Obras", new DateTime(2023, 11, 5, 14, 0, 0), "Área Comum", 1);
    }
}