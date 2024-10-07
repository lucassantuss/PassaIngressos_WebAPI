namespace PassaIngressos_WebAPI.Dto
{
    public class UsuariosPerfilDto
    {
        public int IdUsuario { get; set; }
        public string Login { get; set; }

        public PessoaDto Pessoa { get; set; }
    }
}