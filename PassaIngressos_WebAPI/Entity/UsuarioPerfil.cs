using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Usuario_Perfil", Schema = "acesso")]
    public class UsuarioPerfil
    {
        [Key]
        [Column("Id_Usuario_Perfil")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuarioPerfil { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("IdPerfil")]
        public Perfil Perfil { get; set; }

        #region Foreign Keys

        [Column("Id_Usuario")]
        public int IdUsuario { get; set; }

        [Column("Id_Perfil")]
        public int IdPerfil { get; set; }

        #endregion
    }
}