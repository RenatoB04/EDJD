public class Documento
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Caminho { get; set; }
    public long Tamanho { get; set; }
    public string TipoArquivo { get; set; }
    public int NumeroIdentificacao { get; set; }
    public int UnidadeCondominio { get; set; }

    public Documento(string nome, string tipo, DateTime dataCriacao, string caminho, long tamanho, string tipoArquivo, int numeroIdentificacao, int unidadeCondominio)
    {
        Nome = nome;
        Tipo = tipo;
        DataCriacao = dataCriacao;
        Caminho = caminho;
        Tamanho = tamanho;
        TipoArquivo = tipoArquivo;
        NumeroIdentificacao = numeroIdentificacao;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void Adicionar(Documento documento)
    {
        // Adiciona o documento a uma lista
        Documento.ListaDocumentos.Add(documento);
    }

    private static List<Documento> ListaDocumentos = new List<Documento>();
}