using System;
using System.Collections.Generic;

public class Inquilinos
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public int UnidadeCondominio { get; set; }

    public Inquilinos(string nome, string morada, string contacto, int unidadeCondominio)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        UnidadeCondominio = unidadeCondominio;
    }

    private static List<Inquilinos> ListaInquilinos = new List<Inquilinos>();

    public static void Adicionar(Inquilinos inquilino)
    {
        ListaInquilinos.Add(inquilino);
    }

    public static List<Inquilinos> ObterTodos()
    {
        return ListaInquilinos;
    }
}