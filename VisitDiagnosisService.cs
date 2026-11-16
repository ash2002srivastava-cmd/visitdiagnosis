public sealed class VisitDiagnosisService
{
    private readonly ClinicalDbContext _dbContext;
    private readonly ILogger<VisitDiagnosisService> _logger;

    public VisitDiagnosisService(
        ClinicalDbContext dbContext,
        ILogger<VisitDiagnosisService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AddVisitDiagnosesResult> AddDiagnosesAsync(
        long visitId,
        AddVisitDiagnosesRequest request,
        string currentUser)
    {
        var visit = await _dbContext.Visits
            .SingleOrDefaultAsync(x => x.Id == visitId);

        if (visit is null)
        {
            return AddVisitDiagnosesResult.Failure(
                $"Visit {visitId} was not found.");
        }

        foreach (var diagnosisRequest in request.Diagnoses)
        {
            var masterDiagnosis = await _dbContext.MasterDiagnoses
                .SingleOrDefaultAsync(x =>
                    x.Id == diagnosisRequest.DiagnosisId &&
                    x.IsActive);

            if (masterDiagnosis is null)
            {
                return AddVisitDiagnosesResult.Failure(
                    $"Diagnosis {diagnosisRequest.DiagnosisId} is invalid.");
            }

            foreach (var icdCode in diagnosisRequest.IcdCodes)
            {
                var masterIcdCode = await _dbContext.MasterIcdCodes
                    .SingleOrDefaultAsync(x =>
                        x.MasterDiagnosisId == masterDiagnosis.Id &&
                        x.IcdCode == icdCode &&
                        x.IsActive);

                if (masterIcdCode is null)
                {
                    return AddVisitDiagnosesResult.Failure(
                        $"ICD code {icdCode} is invalid.");
                }

                var visitDiagnosis = new VisitDiagnosis
                {
                    VisitId = visitId,
                    MasterDiagnosisId = masterDiagnosis.Id,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = currentUser
                };

                _dbContext.VisitDiagnoses.Add(visitDiagnosis);

                await _dbContext.SaveChangesAsync();

                var visitDiagnosisIcdCode =
                    new VisitDiagnosisIcdCode
                    {
                        VisitDiagnosisId = visitDiagnosis.Id,
                        MasterIcdCodeId = masterIcdCode.Id,
                        CreatedAtUtc = DateTime.UtcNow
                    };

                _dbContext.VisitDiagnosisIcdCodes.Add(
                    visitDiagnosisIcdCode);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Added diagnosis {DiagnosisId} and ICD code {IcdCode} " +
                    "to visit {VisitId}",
                    masterDiagnosis.Id,
                    masterIcdCode.IcdCode,
                    visitId);
            }
        }

        return AddVisitDiagnosesResult.Success(
            visitId,
            request.Diagnoses.Count);
    }
}
