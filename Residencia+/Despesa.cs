public class Despesa
{
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }

    public Despesa(string descricao, decimal valor, DateTime data, int unidadeCondominio)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void Adicionar(Despesa despesa)
    {
        // Adiciona a despesa a uma lista
        Despesa.ListaDespesas.Add(despesa);
    }

    private static List<Despesa> ListaDespesas = new List<Despesa>();
}