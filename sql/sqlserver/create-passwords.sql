-- Microsoft SQL Server
-- starttime lagras i lokal svensk tid (Europe/Stockholm).
CREATE TABLE dbo.Passwords (
    campus    nvarchar(100) NOT NULL,
    password  nvarchar(100) NOT NULL,
    starttime datetime      NOT NULL
);

CREATE INDEX IX_Passwords_campus_starttime ON dbo.Passwords (campus, starttime);

-- Exempeldata
INSERT INTO dbo.Passwords (campus, password, starttime)
VALUES (N'Ultuna', N'Tenta-Ultuna-42', DATEADD(minute, 10, GETDATE()));
