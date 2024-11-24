using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("IngressoAUXILIAR", Schema = "venda")]
    public class IngressoAUXILIAR
    {
        [Key]
        [Column("Id_Evento")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEvento { get; set; }

        [Column("Nome_Evento")]
        public string NomeEvento { get; set; }

        [Column("Local_Evento")]
        public string LocalEvento { get; set; }

        [Column("Data_Hora_Evento")]
        public DateTime? DataHoraEvento { get; set; }

        [Column("IdArquivoEvento")]
        public Arquivo ArquivoEvento { get; set; }

        [Column("IdTgTipoIngresso")]
        public ItemTabelaGeral TipoIngresso { get; set; }

        [Column("IdPessoaAnunciante")]
        public Pessoa PessoaAnunciante { get; set; }

        [Column("Valor")]
        public decimal? Valor { get; set; }

        #region Foreign Keys

        [Column("Id_Tg_Tipo_Ingresso")]
        public int IdTgTipoIngresso { get; set; }

        [Column("Id_Pessoa_Anunciante")]
        public int IdPessoaAnunciante { get; set; }

        [Column("Id_Arquivo_Evento")]
        public int? IdArquivoEvento { get; set; }

        #endregion
    }
}