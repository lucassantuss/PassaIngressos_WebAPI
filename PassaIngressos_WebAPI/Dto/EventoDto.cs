namespace PassaIngressos_WebAPI.Dto
{
    public class EventoDto
    {
        public string NomeEvento { get; set; }

        public string LocalEvento { get; set; }

        public DateTime? DataHoraEvento { get; set; }

        public int? IdArquivoEvento { get; set; }
    }
}