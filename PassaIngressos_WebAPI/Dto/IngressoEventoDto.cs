namespace PassaIngressos_WebAPI.Dto
{
    public class IngressoEventoDto
    {
        public int IdIngresso { get; set; }

        public string NomeEvento { get; set; }

        public string LocalEvento { get; set; }

        public string DataHoraEvento { get; set; }

        public int IdTipoIngresso { get; set; }

        public int IdPessoaAnunciante { get; set; }

        public int? IdArquivoEvento { get; set; }

        public decimal? Valor { get; set; }
    }
}