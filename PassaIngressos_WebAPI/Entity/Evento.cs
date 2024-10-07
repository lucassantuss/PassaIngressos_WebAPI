using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Evento", Schema = "venda")]
    public class Evento
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

        #region Foreign Keys

        [Column("Id_Arquivo_Evento")]
        public int? IdArquivoEvento { get; set; }

        #endregion
    }
}