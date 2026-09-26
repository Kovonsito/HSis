SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRANSACTION;

	IF OBJECT_ID(N'dbo.Notificacion', N'U') IS NULL
	BEGIN
		THROW 51000, 'No existe la tabla dbo.Notificacion.', 1;
	END;

	IF COL_LENGTH(N'dbo.Notificacion', N'id_Material') IS NULL
	BEGIN
		ALTER TABLE dbo.Notificacion
			ADD id_Material INT NULL;
	END;

	IF NOT EXISTS
	(
		SELECT 1
		FROM sys.foreign_keys
		WHERE parent_object_id = OBJECT_ID(N'dbo.Notificacion')
		  AND name = N'FK_Notificacion_Material'
	)
	   AND NOT EXISTS
	(
		SELECT 1
		FROM dbo.Notificacion AS n
		LEFT JOIN dbo.Material AS m ON m.id_Material = n.id_Material
		WHERE n.id_Material IS NOT NULL
		  AND m.id_Material IS NULL
	)
	BEGIN
		ALTER TABLE dbo.Notificacion
			ADD CONSTRAINT FK_Notificacion_Material
			FOREIGN KEY (id_Material)
			REFERENCES dbo.Material(id_Material)
			ON DELETE SET NULL;
	END;

	IF NOT EXISTS
	(
		SELECT 1
		FROM sys.indexes
		WHERE object_id = OBJECT_ID(N'dbo.Notificacion')
		  AND name = N'IX_Notificacion_Material'
	)
	BEGIN
		CREATE INDEX IX_Notificacion_Material
			ON dbo.Notificacion(id_Material);
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
