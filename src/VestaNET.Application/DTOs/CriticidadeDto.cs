namespace VestaNET.Application.DTOs;

public record CriticidadeDto(
    long IdAbrigo,
    int Score,
    string Nivel,
    string Justificativa,
    List<string> Recomendacoes);
