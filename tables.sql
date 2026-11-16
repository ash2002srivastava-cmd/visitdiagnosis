CREATE TABLE Visit
(
    Id                  BIGINT          NOT NULL PRIMARY KEY,
    PatientId           BIGINT          NOT NULL,
    VisitStatus         VARCHAR(30)     NOT NULL,
    CreatedAtUtc        DATETIME2       NOT NULL,
    UpdatedAtUtc        DATETIME2       NULL,
    RowVersion          ROWVERSION      NOT NULL
);

CREATE TABLE MasterDiagnosis
(
    Id                  INT             NOT NULL PRIMARY KEY,
    DiagnosisName       VARCHAR(200)    NOT NULL,
    IsActive            BIT             NOT NULL
);

CREATE TABLE MasterIcdCode
(
    Id                  INT             NOT NULL PRIMARY KEY,
    IcdCode             VARCHAR(20)     NOT NULL,
    Description         VARCHAR(500)    NULL,
    IsActive            BIT             NOT NULL,

   
);

CREATE TABLE VisitDiagnosis
(
    Id                  BIGINT          IDENTITY PRIMARY KEY,
    VisitId             BIGINT          NOT NULL,
    MasterDiagnosisId   INT             NOT NULL,
    CreatedAtUtc        DATETIME2       NOT NULL,
    CreatedBy           VARCHAR(100)    NOT NULL,

    CONSTRAINT FK_VisitDiagnosis_Visit
        FOREIGN KEY (VisitId)
        REFERENCES Visit(Id),

    CONSTRAINT FK_VisitDiagnosis_MasterDiagnosis
        FOREIGN KEY (MasterDiagnosisId)
        REFERENCES MasterDiagnosis(Id),

    CONSTRAINT UQ_VisitDiagnosis
        UNIQUE (VisitId, MasterDiagnosisId)
);

CREATE TABLE VisitDiagnosisIcdCode
(
    Id                  BIGINT          IDENTITY PRIMARY KEY,
    VisitDiagnosisId    BIGINT          NOT NULL,
    MasterIcdCodeId     INT             NOT NULL,
    CreatedAtUtc        DATETIME2       NOT NULL,

    CONSTRAINT FK_VisitDiagnosisIcdCode_VisitDiagnosis
        FOREIGN KEY (VisitDiagnosisId)
        REFERENCES VisitDiagnosis(Id),

    CONSTRAINT FK_VisitDiagnosisIcdCode_MasterIcdCode
        FOREIGN KEY (MasterIcdCodeId)
        REFERENCES MasterIcdCode(Id),

    CONSTRAINT UQ_VisitDiagnosisIcdCode
        UNIQUE (VisitDiagnosisId, MasterIcdCodeId)
);
