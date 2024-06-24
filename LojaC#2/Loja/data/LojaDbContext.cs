using Microsoft.EntityFrameworkCore;
using loja.models;
using Loja.models;

namespace loja.data
{
    public class LojaDbContext : DbContext
    {
        public LojaDbContext(DbContextOptions<LojaDbContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venda> Vendas { get; set; }

        // Sobrescreva o método OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define 'Id' como chave primária para a entidade 'Produto'
            modelBuilder.Entity<Produto>()
                .HasKey(p => p.Id);

            // Define 'Id' como chave primária para a entidade 'Cliente'
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.Id);

            //Define 'Id' como chave primária para a entidade 'Fornecedor'
            modelBuilder.Entity<Fornecedor>()
                 .HasKey(f => f.Id);

            //Define 'Id' como chave primária para a entidade 'Usuario'
            modelBuilder.Entity<Usuario>()
                 .HasKey(u => u.Id);
        }
    }
}