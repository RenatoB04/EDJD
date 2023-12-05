using System;
using System.Collections.Generic;
using System.IO;

public class Inquilino
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Inquilino> ListaInquilino = new List<Inquilino>();
    public static string caminhoFicheiro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inquilinos.txt");
    public static bool dadosCarregados = false;

    public Inquilino(string nome, string morada, string contacto, int unidadeCondominio)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void AdicionarInquilino()
    {
        Console.Write("Nome do Inquilino: ");
        string nome = Console.ReadLine();

        bool nomeJaExistente = false;
        foreach (var inquilino in ListaInquilino)
        {
            if (inquilino.Nome == nome)
            {
                nomeJaExistente = true;
                break;
            }
        }

        if (nomeJaExistente)
        {
            Console.WriteLine("Já existe um inquilino com esse nome.");
            return;
        }

        Console.Write("Morada do Inquilino: ");
        string morada = Console.ReadLine();

        Console.Write("Contacto do Inquilino: ");
        string contacto = Console.ReadLine();

        Console.Write("Unidade do Condomínio: ");
        if (int.TryParse(Console.ReadLine(), out int unidadeCondominio))
        {
            Inquilino inquilino = new Inquilino(nome, morada, contacto, unidadeCondominio);
            ListaInquilino.Add(inquilino);
            Console.WriteLine("Inquilino adicionado com sucesso!");
            GuardarDadosNoFicheiro(caminhoFicheiro);
        }
        else
        {
            Console.WriteLine("Unidade do Condomínio inválida.");
        }
    }

    public static List<Inquilino> ObterTodos()
    {
        return ListaInquilino;
    }

    public static void CarregarDadosDoFicheiro(string caminhoFicheiro)
    {
        if (!dadosCarregados && File.Exists(caminhoFicheiro))
        {
            ListaInquilino.Clear();

            string[] linhas = File.ReadAllLines(caminhoFicheiro);

            foreach (string linha in linhas)
            {
                string[] dados = linha.Split(',');
                if (dados.Length == 4)
                {
                    Inquilino inquilino = new Inquilino(
                        dados[0].Trim(),
                        dados[1].Trim(),
                        dados[2].Trim(),
                        int.Parse(dados[3].Trim())
                    );

                    ListaInquilino.Add(inquilino);
                }
            }

            dadosCarregados = true;
        }
    }

    public static void GuardarDadosNoFicheiro(string caminhoFicheiro)
    {
        using (StreamWriter writer = new StreamWriter(caminhoFicheiro))
        {
            foreach (Inquilino inquilino in ListaInquilino)
            {
                writer.WriteLine($"{inquilino.Nome},{inquilino.Morada},{inquilino.Contacto},{inquilino.UnidadeCondominio}");
            }
        }
    }
}
