using System;
using System.Collections.Generic;
using System.IO;

public class Proprietario
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public string Inquilino { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Proprietario> ListaProprietario = new List<Proprietario>();
    public static string caminhoFicheiro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "proprietarios.txt");

    public Proprietario(string nome, string morada, string contacto, string inquilino, int unidadeCondominio)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        Inquilino = inquilino;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void AdicionarProprietario()
    {
        Console.Write("Nome do Proprietário: ");
        string nome = Console.ReadLine();

        Console.Write("Morada do Proprietário: ");
        string morada = Console.ReadLine();

        Console.Write("Contacto do Proprietário: ");
        string contacto = Console.ReadLine();

        Console.Write("Nome do Inquilino: ");
        string inquilino = Console.ReadLine();

        Console.Write("Unidade do Condomínio: ");
        if (int.TryParse(Console.ReadLine(), out int unidadeCondominio))
        {
            Proprietario proprietario = new Proprietario(nome, morada, contacto, inquilino, unidadeCondominio);
            ListaProprietario.Add(proprietario);
            Console.WriteLine("Proprietário adicionado com sucesso!");
            GuardarDadosNoFicheiro(caminhoFicheiro);
        }
        else
        {
            Console.WriteLine("Unidade do Condomínio inválida.");
        }
    }

    public static List<Proprietario> ObterTodos()
    {
        CarregarDadosDoFicheiro(caminhoFicheiro);
        return ListaProprietario;
    }

    public static void CarregarDadosDoFicheiro(string caminhoFicheiro)
    {
        ListaProprietario.Clear();

        if (File.Exists(caminhoFicheiro))
        {
            string[] linhas = File.ReadAllLines(caminhoFicheiro);

            foreach (string linha in linhas)
            {
                string[] dados = linha.Split(',');
                if (dados.Length == 5)
                {
                    Proprietario proprietario = new Proprietario(
                        dados[0].Trim(),
                        dados[1].Trim(),
                        dados[2].Trim(),
                        dados[3].Trim(),
                        int.Parse(dados[4].Trim())
                    );

                    ListaProprietario.Add(proprietario);
                }
            }
        }
    }

    public static void GuardarDadosNoFicheiro(string caminhoFicheiro)
    {
        using (StreamWriter writer = new StreamWriter(caminhoFicheiro))
        {
            foreach (Proprietario proprietario in ListaProprietario)
            {
                writer.WriteLine($"{proprietario.Nome},{proprietario.Morada},{proprietario.Contacto},{proprietario.Inquilino},{proprietario.UnidadeCondominio}");
            }
        }
    }
}