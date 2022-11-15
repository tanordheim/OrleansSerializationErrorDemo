namespace Abstractions;

[GenerateSerializer]
[Immutable]
[Alias("Abstractions_BaseModel")]
public abstract record BaseModel
{
    [Id(0)] public string AccessToken { get; init; } = "";
    [Id(1)] public string RefreshToken { get; init; } = "";
    [Id(2)] public string TokenType { get; init; } = "Bearer";
    [Id(3)] public string Scope { get; init; } = "";
    [Id(4)] public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    [Id(5)] public DateTime ExpiresAt { get; init; }
}