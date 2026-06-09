using Microsoft.EntityFrameworkCore;
using VestaNET.Domain.Entities;
using VestaNET.Domain.Enums;

namespace VestaNET.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AnaliseCriticidade> Analises => Set<AnaliseCriticidade>();
    public DbSet<Recomendacao> Recomendacoes => Set<Recomendacao>();

    private bool IsRelational => Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory";

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<AnaliseCriticidade>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Nivel)
                .HasConversion(v => v.ToString().ToUpper(), v => Enum.Parse<NivelCriticidade>(v, true));
            e.HasMany(x => x.Recomendacoes).WithOne()
                .HasForeignKey(r => r.AnaliseCriticidadeId)
                .OnDelete(DeleteBehavior.Cascade);
            if (IsRelational)
            {
                e.ToTable("TB_NET_ANALISE");
                e.Property(x => x.Id).HasColumnName("ID_ANALISE").UseIdentityColumn();
                e.Property(x => x.AbrigoId).HasColumnName("ID_ABRIGO");
                e.Property(x => x.Score).HasColumnName("VL_SCORE").HasColumnType("NUMBER(5,2)");
                e.Property(x => x.Nivel).HasColumnName("TP_NIVEL").HasMaxLength(20);
                e.Property(x => x.Justificativa).HasColumnName("DS_JUSTIFICATIVA").HasMaxLength(2000);
                e.Property(x => x.DataAnalise).HasColumnName("DT_ANALISE");
            }
        });

        mb.Entity<Recomendacao>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Tipo)
                .HasConversion(v => v.ToString().ToUpper(), v => Enum.Parse<TipoRecomendacao>(v, true));
            if (IsRelational)
            {
                e.ToTable("TB_NET_RECOMENDACAO");
                e.Property(x => x.Id).HasColumnName("ID_RECOMENDACAO").UseIdentityColumn();
                e.Property(x => x.AnaliseCriticidadeId).HasColumnName("ID_ANALISE");
                e.Property(x => x.Tipo).HasColumnName("TP_RECOMENDACAO").HasMaxLength(30);
                e.Property(x => x.Descricao).HasColumnName("DS_DESCRICAO").HasMaxLength(500);
            }
        });
    }
}
