using System;
using System.Collections.Generic;
using System.IO;

public class Pagamento
{
    public string Descricao { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime Data { get; set; }
    public string Estado { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Pagamento> ListaPagamento = new List<Pagamento>();
    public static string caminhoFicheiro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pagamentos.txt");
    public static bool dadosCarregados = false;

    public Pagamento(string descricao, decimal valorPago, DateTime data, string estado, int unidadeCondominio)
    {
        Descricao = descricao;
        ValorPago = valorPago;
        Data = data;
        Estado = estado;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void AdicionarPagamento()
    {
        Console.Write("Descrição: ");
        string descricao = Console.ReadLine();

        Console.Write("Valor Pago: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal valorPago))
        {
            Console.Write("Data (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime data))
            {
                Console.Write("Estado do Pagamento: ");
                string estado = Console.ReadLine();

                Console.Write("Unidade do Condomínio: ");
                if (int.TryParse(Console.ReadLine(), out int unidadeCondominio))
                {
                    Pagamento pagamento = new Pagamento(descricao, valorPago, data, estado, unidadeCondominio);
                    ListaPagamento.Add(pagamento);
                    Console.WriteLine("Pagamento adicionado com sucesso!");
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
            Console.WriteLine("Valor Pago inválido.");
        }
    }

    public static List<Pagamento> ObterTodos()
    {
        return ListaPagamento;
    }

    public static void CarregarDadosDoFicheiro(string caminhoFicheiro)
    {
        if (!dadosCarregados && File.Exists(caminhoFicheiro))
        {
            ListaPagamento.Clear();

            string[] linhas = File.ReadAllLines(caminhoFicheiro);

            foreach (string linha in linhas)
            {
                string[] dados = linha.Split(',');
                if (dados.Length == 5)
                {
                    Pagamento pagamento = new Pagamento(
                        dados[0].Trim(),
                        decimal.Parse(dados[1].Trim()),
                        DateTime.Parse(dados[2].Trim()),
                        dados[3].Trim(),
                        int.Parse(dados[4].Trim())
                    );

                    ListaPagamento.Add(pagamento);
                }
            }

            dadosCarregados = true;
        }
    }

    public static void GuardarDadosNoFicheiro(string caminhoFicheiro)
    {
        using (StreamWriter writer = new StreamWriter(caminhoFicheiro))
        {
            foreach (Pagamento pagamento in ListaPagamento)
            {
                writer.WriteLine($"{pagamento.Descricao},{pagamento.ValorPago},{pagamento.Data},{pagamento.Estado},{pagamento.UnidadeCondominio}");
            }
        }
    }
}