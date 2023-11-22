using System;
using System.Collections.Generic;

public class Proprietarios
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public int UnidadeCondominio { get; set; }
    public int NumeroIdentificacao { get; set; }

    private static List<Proprietarios> ListaProprietarios = new List<Proprietarios>();

    public Proprietarios(string nome, string morada, string contacto, int unidadeCondominio, int numeroIdentificacao)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        UnidadeCondominio = unidadeCondominio;
        NumeroIdentificacao = numeroIdentificacao;

        ListaProprietarios.Add(this);
    }

    public static List<Proprietarios> ObterTodos()
    {
        return ListaProprietarios;
    }

    public static void InicializarDados()
    {
        Proprietarios proprietario1 = new Proprietarios("Ana Maia", "Condomínio 1 4ºB", "111222333", 1, 12);
        Proprietarios proprietario2 = new Proprietarios("José Orta", "Condomínio 2 1ºD", "444555666", 2, 27);
        Proprietarios proprietario3 = new Proprietarios("Marta Ramos", "Condomínio 2 2ºA", "777888999", 1, 34);
    }
}
