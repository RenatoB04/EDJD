using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Condominios.InicializarDados();
        Despesas.InicializarDados();
        Documentos.InicializarDados();
        Inquilinos.InicializarDados();
        Pagamentos.InicializarDados();
        Proprietarios.InicializarDados();
        Receitas.InicializarDados();
        Reunioes.InicializarDados();

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
                    ExibirInformacoes(Condominios.ObterTodos());
                    break;
                case 2:
                    ExibirInformacoes(Despesas.ObterTodos());
                    break;
                case 3:
                    ExibirInformacoes(Documentos.ObterTodos());
                    break;
                case 4:
                    ExibirInformacoes(Inquilinos.ObterTodos());
                    break;
                case 5:
                    ExibirInformacoes(Pagamentos.ObterTodos());
                    break;
                case 6:
                    ExibirInformacoes(Proprietarios.ObterTodos());
                    break;
                case 7:
                    ExibirInformacoes(Receitas.ObterTodos());
                    break;
                case 8:
                    ExibirInformacoes(Reunioes.ObterTodos());
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

    //Inacabado (Função de adicionar novo registo)

    static void AdicionarNovoRegisto()
    {
        Console.WriteLine("Escolha uma classe para adicionar um novo registro:");
        Console.WriteLine("1. Condomínios\n2. Despesas\n3. Documentos\n4. Inquilinos\n5. Pagamentos\n6. Proprietários\n7. Receitas\n8. Reuniões");

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
            if (item is Condominios condominio)
            {
                Console.WriteLine($"Nome: {condominio.Nome}, Morada: {condominio.Morada}");
            }
            else if (item is Despesas despesa)
            {
                Console.WriteLine($"Descrição: {despesa.Descricao}, Valor: {despesa.Valor}, Data: {despesa.Data.ToShortDateString()}, Unidade do Condomínio: {despesa.UnidadeCondominio}");
            }
            else if (item is Documentos documento)
            {
                Console.WriteLine($"Nome: {documento.Nome}, Tipo: {documento.Tipo}, Data de Criação: {documento.DataCriacao.ToShortDateString()}, Número de Identificação: {documento.NumeroIdentificacao}");
            }
            else if (item is Inquilinos inquilino)
            {
                Console.WriteLine($"Nome: {inquilino.Nome}, Morada: {inquilino.Morada}, Contacto: {inquilino.Contacto}, Unidade do Condomínio: {inquilino.UnidadeCondominio}, Número de Identificação: {inquilino.NumeroIdentificacao}");
            }
            else if (item is Pagamentos pagamento)
            {
                Console.WriteLine($"Descrição: {pagamento.Descricao}, Valor Pago: {pagamento.ValorPago}, Data: {pagamento.Data.ToShortDateString()}, Unidade do Condomínio: {pagamento.UnidadeCondominio}, Número de Identificação: {pagamento.NumeroIdentificacao}");
            }
            else if (item is Proprietarios proprietario)
            {
                Console.WriteLine($"Nome: {proprietario.Nome}, Morada: {proprietario.Morada}, Contacto: {proprietario.Contacto}, Unidade do Condomínio: {proprietario.UnidadeCondominio}, Número de Identificação: {proprietario.NumeroIdentificacao}");
            }
            else if (item is Receitas receita)
            {
                Console.WriteLine($"Descrição: {receita.Descricao}, Valor: {receita.Valor}, Data: {receita.Data.ToShortDateString()}, Unidade do Condomínio: {receita.UnidadeCondominio}");
            }
            else if (item is Reunioes reuniao)
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