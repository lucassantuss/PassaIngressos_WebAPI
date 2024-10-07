using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassaIngressos_WebAPI.Entity
{
    [Table("Pessoa", Schema = "core")]
    public class Pessoa
    {
        [Key]
        [Column("Id_Pessoa")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPessoa { get; set; }

        [Column("Nome")]
        public string Nome { get; set; }

        [Column("Data_Nascimento")]
        public DateTime? DataNascimento { get; set; }

        [Column("CPF")]
        public string CPF { get; set; }

        [Column("RG")]
        public string RG { get; set; }

        [ForeignKey("IdTgSexo")]
        public ItemTabelaGeral Sexo { get; set; }

        [ForeignKey("IdArquivoFoto")]
        public Arquivo ArquivoFoto { get; set; }

        #region Foreign Keys

        [Column("Id_Tg_Sexo")]
        public int? IdTgSexo { get; set; }

        [Column("Id_Arquivo_Foto")]
        public int? IdArquivoFoto { get; set; }

        #endregion
    }
}