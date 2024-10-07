namespace PassaIngressos_WebAPI.Dto
{
    public class IngressoDto
    {
        public int IdTipoIngresso { get; set; }

        public decimal? Valor { get; set; }

        public int IdPessoaAnunciante { get; set; }

        public int IdEvento { get; set; }
    }
}