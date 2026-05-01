namespace FinanzasMCP.Application.Tags.Commands;

public sealed record UpdateTagCommand(Guid Id, string Name, string? Color);
