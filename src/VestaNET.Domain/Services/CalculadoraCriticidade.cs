using VestaNET.Domain.Entities;
using VestaNET.Domain.Enums;

namespace VestaNET.Domain.Services;

public class CalculadoraCriticidade
{
    public ResultadoCriticidade Calcular(Abrigo a)
    {
        var just = new List<string>();
        var recs = new List<RecomendacaoGerada>();
        var fatores = new List<FatorCalculo>();

        double sOcup = a.TaxaOcupacao switch
        {
            >= 1.0  => 100,
            >= 0.90 => 85,
            >= 0.75 => 60,
            >= 0.50 => 30,
            _       => 0
        };

        fatores.Add(new FatorCalculo(
            "Ocupação",
            0.40,
            Math.Round(a.TaxaOcupacao * 100, 1),
            Math.Round(sOcup * 0.40, 2),
            $"Banco: QT_OCUPACAO_ATUAL={a.OcupacaoAtual}, QT_CAPACIDADE_MAXIMA={a.CapacidadeMaxima}. " +
            $"Taxa={a.TaxaOcupacao:P0} → score bruto {sOcup} × peso 0,40 = {sOcup * 0.40:F2}"));

        if (a.TaxaOcupacao >= 0.90)
        {
            just.Add($"Ocupação crítica: {a.OcupacaoAtual}/{a.CapacidadeMaxima} ({a.TaxaOcupacao:P0}).");
            recs.Add(new(TipoRecomendacao.Transferencia,
                "Avaliar transferência de acolhidos para abrigos com capacidade disponível."));
        }
        else if (a.TaxaOcupacao >= 0.75)
            just.Add($"Ocupação elevada: {a.TaxaOcupacao:P0}.");

        int total = a.Estoques.Count;
        int abaixo = a.RecursosAbaixoMinimo;
        double sRec = total > 0 ? Math.Round((double)abaixo / total * 100, 2) : 0;
        var recursosCriticos = a.Estoques
            .Where(e => e.AbaixoDoMinimo)
            .Select(e => $"{e.NomeRecurso} (atual={e.QuantidadeAtual}, mín={e.QuantidadeMinima})")
            .ToList();

        fatores.Add(new FatorCalculo(
            "Recursos Abaixo do Mínimo",
            0.25,
            Math.Round(sRec, 1),
            Math.Round(sRec * 0.25, 2),
            $"Banco: TB_ESTOQUE_ABRIGO — {abaixo} de {total} recurso(s) com QT_ATUAL < QT_MINIMA. " +
            (recursosCriticos.Any()
                ? $"Críticos: {string.Join(", ", recursosCriticos)}. "
                : "Nenhum recurso crítico. ") +
            $"Score bruto {sRec:F1} × peso 0,25 = {sRec * 0.25:F2}"));

        if (abaixo > 0)
        {
            just.Add($"{abaixo} de {total} recurso(s) abaixo do estoque mínimo.");
            recs.Add(new(TipoRecomendacao.Reposicao,
                $"Repor urgentemente: {string.Join(", ", a.Estoques.Where(e => e.AbaixoDoMinimo).Select(e => e.NomeRecurso))}."));
        }

        var abertas = a.Ocorrencias.Where(o => o.Status != "RESOLVIDA").ToList();
        int nCrit  = abertas.Count(o => o.Severidade == "CRITICA");
        int nAlta  = abertas.Count(o => o.Severidade == "ALTA");
        int nMedia = abertas.Count(o => o.Severidade == "MEDIA");
        int nBaixa = abertas.Count(o => o.Severidade == "BAIXA");
        double pontos = nCrit * 40d + nAlta * 25d + nMedia * 12d + nBaixa * 5d;
        double sOcorr = Math.Min(100, pontos);

        fatores.Add(new FatorCalculo(
            "Ocorrências Abertas",
            0.25,
            Math.Round(sOcorr, 1),
            Math.Round(sOcorr * 0.25, 2),
            $"Banco: TB_OCORRENCIA (ST_STATUS ≠ RESOLVIDA) — " +
            $"Crítica={nCrit}×40pts, Alta={nAlta}×25pts, Média={nMedia}×12pts, Baixa={nBaixa}×5pts. " +
            $"Total={pontos:F0}pts (máx 100) → score bruto {sOcorr:F1} × peso 0,25 = {sOcorr * 0.25:F2}"));

        int graves = nCrit + nAlta;
        if (graves > 0)
        {
            just.Add($"{graves} ocorrência(s) grave(s) ou crítica(s) em aberto.");
            recs.Add(new(TipoRecomendacao.Operacional,
                $"Resolver {graves} ocorrência(s) de alta/crítica severidade com prioridade."));
        }
        else if (abertas.Count > 0)
            just.Add($"{abertas.Count} ocorrência(s) de baixa/média severidade em aberto.");

        int pend = a.SolicitacoesPendentes;
        double sSolic = Math.Min(100, pend * 25d);

        fatores.Add(new FatorCalculo(
            "Solicitações Pendentes",
            0.10,
            pend,
            Math.Round(sSolic * 0.10, 2),
            $"Banco: TB_SOLICITACAO_RECURSO (ST_STATUS IN ABERTA/EM_ANALISE/EM_ATENDIMENTO) — " +
            $"{pend} solicitação(ões) pendente(s). " +
            $"Score bruto {sSolic:F0} × peso 0,10 = {sSolic * 0.10:F2}"));

        if (pend > 0)
        {
            just.Add($"{pend} solicitação(ões) de recurso pendente(s).");
            recs.Add(new(TipoRecomendacao.Operacional,
                $"Atender {pend} solicitação(ões) pendente(s)."));
        }

        double score = Math.Clamp(
            Math.Round(sOcup * 0.40 + sRec * 0.25 + sOcorr * 0.25 + sSolic * 0.10, 2),
            0, 100);

        var nivel = score switch
        {
            >= 75 => NivelCriticidade.Critica,
            >= 50 => NivelCriticidade.Alta,
            >= 25 => NivelCriticidade.Media,
            _     => NivelCriticidade.Baixa
        };

        var explicacaoNivel = nivel switch
        {
            NivelCriticidade.Critica =>
                $"Score {score} ≥ 75 → CRÍTICA. Requer atenção imediata e prioridade máxima de recursos.",
            NivelCriticidade.Alta =>
                $"Score {score} entre 50 e 74 → ALTA. Situação preocupante, intervenção necessária em breve.",
            NivelCriticidade.Media =>
                $"Score {score} entre 25 e 49 → MÉDIA. Monitoramento ativo recomendado.",
            _ =>
                $"Score {score} < 25 → BAIXA. Abrigo dentro dos parâmetros operacionais normais."
        };

        if (nivel == NivelCriticidade.Critica)
            recs.Add(new(TipoRecomendacao.Prioridade,
                "Marcar como prioridade máxima no direcionamento de equipes e recursos."));

        if (just.Count == 0)
            just.Add("Abrigo dentro dos parâmetros operacionais normais.");

        return new(score, nivel, explicacaoNivel, fatores, just, recs,
            a.OcupacaoAtual, a.CapacidadeMaxima,
            total, abaixo, recursosCriticos.Select(r => r.Split(" (")[0]).ToList(),
            abertas.Count, nCrit, nAlta, nMedia, nBaixa, pend);
    }
}

public record FatorCalculo(
    string Fator, double Peso, double ValorBruto, double ScoreParcial, string Descricao);

public record RecomendacaoGerada(TipoRecomendacao Tipo, string Descricao);

public record ResultadoCriticidade(
    double Score,
    NivelCriticidade Nivel,
    string ExplicacaoNivel,
    List<FatorCalculo> Fatores,
    List<string> Justificativas,
    List<RecomendacaoGerada> Recomendacoes,
    int OcupacaoAtual,
    int CapacidadeMaxima,
    int TotalRecursos,
    int RecursosAbaixoMinimo,
    List<string> RecursosCriticos,
    int TotalOcorrenciasAbertas,
    int OcorrenciasCriticas,
    int OcorrenciasAltas,
    int OcorrenciasMedias,
    int OcorrenciasBaixas,
    int SolicitacoesPendentes);
