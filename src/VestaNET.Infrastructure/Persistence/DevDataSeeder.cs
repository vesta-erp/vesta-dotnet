using VestaNET.Domain.Entities;
namespace VestaNET.Infrastructure.Persistence;

public static class DevDataSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Abrigos.Any()) return;
        db.Recursos.AddRange(
            new Recurso { Id=1, Nome="Água Mineral (L)",    UnidadeMedida="L"   },
            new Recurso { Id=2, Nome="Alimento (kg)",       UnidadeMedida="kg"  },
            new Recurso { Id=3, Nome="Cobertor",            UnidadeMedida="un"  },
            new Recurso { Id=4, Nome="Kit Higiene",         UnidadeMedida="kit" },
            new Recurso { Id=5, Nome="Medicamento Básico",  UnidadeMedida="cx"  }
        );
        db.Abrigos.AddRange(
            new Abrigo { Id=1, Nome="Ginásio Municipal Centro",  Endereco="Av. Principal, 100",  CapacidadeMaxima=200, OcupacaoAtual=195, Status="LOTADO",   RegiaoId=1, InstituicaoId=1 },
            new Abrigo { Id=2, Nome="Escola Estadual Norte",     Endereco="Rua das Flores, 250", CapacidadeMaxima=150, OcupacaoAtual=60,  Status="ATIVO",    RegiaoId=1, InstituicaoId=1 },
            new Abrigo { Id=3, Nome="Centro Comunitário Leste",  Endereco="Rua do Lago, 80",     CapacidadeMaxima=100, OcupacaoAtual=92,  Status="ATIVO",    RegiaoId=2, InstituicaoId=2 },
            new Abrigo { Id=4, Nome="Clube Esportivo Sul",       Endereco="Av. do Esporte, 300", CapacidadeMaxima=300, OcupacaoAtual=40,  Status="ATIVO",    RegiaoId=2, InstituicaoId=2 },
            new Abrigo { Id=5, Nome="Igreja Matriz Oeste",       Endereco="Praça Central, 1",    CapacidadeMaxima=80,  OcupacaoAtual=78,  Status="ATIVO",    RegiaoId=3, InstituicaoId=3 }
        );
        db.Estoques.AddRange(
            new EstoqueAbrigo { Id=1,  AbrigoId=1, RecursoId=1, QuantidadeAtual=50,  QuantidadeMinima=400 },
            new EstoqueAbrigo { Id=2,  AbrigoId=1, RecursoId=2, QuantidadeAtual=20,  QuantidadeMinima=200 },
            new EstoqueAbrigo { Id=3,  AbrigoId=1, RecursoId=3, QuantidadeAtual=80,  QuantidadeMinima=200 },
            new EstoqueAbrigo { Id=4,  AbrigoId=1, RecursoId=4, QuantidadeAtual=10,  QuantidadeMinima=100 },
            new EstoqueAbrigo { Id=5,  AbrigoId=2, RecursoId=1, QuantidadeAtual=300, QuantidadeMinima=150 },
            new EstoqueAbrigo { Id=6,  AbrigoId=2, RecursoId=2, QuantidadeAtual=200, QuantidadeMinima=100 },
            new EstoqueAbrigo { Id=7,  AbrigoId=3, RecursoId=1, QuantidadeAtual=100, QuantidadeMinima=200 },
            new EstoqueAbrigo { Id=8,  AbrigoId=3, RecursoId=5, QuantidadeAtual=5,   QuantidadeMinima=50  },
            new EstoqueAbrigo { Id=9,  AbrigoId=4, RecursoId=1, QuantidadeAtual=600, QuantidadeMinima=300 },
            new EstoqueAbrigo { Id=10, AbrigoId=4, RecursoId=2, QuantidadeAtual=500, QuantidadeMinima=200 },
            new EstoqueAbrigo { Id=11, AbrigoId=5, RecursoId=1, QuantidadeAtual=30,  QuantidadeMinima=160 },
            new EstoqueAbrigo { Id=12, AbrigoId=5, RecursoId=2, QuantidadeAtual=15,  QuantidadeMinima=80  }
        );
        db.Ocorrencias.AddRange(
            new Ocorrencia { Id=1, AbrigoId=1, Titulo="Falta de energia",       Severidade="ALTA",   Status="ABERTA"      },
            new Ocorrencia { Id=2, AbrigoId=1, Titulo="Caso suspeito de sarampo", Severidade="CRITICA",Status="ABERTA"    },
            new Ocorrencia { Id=3, AbrigoId=3, Titulo="Banheiros em manutenção", Severidade="MEDIA",  Status="ABERTA"     },
            new Ocorrencia { Id=4, AbrigoId=5, Titulo="Falta de água encanada",  Severidade="ALTA",   Status="ABERTA"     },
            new Ocorrencia { Id=5, AbrigoId=5, Titulo="Superlotação iminente",   Severidade="ALTA",   Status="EM_ANDAMENTO"},
            new Ocorrencia { Id=6, AbrigoId=2, Titulo="Lâmpada queimada",        Severidade="BAIXA",  Status="RESOLVIDA"  }
        );
        db.Solicitacoes.AddRange(
            new SolicitacaoRecurso { Id=1, AbrigoId=1, RecursoId=1, Quantidade=500, Status="ABERTA"       },
            new SolicitacaoRecurso { Id=2, AbrigoId=1, RecursoId=2, Quantidade=200, Status="EM_ANALISE"   },
            new SolicitacaoRecurso { Id=3, AbrigoId=1, RecursoId=4, Quantidade=100, Status="EM_ATENDIMENTO"},
            new SolicitacaoRecurso { Id=4, AbrigoId=3, RecursoId=1, Quantidade=200, Status="ABERTA"       },
            new SolicitacaoRecurso { Id=5, AbrigoId=5, RecursoId=2, Quantidade=80,  Status="ABERTA"       },
            new SolicitacaoRecurso { Id=6, AbrigoId=2, RecursoId=3, Quantidade=50,  Status="CONCLUIDA"    }
        );
        db.SaveChanges();
    }
}
