namespace PassaIngressos_WebAPI.Dto
{
    public class UsuarioDto
    {
        public string Login { get; set; }

        public string Senha { get; set; }

        public string NomePessoa { get; set; }

        // Campos Opcionais
        public string CPF { get; set; }
        public string RG { get; set; }
        public int? IdArquivoFoto { get; set; }
        public int? IdTgSexo { get; set; }
    }
}