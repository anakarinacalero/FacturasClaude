CREATE TABLE [dbo].[tDSVFACclaudeUsage]
(
    [id]                    INT             NOT NULL    IDENTITY(1,1),
    [document_id]           INT             NOT NULL,
    [message_id]            VARCHAR(100)    NULL,
    [request_id]            VARCHAR(100)    NULL,
    [model]                 VARCHAR(100)    NOT NULL,
    [stop_reason]           VARCHAR(50)     NULL,
    [input_tokens]          INT             NOT NULL    DEFAULT 0,
    [output_tokens]         INT             NOT NULL    DEFAULT 0,
    [cache_creation_tokens] INT             NOT NULL    DEFAULT 0,
    [cache_read_tokens]     INT             NOT NULL    DEFAULT 0,
    [elapsed_ms]            INT             NOT NULL,
    [created_at]            DATETIME2       NOT NULL    DEFAULT GETDATE(),

    CONSTRAINT [PK_tDSVFACclaudeUsage]
        PRIMARY KEY CLUSTERED ([id] ASC),

    CONSTRAINT [FK_tDSVFACclaudeUsage_document]
        FOREIGN KEY ([document_id])
        REFERENCES [dbo].[tDSVFACdocument] ([id])
)
