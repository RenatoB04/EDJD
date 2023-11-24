using System;
using System.Collections.Generic;

public class Condominio
{
    public string Nome { get; set; }
    public string Morada { get; set; }

    private static List<Condominio> ListaCondominio = new List<Condominio>();

    public Condominio(string nome, string morada)
    {
        Nome = nome;
        Morada = morada;

        ListaCondominio.Add(this);
    }

    public static List<Condominio> ObterTodos()
    {
        return ListaCondominio;
    }
    public static void InicializarDados()
    {
        Condominio condo1 = new Condominio("Condomínio 1", "Rua das Flores 127");
        Condominio condo2 = new Condominio("Condomínio 2", "Avenida do Sol 453");
    }
}