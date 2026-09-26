SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	IF OBJECT_ID(N'dbo.Notificacion', N'U') IS NULL
	BEGIN
		THROW 51000, 'No existe la tabla dbo.Notificacion.', 1;
	END;

	IF COL_LENGTH(N'dbo.Notificacion', N'id_Ticket') IS NULL
	BEGIN
		ALTER TABLE dbo.Notificacion
			ADD id_Ticket INT NULL;
	END;

	;WITH Candidatos AS
	(
		SELECT
			n.id_Notificacion,
			LTRIM(SUBSTRING(
				n.Mensaje,
				PATINDEX('%ticket [0-9]%', LOWER(n.Mensaje)) + LEN('ticket '),
				32)) AS TextoTicket
		FROM dbo.Notificacion AS n
		WHERE n.id_Ticket IS NULL
		  AND PATINDEX('%ticket [0-9]%', LOWER(n.Mensaje)) > 0
	), Numeros AS
	(
		SELECT
			id_Notificacion,
			LEFT(TextoTicket, CASE
				WHEN PATINDEX('%[^0-9]%', TextoTicket) = 0 THEN LEN(TextoTicket)
				ELSE PATINDEX('%[^0-9]%', TextoTicket) - 1
			END) AS NumeroTicket
		FROM Candidatos
	)
	UPDATE n
	SET n.id_Ticket = TRY_CONVERT(INT, NULLIF(numeros.NumeroTicket, ''))
	FROM dbo.Notificacion AS n
	INNER JOIN Numeros AS numeros
		ON numeros.id_Notificacion = n.id_Notificacion
	WHERE n.id_Ticket IS NULL;

	IF NOT EXISTS
	(
		SELECT 1
		FROM sys.indexes
		WHERE object_id = OBJECT_ID(N'dbo.Notificacion')
		  AND name = N'IX_Notificacion_Usuario_Leido_Fecha'
	)
	BEGIN
		CREATE INDEX IX_Notificacion_Usuario_Leido_Fecha
			ON dbo.Notificacion(usuario_Destino_Id, Leido, FechaCreacion);
	END;

	IF NOT EXISTS
	(
		SELECT 1
		FROM sys.foreign_keys
		WHERE parent_object_id = OBJECT_ID(N'dbo.Notificacion')
		  AND name = N'FK_Notificacion_Ticket'
	)
	   AND NOT EXISTS
	(
		SELECT 1
		FROM dbo.Notificacion AS n
		LEFT JOIN dbo.Ticket AS t ON t.id_Ticket = n.id_Ticket
		WHERE n.id_Ticket IS NOT NULL
		  AND t.id_Ticket IS NULL
	)
	BEGIN
		ALTER TABLE dbo.Notificacion
			ADD CONSTRAINT FK_Notificacion_Ticket
			FOREIGN KEY (id_Ticket)
			REFERENCES dbo.Ticket(id_Ticket)
			ON DELETE SET NULL;
	END;

	COMMIT TRANSACTION;
END TRY
BEGIN CATCH
	IF XACT_STATE() <> 0
	BEGIN
		ROLLBACK TRANSACTION;
	END;

	THROW;
END CATCH;
GO
