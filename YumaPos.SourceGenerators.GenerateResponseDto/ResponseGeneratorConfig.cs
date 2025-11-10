namespace YumaPos.SourceGenerators.GenerateResponseDto;

internal sealed record ResponseGeneratorConfig
{
    public string? OutputNamespace { get; set; }

    public static ResponseGeneratorConfig Default { get; } = new()
    {
        OutputNamespace = "FIXGENERATORCONFIG",
    };
}