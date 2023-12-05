using System;
using System.Collections.Generic;


class Program
{
    static void Main()
    {
        Condominio.dadosCarregados = false;
        Condominio.CarregarDadosDoFicheiro(Condominio.caminhoFicheiro);
        Despesa.dadosCarregados = false;
        Despesa.CarregarDadosDoFicheiro(Despesa.caminhoFicheiro);
        Documento.dadosCarregados = false;
        Documento.CarregarDadosDoFicheiro(Documento.caminhoFicheiro);
        Inquilino.dadosCarregados = false;
        Inquilino.CarregarDadosDoFicheiro(Inquilino.caminhoFicheiro);
        Pagamento.dadosCarregados = false;
        Pagamento.CarregarDadosDoFicheiro(Pagamento.caminhoFicheiro);
        Proprietario.dadosCarregados = false;
        Proprietario.CarregarDadosDoFicheiro(Proprietario.caminhoFicheiro);
        Receita.dadosCarregados = false;
        Receita.CarregarDadosDoFicheiro(Receita.caminhoFicheiro);
        Reuniao.dadosCarregados = false;
        Reuniao.CarregarDadosDoFicheiro(Reuniao.caminhoFicheiro);

        while (true)
        {
            Console.WriteLine("Bem-vindo ao Residencia+\nEscolha uma opção:\n1. Consultar informações\n2. Adicionar novo registro\n3. Sair");
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
        Pagamento.CarregarDadosDoFicheiro(Pagamento.caminhoFicheiro);
        Pagamento.dadosCarregados = true;
    }
        if (!Condominio.dadosCarregados)
        {
            Condominio.CarregarDadosDoFicheiro(Condominio.caminhoFicheiro);
            Condominio.dadosCarregados = true;
        }
        Console.WriteLine("Escolha uma classe para adicionar um novo registro:");
        Console.WriteLine("1. Condomínios\n2. Despesa\n3. Documento\n4. Inquilino\n5. Pagamento\n6. Proprietários\n7. Receita\n8. Reuniões");

        if (int.TryParse(Console.ReadLine(), out int opcao))
        {
            Console.Clear();
            switch (opcao)
            {
                case 1:
                    Condominio.AdicionarCondominio();
                    Condominio.GuardarDadosNoFicheiro(Condominio.caminhoFicheiro);
                    break;
                case 2:
                    Despesa.AdicionarDespesa();
                    Despesa.GuardarDadosNoFicheiro(Despesa.caminhoFicheiro);
                    break;
                case 3:
                    Documento.AdicionarDocumento();
                    Documento.GuardarDadosNoFicheiro(Documento.caminhoFicheiro);
                    break;
                case 4:
                    Inquilino.AdicionarInquilino();
                    Inquilino.GuardarDadosNoFicheiro(Inquilino.caminhoFicheiro);
                    break;
                case 5:
                    Pagamento.AdicionarPagamento();
                    Pagamento.GuardarDadosNoFicheiro(Pagamento.caminhoFicheiro);
                    break;
                case 6:
                    Proprietario.AdicionarProprietario();
                    Proprietario.GuardarDadosNoFicheiro(Proprietario.caminhoFicheiro);
                    break;
                case 7:
                    Receita.AdicionarReceita();
                    Receita.GuardarDadosNoFicheiro(Receita.caminhoFicheiro);
                    break;
                case 8:
                    Reuniao.AdicionarReuniao();
                    Reuniao.GuardarDadosNoFicheiro(Reuniao.caminhoFicheiro);
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
            switch (item)
            {
                case Condominio condominio:
                    Console.WriteLine($"Nome: {condominio.Nome}, Morada: {condominio.Morada}");
                    break;
                case Despesa despesa:
                    Console.WriteLine($"Descrição: {despesa.Descricao}, Valor: {despesa.Valor}, Data: {despesa.Data.ToShortDateString()}, Unidade do Condomínio: {despesa.UnidadeCondominio}, Estado: {despesa.Estado}");
                    break;
                case Documento documento:
                    Console.WriteLine($"Nome: {documento.Nome}, Tipo: {documento.Tipo}, Data de Criação: {documento.DataCriacao.ToShortDateString()}");
                    break;
                case Inquilino inquilino:
                    Console.WriteLine($"Nome: {inquilino.Nome}, Morada: {inquilino.Morada}, Contacto: {inquilino.Contacto}, Unidade do Condomínio: {inquilino.UnidadeCondominio}");
                    break;
                case Pagamento pagamento:
                    Console.WriteLine($"Descrição: {pagamento.Descricao}, Valor Pago: {pagamento.ValorPago}, Data: {pagamento.Data.ToShortDateString()}, Unidade do Condomínio: {pagamento.UnidadeCondominio}, Estado: {pagamento.Estado}");
                    break;
                case Proprietario proprietario:
                    Console.WriteLine($"Nome: {proprietario.Nome}, Morada: {proprietario.Morada}, Contacto: {proprietario.Contacto}, Unidade do Condomínio: {proprietario.UnidadeCondominio}");
                    break;
                case Receita receita:
                    Console.WriteLine($"Descrição: {receita.Descricao}, Valor: {receita.Valor}, Data: {receita.Data.ToShortDateString()}, Unidade do Condomínio: {receita.UnidadeCondominio}");
                    break;
                case Reuniao reuniao:
                    Console.WriteLine($"Assunto: {reuniao.Assunto}, Data e Hora: {reuniao.DataHora}, Local: {reuniao.Local}, Unidade do Condomínio: {reuniao.UnidadeCondominio}");
                    break;
                default:
                    Console.WriteLine(item);
                    break;
            }
        }
    }
}