using System;
using System.Collections.Generic;
using System.IO;

public class Condominio
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }

    private static List<Condominio> ListaCondominio = new List<Condominio>();
    public static string caminhoFicheiro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "condominios.txt");
    public static bool dadosCarregados = false;

    public Condominio(string nome, string morada, string contacto)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
    }

    public static void AdicionarCondominio()
    {
        Console.Write("Nome do Condomínio: ");
        string nome = Console.ReadLine();

        Console.Write("Morada do Condomínio: ");
        string morada = Console.ReadLine();

        Console.Write("Contacto do Condomínio: ");
        string contacto = Console.ReadLine();

        Condominio condominio = new Condominio(nome, morada, contacto);
        ListaCondominio.Add(condominio);
        Console.WriteLine("Condomínio adicionado com sucesso!");
        GuardarDadosNoFicheiro(caminhoFicheiro);
    }

    public static List<Condominio> ObterTodos()
    {
        return ListaCondominio;
    }

    public static void CarregarDadosDoFicheiro(string caminhoFicheiro)
    {
        if (!dadosCarregados && File.Exists(caminhoFicheiro))
        {
            if (File.Exists(caminhoFicheiro))
            {
                ListaCondominio.Clear();

                string[] linhas = File.ReadAllLines(caminhoFicheiro);

                foreach (string linha in linhas)
                {
                    string[] dados = linha.Split(',');
                    if (dados.Length == 3)
                    {
                        Condominio condominio = new Condominio(
                            dados[0].Trim(),
                            dados[1].Trim(),
                            dados[2].Trim()
                        );

                        ListaCondominio.Add(condominio);
                    }
                }
            }
            dadosCarregados = true;
        }
    }

    public static void GuardarDadosNoFicheiro(string caminhoFicheiro)
    {
        using (StreamWriter writer = new StreamWriter(caminhoFicheiro))
        {
            foreach (Condominio condominio in ListaCondominio)
            {
                writer.WriteLine($"{condominio.Nome},{condominio.Morada},{condominio.Contacto}");
            }
        }
    }
}