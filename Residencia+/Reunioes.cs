public class Reuniao
{
    public string Assunto { get; set; }
    public DateTime DataHora { get; set; }
    public string Local { get; set; }
    public int UnidadeCondominio { get; set; }

    public Reuniao(string assunto, DateTime dataHora, string local, int unidadeCondominio)
    {
        Assunto = assunto;
        DataHora = dataHora;
        Local = local;
        UnidadeCondominio = unidadeCondominio;
    }

    public static void Adicionar(Reuniao reuniao)
    {
        // Adiciona a reunião a uma lista
        Reuniao.ListaReuniões.Add(reuniao);
    }

    private static List<Reuniao> ListaReuniões = new List<Reuniao>();
}