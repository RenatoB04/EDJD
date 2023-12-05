using System;
using System.Collections.Generic;


class Program
{
    static void Main()
    {
        Pagamento.dadosCarregados = false;
        Condominio.InicializarDados();
        Despesa.InicializarDados();
        Documento.InicializarDados();
        Inquilino.InicializarDados();
        Pagamento.CarregarDadosDoArquivo(Pagamento.caminhoArquivo);
        Proprietario.InicializarDados();
        Receita.InicializarDados();
        Reuniao.InicializarDados();

        while (true)
        {
            Console.WriteLine("\nBem-vindo ao Residencia+\nEscolha uma opção:\n1. Consultar informações\n2. Adicionar novo registro\n3. Sair");
            if (int.TryParse(Console.ReadLine(), out int opcao))
            {
                Console.Clear();
                switch (opcao)
                {
                    case 1:
                        ConsultarInformacoes();
                        break;
                    case 2:
                        AdicionarNovoRegisto();
                        break;
                    case 3:
                        Console.WriteLine("A encerrar o programa.");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Opção inválida. Tente novamente.");
            }

            Console.WriteLine();
        }
    }

    static void ConsultarInformacoes()
    {
        Console.WriteLine("Escolha uma classe para consultar informações:");
        Console.WriteLine("1. Condomínios\n2. Despesas\n3. Documentos\n4. Inquilinos\n5. Pagamentos\n6. Proprietários\n7. Receitas\n8. Reuniões");

        if (int.TryParse(Console.ReadLine(), out int opcao))
        {
            Console.Clear();
            switch (opcao)
            {
                case 1:
                    ExibirInformacoes(Condominio.ObterTodos());
                    break;
                case 2:
                    ExibirInformacoes(Despesa.ObterTodos());
                    break;
                case 3:
                    ExibirInformacoes(Documento.ObterTodos());
                    break;
                case 4:
                    ExibirInformacoes(Inquilino.ObterTodos());
                    break;
                case 5:
                    ExibirInformacoes(Pagamento.ObterTodos());
                    break;
                case 6:
                    ExibirInformacoes(Proprietario.ObterTodos());
                    break;
                case 7:
                    ExibirInformacoes(Receita.ObterTodos());
                    break;
                case 8:
                    ExibirInformacoes(Reuniao.ObterTodos());
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Opção inválida. Tente novamente.");
        }
    }

    static void AdicionarNovoRegisto()
    {
        if (!Pagamento.dadosCarregados)
    {
        Pagamento.CarregarDadosDoArquivo(Pagamento.caminhoArquivo);
        Pagamento.dadosCarregados = true;
    }
        Console.WriteLine("Escolha uma classe para adicionar um novo registro:");
        Console.WriteLine("1. Condomínios\n2. Despesa\n3. Documento\n4. Inquilino\n5. Pagamento\n6. Proprietários\n7. Receita\n8. Reuniões");

        if (int.TryParse(Console.ReadLine(), out int opcao))
        {
            Console.Clear();
            switch (opcao)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    if (!Pagamento.dadosCarregados)
                    {
                        Pagamento.CarregarDadosDoArquivo(Pagamento.caminhoArquivo);
                    }

                    Pagamento.AdicionarPagamento();
                    Pagamento.SalvarDadosNoArquivo(Pagamento.caminhoArquivo);
                    Console.WriteLine("Pagamento adicionado com sucesso!");
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Opção inválida. Tente novamente.");
        }
    }

    static void ExibirInformacoes<T>(List<T> lista)
    {
        Console.WriteLine($"Informações da classe {typeof(T).Name}:");
        foreach (var item in lista)
        {
            if (item is Condominio condominio)
            {
                Console.WriteLine($"Nome: {condominio.Nome}, Morada: {condominio.Morada}");
            }
            else if (item is Despesa despesa)
            {
                Console.WriteLine($"Descrição: {despesa.Descricao}, Valor: {despesa.Valor}, Data: {despesa.Data.ToShortDateString()}, Unidade do Condomínio: {despesa.UnidadeCondominio}");
            }
            else if (item is Documento documento)
            {
                Console.WriteLine($"Nome: {documento.Nome}, Tipo: {documento.Tipo}, Data de Criação: {documento.DataCriacao.ToShortDateString()}, Número de Identificação: {documento.NumeroIdentificacao}");
            }
            else if (item is Inquilino inquilino)
            {
                Console.WriteLine($"Nome: {inquilino.Nome}, Morada: {inquilino.Morada}, Contacto: {inquilino.Contacto}, Unidade do Condomínio: {inquilino.UnidadeCondominio}, Número de Identificação: {inquilino.NumeroIdentificacao}");
            }
            else if (item is Pagamento pagamento)
            {
                Console.WriteLine($"Descrição: {pagamento.Descricao}, Valor Pago: {pagamento.ValorPago}, Data: {pagamento.Data.ToShortDateString()}, Unidade do Condomínio: {pagamento.UnidadeCondominio}, Número de Identificação: {pagamento.NumeroIdentificacao}");
            }
            else if (item is Proprietario proprietario)
            {
                Console.WriteLine($"Nome: {proprietario.Nome}, Morada: {proprietario.Morada}, Contacto: {proprietario.Contacto}, Unidade do Condomínio: {proprietario.UnidadeCondominio}, Número de Identificação: {proprietario.NumeroIdentificacao}");
            }
            else if (item is Receita receita)
            {
                Console.WriteLine($"Descrição: {receita.Descricao}, Valor: {receita.Valor}, Data: {receita.Data.ToShortDateString()}, Unidade do Condomínio: {receita.UnidadeCondominio}");
            }
            else if (item is Reuniao reuniao)
            {
                Console.WriteLine($"Assunto: {reuniao.Assunto}, Data e Hora: {reuniao.DataHora}, Local: {reuniao.Local}, Unidade do Condomínio: {reuniao.UnidadeCondominio}");
            }
            else
            {
                Console.WriteLine(item);
            }
        }
    }
}