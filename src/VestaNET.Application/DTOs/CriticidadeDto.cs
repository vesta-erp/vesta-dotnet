namespace VestaNET.Application.DTOs;

public record CriticidadeDto(
    long IdAbrigo,
    double Score,
    string Nivel,
    string Justificativa,
    List<string> Recomendacoes);
