using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Database
{
    public class DbPassaIngressos : DbContext
    {
        public DbPassaIngressos (DbContextOptions<DbPassaIngressos> options): base(options)
        {

        }

        public DbSet<Arquivo> Arquivos { get; set; }
        public DbSet<TabelaGeral> TabelasGerais { get; set; }
        public DbSet<ItemTabelaGeral> ItensTabelaGeral { get; set; }

        public DbSet<Pessoa> Pessoas { get; set; }
        
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; }
        
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Evento
            modelBuilder.Entity<Evento>()
                        .HasOne(u => u.ArquivoEvento)
                        .WithMany()
                        .HasForeignKey(u => u.IdArquivoEvento);
            #endregion

            #region Feedback
            modelBuilder.Entity<Feedback>()
                        .HasOne(u => u.Pessoa)
                        .WithMany()
                        .HasForeignKey(u => u.IdPessoa);
            #endregion

            #region Ingresso
            modelBuilder.Entity<Ingresso>()
                        .HasOne(u => u.TipoIngresso)
                        .WithMany()
                        .HasForeignKey(u => u.IdTgTipoIngresso);

            modelBuilder.Entity<Ingresso>()
                        .HasOne(u => u.PessoaAnunciante)
                        .WithMany()
                        .HasForeignKey(u => u.IdPessoaAnunciante);

            modelBuilder.Entity<Ingresso>()
                        .HasOne(u => u.Evento)
                        .WithMany()
                        .HasForeignKey(u => u.IdEvento);
            #endregion

            #region ItemTabelaGeral
            modelBuilder.Entity<ItemTabelaGeral>()
                        .HasOne(u => u.TabelaGeral)
                        .WithMany()
                        .HasForeignKey(u => u.IdTabelaGeral);
            #endregion

            #region Pessoa
            modelBuilder.Entity<Pessoa>()
                        .HasOne(u => u.Sexo)
                        .WithMany()
                        .HasForeignKey(u => u.IdTgSexo);

            modelBuilder.Entity<Pessoa>()
                        .HasOne(u => u.ArquivoFoto)
                        .WithMany()
                        .HasForeignKey(u => u.IdArquivoFoto);
            #endregion

            #region Usuario
            modelBuilder.Entity<Usuario>()
                        .HasOne(u => u.Pessoa)
                        .WithMany()
                        .HasForeignKey(u => u.IdPessoa);
            #endregion

            #region UsuarioPerfil
            modelBuilder.Entity<UsuarioPerfil>()
                        .HasOne(u => u.Usuario)
                        .WithMany()
                        .HasForeignKey(u => u.IdUsuario);

            modelBuilder.Entity<UsuarioPerfil>()
                        .HasOne(u => u.Perfil)
                        .WithMany()
                        .HasForeignKey(u => u.IdPerfil);
            #endregion
        }
    }
}