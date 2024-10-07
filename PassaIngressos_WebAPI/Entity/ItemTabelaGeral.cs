using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Item_Tabela_Geral", Schema = "core")]
    public class ItemTabelaGeral
    {
        [Key]
        [Column("Id_Item_Tabela_Geral")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdItemTabelaGeral { get; set; }

        [Column("Sigla")]
        public string Sigla { get; set; }

        [Column("Descricao")]
        public string Descricao { get; set; }

        [ForeignKey("IdTabelaGeral")]
        public TabelaGeral TabelaGeral { get; set; }

        #region Foreign Keys

        [Column("Id_Tabela_Geral")]
        public int IdTabelaGeral { get; set; }

        #endregion
    }
}