using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Ingresso", Schema = "venda")]
    public class Ingresso
    {
        [Key]
        [Column("Id_Ingresso")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdIngresso { get; set; }

        [Column("Valor")]
        public decimal? Valor { get; set; }

        [Column("Vendido")]
        public bool Vendido { get; set; }

        [ForeignKey("IdTgTipoIngresso")]
        public ItemTabelaGeral TipoIngresso { get; set; }

        [Column("IdPessoaAnunciante")]
        public Pessoa PessoaAnunciante { get; set; }

        [Column("Id_Pessoa_Comprador")]
        public int? IdPessoaComprador { get; set; }

        [Column("IdEvento")]
        public Evento Evento { get; set; }

        #region Foreign Keys

        [Column("Id_Tg_Tipo_Ingresso")]
        public int IdTgTipoIngresso { get; set; }

        [Column("Id_Pessoa_Anunciante")]
        public int IdPessoaAnunciante { get; set; }

        [Column("Id_Evento")]
        public int IdEvento { get; set; }

        #endregion
    }
}