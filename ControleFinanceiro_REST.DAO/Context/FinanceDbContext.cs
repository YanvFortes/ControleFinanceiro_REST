using ControleFinanceiro_REST.DAO.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro_REST.DAO.Context;

public partial class FinanceDbContext : DbContext
{
    public FinanceDbContext()
    {
    }

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Transacao> Transacoes { get; set; }

    public virtual DbSet<Tipousuario> Tipousuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=financeDb;Username=postgres;Password=postgres;Ssl Mode=Disable;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetRoles_pkey");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetRoleClaims_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("AspNetRoleClaims_RoleId_fkey");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUsers_pkey");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("AspNetUserRoles_RoleId_fkey"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("AspNetUserRoles_UserId_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("AspNetUserRoles_pkey");
                        j.ToTable("AspNetUserRoles");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUserClaims_pkey");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("AspNetUserClaims_UserId_fkey");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("AspNetUserLogins_pkey");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("AspNetUserLogins_UserId_fkey");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("AspNetUserTokens_pkey");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("AspNetUserTokens_UserId_fkey");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Descricao)
                .HasMaxLength(400)
                .IsRequired();

            entity.Property(e => e.Finalidade)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => new { e.UsuarioId, e.Descricao });

            entity.HasMany(e => e.Transacoes)
                .WithOne(t => t.Categoria)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.ToTable("Pessoas");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nome)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => new { e.UsuarioId, e.Nome });

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Pessoas)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Transacoes)
                .WithOne(t => t.Pessoa)
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.ToTable("Transacoes");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Descricao)
                .HasMaxLength(400)
                .IsRequired();

            entity.Property(e => e.Valor)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Tipo)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.PessoaId);
            entity.HasIndex(e => e.CategoriaId);

            entity.HasOne(e => e.Pessoa)
                .WithMany(p => p.Transacoes)
                .HasForeignKey(e => e.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Transacoes)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nome)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasOne(e => e.TipoUsuario)
                .WithMany(t => t.Usuarios)
                .HasForeignKey(e => e.TipoUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Usuario>(e => e.AspNetUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Categorias)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Transacoes)
                .WithOne(t => t.Usuario)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}