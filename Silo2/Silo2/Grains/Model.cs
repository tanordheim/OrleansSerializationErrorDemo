using Abstractions;

namespace Silo2.Grains;

[GenerateSerializer]
[Immutable]
[Alias("Silo2_Model")]
public sealed record Model : BaseModel;