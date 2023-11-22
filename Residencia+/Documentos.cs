using System;
using System.Collections.Generic;

public class Documentos
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int NumeroIdentificacao { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Documentos> ListaDocumentos = new List<Documentos>();

    public Documentos(string nome, string tipo, DateTime dataCriacao, int numeroIdentificacao, int unidadeCondominio)
    {
        Nome = nome;
        Tipo = tipo;
        DataCriacao = dataCriacao;
        NumeroIdentificacao = numeroIdentificacao;
        UnidadeCondominio = unidadeCondominio;

        ListaDocumentos.Add(this);
    }

    public static List<Documentos> ObterTodos()
    {
        return ListaDocumentos;
    }

    public static void InicializarDados()
    {
        Documentos documento1 = new Documentos("Alvará", "PDF", new DateTime(2023, 11, 1), 0, 1);
        Documentos documento2 = new Documentos("Regulamento Interno", "Word", new DateTime(2023, 11, 5), 0, 2);
        Documentos documento3 = new Documentos("Registo de Limpeza", "Excel", new DateTime(2023, 11, 10), 0, 1);
    }
}