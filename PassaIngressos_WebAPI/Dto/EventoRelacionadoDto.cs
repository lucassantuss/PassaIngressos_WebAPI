namespace PassaIngressos_WebAPI.Dto
{
    public class EventoRelacionadoDto
    {
        public int? IdEvento { get; set; }

        public string NomeEvento { get; set; }

        public int Ano { get; set; }

        public int QuantidadeIngressosDisponiveis { get; set; }

        public int? IdArquivoEvento { get; set; }
    }
}