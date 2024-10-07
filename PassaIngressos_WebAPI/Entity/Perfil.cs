using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Perfil", Schema = "acesso")]
    public class Perfil
    {
        [Key]
        [Column("Id_Perfil")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPerfil { get; set; }

        [Column("Nome_Perfil")]
        public string NomePerfil { get; set; }

        [Column("Descricao_Perfil")]
        public string DescricaoPerfil { get; set; }
    }
}