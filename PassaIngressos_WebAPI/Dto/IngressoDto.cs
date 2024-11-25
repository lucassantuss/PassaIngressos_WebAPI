namespace PassaIngressos_WebAPI.Dto
{
    public class IngressoDto
    {
        public string NomeEvento { get; set; }

        public string LocalEvento { get; set; }

        public DateTime? DataHoraEvento { get; set; }

        public int IdTipoIngresso { get; set; }

        public int IdPessoaAnunciante { get; set; }

        public int IdArquivoEvento { get; set; }

        public decimal? Valor { get; set; }
    }
}