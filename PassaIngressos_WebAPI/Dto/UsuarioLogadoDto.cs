namespace PassaIngressos_WebAPI.Dto
{
    public class UsuarioLogadoDto
    {
        public string Login { get; set; }
        public string NomePessoa { get; set; }

        public DateTime? DataNascimento { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public int? IdArquivoFoto { get; set; }
        public int? IdTgSexo { get; set; }
    }
}