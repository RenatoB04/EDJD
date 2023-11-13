using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Bem-vindo ao Residencia+\nEscolha uma opção:");
            Console.WriteLine("1. Consultar informações");
            Console.WriteLine("2. Adicionar novo registro");
            Console.WriteLine("3. Sair");

            if (int.TryParse(Console.ReadLine(), out int opcao))
            {
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
        Console.WriteLine("1. Condomínios");
        Console.WriteLine("2. Despesas");
        Console.WriteLine("3. Documentos");
        Console.WriteLine("4. Inquilinos");
        Console.WriteLine("5. Pagamentos");
        Console.WriteLine("6. Proprietários");
        Console.WriteLine("7. Receitas");
        Console.WriteLine("8. Reuniões");

        if (int.TryParse(Console.ReadLine(), out int opcao))
        {
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
        Console.WriteLine("1. Proprietários");
        Console.WriteLine("2. Inquilinos");
        Console.WriteLine("3. Despesas");
        Console.WriteLine("4. Receitas");
        Console.WriteLine("5. Reuniões");
        Console.WriteLine("6. Documentos");
        Console.WriteLine("7. Condomínios");
        Console.WriteLine("8. Pagamentos");

        if (int.TryParse(Console.ReadLine(), out int opcao))
        {
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
            Console.WriteLine(item);
        }
    }
}