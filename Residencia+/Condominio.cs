public class Condominio
{
    public string Nome { get; set; }
    public string Morada { get; set; }

    public Condominio(string nome, string morada)
    {
        Nome = nome;
        Morada = morada;
    }

    public static void Adicionar(Condominio condominio)
    {
        // Adiciona o condomínio a uma lista
        Condominio.ListaCondominios.Add(condominio);
    }

    private static List<Condominio> ListaCondominios = new List<Condominio>();
}