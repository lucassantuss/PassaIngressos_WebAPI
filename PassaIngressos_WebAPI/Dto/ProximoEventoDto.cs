namespace PassaIngressos_WebAPI.Dto
{
    public class ProximoEventoDto
    {
        public int IdEvento { get; set; }

        public string NomeEvento { get; set; }

        public int Ano { get; set; }

        public int? IdArquivoEvento { get; set; }
    }
}