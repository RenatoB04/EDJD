using System;
using System.Collections.Generic;
using System.IO;

public class Pagamento
{
    public string Descricao { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }
    public int NumeroIdentificacao { get; set; }

    private static List<Pagamento> ListaPagamento = new List<Pagamento>();

    public Pagamento(string descricao, decimal valorPago, DateTime data, int unidadeCondominio, int numeroIdentificacao)
    {
        Descricao = descricao;
        ValorPago = valorPago;
        Data = data;
        UnidadeCondominio = unidadeCondominio;
        NumeroIdentificacao = numeroIdentificacao;
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
                Console.Write("Unidade do Condomínio: ");
                if (int.TryParse(Console.ReadLine(), out int unidadeCondominio))
                {
                    Console.Write("Número de Identificação: ");
                    if (int.TryParse(Console.ReadLine(), out int numeroIdentificacao))
                    {
                        Pagamento pagamento = new Pagamento(descricao, valorPago, data, unidadeCondominio, numeroIdentificacao);
                        ListaPagamento.Add(pagamento);
                        Console.WriteLine("Pagamento adicionado com sucesso!");
                        SalvarDadosNoArquivo(caminhoArquivo);
                    }
                    else
                    {
                        Console.WriteLine("Número de Identificação inválido.");
                    }
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

    public static string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pagamentos.txt");
    public static bool dadosCarregados = false;

    static void ExibirInformacoes<T>(List<T> lista)
    {
        Console.WriteLine($"Informações da classe {typeof(T).Name}:");

        if (lista.Count == 0)
        {
            Console.WriteLine("Nenhum registro encontrado.");
        }
        else
        {
            foreach (var item in lista)
            {
                if (item is Pagamento pagamento)
                {
                    Console.WriteLine($"Descrição: {pagamento.Descricao}, Valor Pago: {pagamento.ValorPago}, Data: {pagamento.Data.ToShortDateString()}, Unidade do Condomínio: {pagamento.UnidadeCondominio}, Número de Identificação: {pagamento.NumeroIdentificacao}");
                }
                else
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
    public static void CarregarDadosDoArquivo(string caminhoArquivo)
    {
        if (!dadosCarregados && File.Exists(caminhoArquivo))
        {
            if (File.Exists(caminhoArquivo))
            {
                ListaPagamento.Clear();

                string[] linhas = File.ReadAllLines(caminhoArquivo);

                foreach (string linha in linhas)
                {
                    string[] dados = linha.Split(',');
                    if (dados.Length == 5)
                    {
                        Pagamento pagamento = new Pagamento(
                            dados[0].Trim(),
                            decimal.Parse(dados[1].Trim()),
                            DateTime.Parse(dados[2].Trim()),
                            int.Parse(dados[3].Trim()),
                            int.Parse(dados[4].Trim())
                        );

                        ListaPagamento.Add(pagamento);
                    }
                }
            }
            dadosCarregados = true;
        }
    }

    public static void SalvarDadosNoArquivo(string caminhoArquivo)
    {
        using (StreamWriter writer = new StreamWriter(caminhoArquivo))
        {
            foreach (Pagamento pagamento in ListaPagamento)
            {
                writer.WriteLine($"{pagamento.Descricao},{pagamento.ValorPago},{pagamento.Data},{pagamento.UnidadeCondominio},{pagamento.NumeroIdentificacao}");
            }
        }
    }
}