CREATE TABLE [dbo].[tDSVFACextractedData]
(
    [id]              INT            NOT NULL IDENTITY(1,1),
    [document_id]     INT            NOT NULL,
    [invoice_number]  NVARCHAR(100)  NULL,
    [issue_date]      DATE           NULL,
    [total_amount]    DECIMAL(18,2)  NULL,
    [currency]        NCHAR(3)       NULL,
    [supplier]        NVARCHAR(500)  NULL,
    [client_address]  NVARCHAR(500)  NULL,
    [client_zip_code] NVARCHAR(20)   NULL,
    [description]     NVARCHAR(2000) NULL,
    [taxes]           DECIMAL(18,2)  NULL,

    CONSTRAINT [PK_tDSVFACextractedData]              PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_tDSVFACextractedData_tDSVFACdocument]
        FOREIGN KEY ([document_id]) REFERENCES [dbo].[tDSVFACdocument] ([id])
)
