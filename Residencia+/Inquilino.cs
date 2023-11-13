public class Inquilino
{
	public string Nome { get; set; }
	public string Morada { get; set; }
	public string Contacto { get; set; }
	public int UnidadeCondominio { get; set; }

	public Inquilino(string nome, string morada, string contacto, int unidadeCondominio)
	{
		Nome = nome;
		Morada = morada;
		Contacto = contacto;
		UnidadeCondominio = unidadeCondominio;
	}

	public static void Adicionar(Inquilino inquilino)
	{
		// Adiciona o inquilino a uma lista
		Inquilino.ListaInquilinos.Add(inquilino);
	}

	private static List<Inquilino> ListaInquilinos = new List<Inquilino>();
}