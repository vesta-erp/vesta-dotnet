namespace VestaNET.Application.DTOs;
public record TokenRequest(string? UserId, string? Email, string? Role);
public record TokenResponse(string Token, DateTime ExpiraEm, string Role);
