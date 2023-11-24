using System;
using System.Collections.Generic;

public class Documento
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int NumeroIdentificacao { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Documento> ListaDocumento = new List<Documento>();

    public Documento(string nome, string tipo, DateTime dataCriacao, int numeroIdentificacao, int unidadeCondominio)
    {
        Nome = nome;
        Tipo = tipo;
        DataCriacao = dataCriacao;
        NumeroIdentificacao = numeroIdentificacao;
        UnidadeCondominio = unidadeCondominio;

        ListaDocumento.Add(this);
    }

    public static List<Documento> ObterTodos()
    {
        return ListaDocumento;
    }

    public static void InicializarDados()
    {
        Documento documento1 = new Documento("Alvará", "PDF", new DateTime(2023, 11, 1), 0, 1);
        Documento documento2 = new Documento("Regulamento Interno", "Word", new DateTime(2023, 11, 5), 0, 2);
        Documento documento3 = new Documento("Registo de Limpeza", "Excel", new DateTime(2023, 11, 10), 0, 1);
    }
}