namespace PassaIngressos_WebAPI.Dto
{
    public class ArquivoDto
    {
        public int IdArquivo { get; set; }

        public byte[] ConteudoArquivo { get; set; }

        public string Extensao { get; set; }

        public string ContentType { get; set; }

        public string Nome { get; set; }
    }
}