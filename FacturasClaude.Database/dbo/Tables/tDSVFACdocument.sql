CREATE TABLE [dbo].[tDSVFACdocument]
(
    [id]          INT           NOT NULL IDENTITY(1,1),
    [file_name]   NVARCHAR(500) NOT NULL,
    [media_type]  NVARCHAR(100) NOT NULL,
    [file_size]   BIGINT        NOT NULL,
    [uploaded_at] DATETIME2     NOT NULL CONSTRAINT [DF_tDSVFACdocument_uploaded_at] DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_tDSVFACdocument] PRIMARY KEY CLUSTERED ([id] ASC)
)
