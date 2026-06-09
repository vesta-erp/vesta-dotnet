using Microsoft.EntityFrameworkCore;
using VestaNET.Domain.Entities;
using VestaNET.Domain.Enums;

namespace VestaNET.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Abrigo> Abrigos => Set<Abrigo>();
    public DbSet<Recurso> Recursos => Set<Recurso>();
    public DbSet<EstoqueAbrigo> Estoques => Set<EstoqueAbrigo>();
    public DbSet<Ocorrencia> Ocorrencias => Set<Ocorrencia>();
    public DbSet<SolicitacaoRecurso> Solicitacoes => Set<SolicitacaoRecurso>();
    public DbSet<AnaliseCriticidade> Analises => Set<AnaliseCriticidade>();
    public DbSet<Recomendacao> Recomendacoes => Set<Recomendacao>();

    private bool IsRelational => Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory";

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Abrigo>(e =>
        {
            e.HasKey(x => x.Id);
            e.Ignore(x => x.TaxaOcupacao);
            e.Ignore(x => x.OcorrenciasAbertas);
            e.Ignore(x => x.SolicitacoesPendentes);
            e.Ignore(x => x.RecursosAbaixoMinimo);
            if (IsRelational)
            {
                e.ToTable("TB_ABRIGO", t => t.ExcludeFromMigrations());
                e.Property(x => x.Id).HasColumnName("ID_ABRIGO");
                e.Property(x => x.Nome).HasColumnName("NM_ABRIGO");
                e.Property(x => x.Endereco).HasColumnName("DS_ENDERECO");
                e.Property(x => x.CapacidadeMaxima).HasColumnName("QT_CAPACIDADE_MAXIMA");
                e.Property(x => x.OcupacaoAtual).HasColumnName("QT_OCUPACAO_ATUAL");
                e.Property(x => x.Status).HasColumnName("ST_STATUS");
                e.Property(x => x.RegiaoId).HasColumnName("ID_REGIAO");
                e.Property(x => x.InstituicaoId).HasColumnName("ID_INSTITUICAO");
            }
        });

        mb.Entity<Recurso>(e =>
        {
            e.HasKey(x => x.Id);
            if (IsRelational)
            {
                e.ToTable("TB_RECURSO", t => t.ExcludeFromMigrations());
                e.Property(x => x.Id).HasColumnName("ID_RECURSO");
                e.Property(x => x.Nome).HasColumnName("NM_RECURSO");
                e.Property(x => x.UnidadeMedida).HasColumnName("DS_UNIDADE_MEDIDA");
            }
        });

        mb.Entity<EstoqueAbrigo>(e =>
        {
            e.HasKey(x => x.Id);
            e.Ignore(x => x.NomeRecurso);
            e.Ignore(x => x.AbaixoDoMinimo);
            e.HasOne(x => x.Recurso).WithMany().HasForeignKey(x => x.RecursoId);
            if (IsRelational)
            {
                e.ToTable("TB_ESTOQUE_ABRIGO", t => t.ExcludeFromMigrations());
                e.Property(x => x.Id).HasColumnName("ID_ESTOQUE");
                e.Property(x => x.AbrigoId).HasColumnName("ID_ABRIGO");
                e.Property(x => x.RecursoId).HasColumnName("ID_RECURSO");
                e.Property(x => x.QuantidadeAtual).HasColumnName("QT_ATUAL");
                e.Property(x => x.QuantidadeMinima).HasColumnName("QT_MINIMA");
            }
        });

        mb.Entity<Ocorrencia>(e =>
        {
            e.HasKey(x => x.Id);
            if (IsRelational)
            {
                e.ToTable("TB_OCORRENCIA", t => t.ExcludeFromMigrations());
                e.Property(x => x.Id).HasColumnName("ID_OCORRENCIA");
                e.Property(x => x.AbrigoId).HasColumnName("ID_ABRIGO");
                e.Property(x => x.Titulo).HasColumnName("NM_TITULO");
                e.Property(x => x.Severidade).HasColumnName("TP_SEVERIDADE");
                e.Property(x => x.Status).HasColumnName("ST_STATUS");
            }
        });

        mb.Entity<SolicitacaoRecurso>(e =>
        {
            e.HasKey(x => x.Id);
            if (IsRelational)
            {
                e.ToTable("TB_SOLICITACAO_RECURSO", t => t.ExcludeFromMigrations());
                e.Property(x => x.Id).HasColumnName("ID_SOLICITACAO");
                e.Property(x => x.AbrigoId).HasColumnName("ID_ABRIGO");
                e.Property(x => x.RecursoId).HasColumnName("ID_RECURSO");
                e.Property(x => x.Quantidade).HasColumnName("QT_SOLICITADA");
                e.Property(x => x.Status).HasColumnName("ST_STATUS");
            }
        });

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
