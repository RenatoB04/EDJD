using System;
using System.Collections.Generic;

public class Inquilino
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public int UnidadeCondominio { get; set; }
    public int NumeroIdentificacao { get; set; }

    private static List<Inquilino> ListaInquilino = new List<Inquilino>();

    public Inquilino(string nome, string morada, string contacto, int unidadeCondominio, int numeroIdentificacao)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        UnidadeCondominio = unidadeCondominio;
        NumeroIdentificacao = numeroIdentificacao;

        ListaInquilino.Add(this);
    }

    public static List<Inquilino> ObterTodos()
    {
        return ListaInquilino;
    }

    public static void InicializarDados()
    {
        Inquilino inquilino1 = new Inquilino("João Cruz", "Condomínio 1 4ºB", "123456789", 1, 197);
        Inquilino inquilino2 = new Inquilino("Maria Jordona", "Condomínio 2 1ºD", "987654321", 2, 214);
        Inquilino inquilino3 = new Inquilino("Carlos Maia", "Condomínio 2 2ºA", "555666777", 1, 320);
    }
}