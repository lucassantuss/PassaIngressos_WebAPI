namespace PassaIngressos_WebAPI.Dto
{
    public class FeedbackRetornoDto
    {
        public int IdFeedback { get; set; }

        public string DescricaoFeedback { get; set; }

        public int IdPessoa { get; set; }

        public string NomePessoa { get; set; }

        public int IdadePessoa { get; set; }

        public int? IdArquivoFoto { get; set; }
    }
}