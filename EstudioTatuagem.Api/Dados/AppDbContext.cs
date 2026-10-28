using Microsoft.EntityFrameworkCore;
using EstudioTatuagem.Api.Modelos;

namespace EstudioTatuagem.Api.Dados;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Tatuador> Tatuadores => Set<Tatuador>();
    public DbSet<Sessao> Sessoes => Set<Sessao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tatuador>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Nome).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Especialidade).IsRequired().HasMaxLength(100);
            builder.Property(t => t.ValorHora).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Sessao>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.DescricaoTatuagem).IsRequired().HasMaxLength(500);

            builder.HasOne(s => s.Tatuador)
                   .WithMany(t => t.Sessoes)
                   .HasForeignKey(s => s.TatuadorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Cliente)
                   .WithMany()
                   .HasForeignKey(s => s.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Usuario>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        });
    }
}