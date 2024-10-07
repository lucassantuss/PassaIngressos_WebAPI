using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Feedback", Schema = "core")]
    public class Feedback
    {
        [Key]
        [Column("Id_Feedback")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFeedback { get; set; }

        [Column("Descricao_Feedback")]
        public string DescricaoFeedback { get; set; }

        [ForeignKey("IdPessoa")]
        public Pessoa Pessoa { get; set; }

        #region Foreign Keys

        [Column("Id_Pessoa")]
        public int IdPessoa { get; set; }

        #endregion
    }
}