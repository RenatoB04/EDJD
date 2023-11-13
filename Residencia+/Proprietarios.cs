using System;
using System.Collections.Generic;

public class Proprietarios
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Proprietarios> ListaProprietarios = new List<Proprietarios>();

    public Proprietarios(string nome, string morada, string contacto, int unidadeCondominio)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        UnidadeCondominio = unidadeCondominio;

        ListaProprietarios.Add(this);
    }

    public static List<Proprietarios> ObterTodos()
    {
        return ListaProprietarios;
    }
}