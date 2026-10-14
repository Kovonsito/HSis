IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OperacionesIdempotentes')
BEGIN
    CREATE TABLE OperacionesIdempotentes (
        IdOperacion UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        IdUsuario INT NOT NULL,
        RutaEndpoint NVARCHAR(250) NOT NULL,
        MetodoHttp NVARCHAR(10) NOT NULL,
        CodigoEstadoHttp INT NOT NULL,
        RespuestaJson NVARCHAR(MAX) NULL,
        FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    CREATE NONCLUSTERED INDEX IX_OperacionesIdempotentes_FechaCreacion 
    ON OperacionesIdempotentes (FechaCreacion);
END
