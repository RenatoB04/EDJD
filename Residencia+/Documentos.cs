using System;
using System.Collections.Generic;

public class Documentos
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Caminho { get; set; }
    public long Tamanho { get; set; }
    public string TipoArquivo { get; set; }
    public int NumeroIdentificacao { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Documentos> ListaDocumentos = new List<Documentos>();

    public Documentos(string nome, string tipo, DateTime dataCriacao, string caminho, long tamanho, string tipoArquivo, int numeroIdentificacao, int unidadeCondominio)
    {
        Nome = nome;
        Tipo = tipo;
        DataCriacao = dataCriacao;
        Caminho = caminho;
        Tamanho = tamanho;
        TipoArquivo = tipoArquivo;
        NumeroIdentificacao = numeroIdentificacao;
        UnidadeCondominio = unidadeCondominio;

        ListaDocumentos.Add(this);
    }

    public static List<Documentos> ObterTodos()
    {
        return ListaDocumentos;
    }
}