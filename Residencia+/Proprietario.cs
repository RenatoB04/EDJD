using System;
using System.Collections.Generic;

public class Proprietario
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public int UnidadeCondominio { get; set; }
    public int NumeroIdentificacao { get; set; }

    private static List<Proprietario> ListaProprietario = new List<Proprietario>();

    public Proprietario(string nome, string morada, string contacto, int unidadeCondominio, int numeroIdentificacao)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        UnidadeCondominio = unidadeCondominio;
        NumeroIdentificacao = numeroIdentificacao;

        ListaProprietario.Add(this);
    }

    public static List<Proprietario> ObterTodos()
    {
        return ListaProprietario;
    }

    public static void InicializarDados()
    {
        Proprietario proprietario1 = new Proprietario("Ana Maia", "Condomínio 1 4ºB", "111222333", 1, 12);
        Proprietario proprietario2 = new Proprietario("José Orta", "Condomínio 2 1ºD", "444555666", 2, 27);
        Proprietario proprietario3 = new Proprietario("Marta Ramos", "Condomínio 2 2ºA", "777888999", 1, 34);
    }
}
