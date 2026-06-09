namespace VestaNET.Application.Settings;

public class JavaApiSettings
{
    public string BaseUrl { get; set; } = "http://localhost:8080";
    public string ServiceEmail { get; set; } = string.Empty;
    public string ServicePassword { get; set; } = string.Empty;
}
