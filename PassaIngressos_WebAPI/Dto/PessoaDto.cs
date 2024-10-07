namespace PassaIngressos_WebAPI.Dto
{
    public class PessoaDto
    {
        public int? IdPessoa { get; set; }
        public string Nome { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }

        public ItemTabelaGeralDto Sexo { get; set; }
        public ArquivoDto ArquivoFoto { get; set; }
    }
}