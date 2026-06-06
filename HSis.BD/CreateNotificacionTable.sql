CREATE TABLE [dbo].[Notificacion] (
    [id_Notificacion] INT IDENTITY(1,1) NOT NULL,
    [usuario_Destino_Id] INT NOT NULL,
    [Mensaje] VARCHAR(500) NOT NULL,
    [Tipo] VARCHAR(50) NOT NULL,
    [Leido] BIT NOT NULL CONSTRAINT [DF_Notificacion_Leido] DEFAULT (0),
    [FechaCreacion] DATETIME NOT NULL CONSTRAINT [DF_Notificacion_FechaCreacion] DEFAULT (GETDATE()),
    CONSTRAINT [PK_Notificacion] PRIMARY KEY CLUSTERED ([id_Notificacion] ASC),
    CONSTRAINT [FK_Notificacion_Usuario] FOREIGN KEY ([usuario_Destino_Id]) REFERENCES [dbo].[Usuario] ([id_Usuario]) ON DELETE CASCADE
);
GO
