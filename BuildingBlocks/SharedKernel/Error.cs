namespace SharedKernel;

/// <summary>
/// Domain veya application seviyesinde oluşan
/// hata bilgisini temsil eder.
/// </summary>
public sealed record Error(string Code, string Message);
