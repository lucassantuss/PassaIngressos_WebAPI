using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Usuario", Schema = "acesso")]
    public class Usuario
    {
        [Key]
        [Column("Id_Usuario")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Column("Login")]
        public string Login { get; set; }

        [Column("Senha")]
        public string Senha { get; set; }

        [ForeignKey("IdPessoa")]
        public Pessoa Pessoa { get; set; }

        #region Foreign Keys

        [Column("Id_Pessoa")]
        public int IdPessoa { get; set; }

        #endregion
    }
}