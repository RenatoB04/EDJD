using System;
using System.Collections.Generic;
using System.IO;

public class Documento
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int UnidadeCondominio { get; set; }

    private static List<Documento> ListaDocumento = new List<Documento>();
    public static string caminhoFicheiro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "documentos.txt");

    public Documento(string nome, string tipo, DateTime dataCriacao, int unidadeCondominio)
    {
        Nome = nome;
        Tipo = tipo;
        DataCriacao = dataCriacao;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void AdicionarDocumento()
    {
        Console.Write("Nome do Documento: ");
        string nome = Console.ReadLine();

        bool nomeJaExistente = false;
        foreach (var documento in ListaDocumento)
        {
            if (documento.Nome == nome)
            {
                nomeJaExistente = true;
                break;
            }
        }

        if (nomeJaExistente)
        {
            Console.WriteLine("Já existe um documento com esse nome.");
            return;
        }

        Console.Write("Tipo do Documento: ");
        string tipo = Console.ReadLine();

        Console.Write("Data de Criação do Documento (yyyy-MM-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime dataCriacao))
        {
            Console.Write("Unidade do Condomínio: ");
            if (int.TryParse(Console.ReadLine(), out int unidadeCondominio))
            {
                Documento documento = new Documento(nome, tipo, dataCriacao, unidadeCondominio);
                ListaDocumento.Add(documento);
                Console.WriteLine("Documento adicionado com sucesso!");
                GuardarDadosNoFicheiro(caminhoFicheiro);
            }
            else
            {
                Console.WriteLine("Unidade do Condomínio inválida.");
            }
        }
        else
        {
            Console.WriteLine("Data de Criação inválida.");
        }
    }

    public static List<Documento> ObterTodos()
    {
        CarregarDadosDoFicheiro(caminhoFicheiro);
        return ListaDocumento;
    }

    public static void CarregarDadosDoFicheiro(string caminhoFicheiro)
    {
        ListaDocumento.Clear();

        if (File.Exists(caminhoFicheiro))
        {
            string[] linhas = File.ReadAllLines(caminhoFicheiro);

            foreach (string linha in linhas)
            {
                string[] dados = linha.Split(',');
                if (dados.Length == 4)
                {
                    Documento documento = new Documento(
                        dados[0].Trim(),
                        dados[1].Trim(),
                        DateTime.Parse(dados[2].Trim()),
                        int.Parse(dados[3].Trim())
                    );

                    ListaDocumento.Add(documento);
                }
            }
        }
    }

    public static void RemoverDados()
    {
        Console.Write("Escreva o nome do Documento que deseja remover: ");
        string nome = Console.ReadLine();

        Documento documentoParaRemover = ListaDocumento.Find(d => d.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (documentoParaRemover != null)
        {
            ListaDocumento.Remove(documentoParaRemover);
            Console.WriteLine("Documento removido com sucesso!");
            GuardarDadosNoFicheiro(caminhoFicheiro);
        }
        else
        {
            Console.WriteLine("Documento não encontrado.");
        }
    }

    public static void GuardarDadosNoFicheiro(string caminhoFicheiro)
    {
        using (StreamWriter writer = new StreamWriter(caminhoFicheiro))
        {
            foreach (Documento documento in ListaDocumento)
            {
                writer.WriteLine($"{documento.Nome},{documento.Tipo},{documento.DataCriacao},{documento.UnidadeCondominio}");
            }
        }
    }
}
