public sealed class AddVisitDiagnosesRequest
{
    public List<DiagnosisRequest> Diagnoses { get; init; } = [];
}

public sealed class DiagnosisRequest
{
    public int DiagnosisId { get; init; }

    public List<string> IcdCodes { get; init; } = [];
}
