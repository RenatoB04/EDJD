public class Receita
{
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int UnidadeCondominio { get; set; }

    public Receita(string descricao, decimal valor, DateTime data, int unidadeCondominio)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void Adicionar(Receita receita)
    {
        // Adiciona a receita a uma lista
        Receita.ListaReceitas.Add(receita);
    }

    private static List<Receita> ListaReceitas = new List<Receita>();
}