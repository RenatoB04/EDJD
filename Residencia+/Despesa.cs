using System;
using System.Collections.Generic;
using System.IO;

public class Despesa
{
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }
    public int Estado { get; set; }

    private static List<Despesa> ListaDespesa = new List<Despesa>();
    public static string caminhoFicheiro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "despesas.txt");

    public Despesa(string descricao, decimal valor, DateTime data, int unidadeCondominio, string estado)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        UnidadeCondominio = unidadeCondominio;
        Estado = Estado;
    }

    public static void AdicionarDespesa()
    {
        Console.Write("Descrição da Despesa: ");
        string descricao = Console.ReadLine();

        bool descricaoJaExistente = false;
        foreach (var despesa in ListaDespesa)
        {
            if (despesa.Descricao == descricao)
            {
                descricaoJaExistente = true;
                break;
            }
        }

        if (descricaoJaExistente)
        {
            Console.WriteLine("Já existe uma despesa com essa descrição.");
            return;
        }

        Console.Write("Valor da Despesa: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal valor))
        {
            Console.Write("Data da Despesa (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime data))
            {
                Console.Write("Unidade do Condomínio: ");
                if (int.TryParse(Console.ReadLine(), out int unidadeCondominio))
                {
                    Console.Write("Estado da Despesa: ");
                    string estado = Console.ReadLine();

                    Despesa despesa = new Despesa(descricao, valor, data, unidadeCondominio, estado);
                    ListaDespesa.Add(despesa);
                    Console.WriteLine("Despesa adicionada com sucesso!");
                    GuardarDadosNoFicheiro(caminhoFicheiro);
                }
                else
                {
                    Console.WriteLine("Unidade do Condomínio inválida.");
                }
            }
            else
            {
                Console.WriteLine("Data inválida.");
            }
        }
        else
        {
            Console.WriteLine("Valor da Despesa inválido.");
        }
    }

    public static List<Despesa> ObterTodos()
    {
        CarregarDadosDoFicheiro(caminhoFicheiro);
        return ListaDespesa;
    }

    public static void CarregarDadosDoFicheiro(string caminhoFicheiro)
    {
        ListaDespesa.Clear();

        if (File.Exists(caminhoFicheiro))
        {
            string[] linhas = File.ReadAllLines(caminhoFicheiro);

            foreach (string linha in linhas)
            {
                string[] dados = linha.Split(',');
                if (dados.Length == 5)
                {
                    Despesa despesa = new Despesa(
                        dados[0].Trim(),
                        decimal.Parse(dados[1].Trim()),
                        DateTime.Parse(dados[2].Trim()),
                        int.Parse(dados[3].Trim()),
                        dados[4].Trim()
                    );

                    ListaDespesa.Add(despesa);
                }
            }
        }
    }

    public static void GuardarDadosNoFicheiro(string caminhoFicheiro)
    {
        using (StreamWriter writer = new StreamWriter(caminhoFicheiro))
        {
            foreach (Despesa despesa in ListaDespesa)
            {
                writer.WriteLine($"{despesa.Descricao},{despesa.Valor},{despesa.Data},{despesa.UnidadeCondominio},{despesa.Estado}");
            }
        }
    }
}