using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Tabela_Geral", Schema = "core")]
    public class TabelaGeral
    {
        [Key]
        [Column("Id_Tabela_Geral")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTabelaGeral { get; set; }

        [Column("Tabela")]
        public string Tabela { get; set; }
    }
}