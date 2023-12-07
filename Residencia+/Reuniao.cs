using System;
using System.Collections.Generic;
using System.IO;

public class Reuniao
{
    public string Assunto { get; set; }
    public DateTime DataHora { get; set; }
    public string Local { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Reuniao> ListaReuniao = new List<Reuniao>();
    public static string caminhoFicheiro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reunioes.txt");

    public Reuniao(string assunto, DateTime dataHora, string local, int unidadeCondominio)
    {
        Assunto = assunto;
        DataHora = dataHora;
        Local = local;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void AdicionarReuniao()
    {
        Console.Write("Assunto da Reunião: ");
        string assunto = Console.ReadLine();

        Console.Write("Data e Hora da Reunião (yyyy-MM-dd HH:mm): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime dataHora))
        {
            Console.Write("Local da Reunião: ");
            string local = Console.ReadLine();

            Console.Write("Unidade do Condomínio: ");
            if (int.TryParse(Console.ReadLine(), out int unidadeCondominio))
            {
                Reuniao reuniao = new Reuniao(assunto, dataHora, local, unidadeCondominio);
                ListaReuniao.Add(reuniao);
                Console.WriteLine("Reunião adicionada com sucesso!");
                GuardarDadosNoFicheiro(caminhoFicheiro);
            }
            else
            {
                Console.WriteLine("Unidade do Condomínio inválida.");
            }
        }
        else
        {
            Console.WriteLine("Formato de Data e Hora inválido.");
        }
    }

    public static List<Reuniao> ObterTodos()
    {
        CarregarDadosDoFicheiro(caminhoFicheiro);
        return ListaReuniao;
    }

    public static void CarregarDadosDoFicheiro(string caminhoFicheiro)
    {
        ListaReuniao.Clear();

        if (File.Exists(caminhoFicheiro))
        {
            string[] linhas = File.ReadAllLines(caminhoFicheiro);

            foreach (string linha in linhas)
            {
                string[] dados = linha.Split(',');
                if (dados.Length == 4)
                {
                    Reuniao reuniao = new Reuniao(
                        dados[0].Trim(),
                        DateTime.Parse(dados[1].Trim()),
                        dados[2].Trim(),
                        int.Parse(dados[3].Trim())
                    );

                    ListaReuniao.Add(reuniao);
                }
            }
        }
    }

    public static void GuardarDadosNoFicheiro(string caminhoFicheiro)
    {
        using (StreamWriter writer = new StreamWriter(caminhoFicheiro))
        {
            foreach (Reuniao reuniao in ListaReuniao)
            {
                writer.WriteLine($"{reuniao.Assunto},{reuniao.DataHora},{reuniao.Local},{reuniao.UnidadeCondominio}");
            }
        }
    }
}