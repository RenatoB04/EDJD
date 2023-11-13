public class Proprietario
{
    public string Nome { get; set; }
    public string Morada { get; set; }
    public string Contacto { get; set; }
    public int UnidadeCondominio { get; set; }

    public Proprietario(string nome, string morada, string contacto, int unidadeCondominio)
    {
        Nome = nome;
        Morada = morada;
        Contacto = contacto;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void Adicionar(Proprietario proprietario)
    {
        // Adiciona o proprietário a uma lista
        Proprietario.ListaProprietarios.Add(proprietario);
    }

    private static List<Proprietario> ListaProprietarios = new List<Proprietario>();
}