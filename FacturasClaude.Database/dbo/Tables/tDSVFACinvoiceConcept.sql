CREATE TABLE [dbo].[tDSVFACinvoiceConcept]
(
    [id]                INT            NOT NULL IDENTITY(1,1),
    [extracted_data_id] INT            NOT NULL,
    [description]       NVARCHAR(2000) NULL,
    [quantity]          DECIMAL(18,4)  NULL,
    [unit_of_measure]   NVARCHAR(50)   NULL,
    [unit_price]        DECIMAL(18,4)  NULL,
    [subtotal]          DECIMAL(18,2)  NULL,

    CONSTRAINT [PK_tDSVFACinvoiceConcept] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_tDSVFACinvoiceConcept_tDSVFACextractedData]
        FOREIGN KEY ([extracted_data_id]) REFERENCES [dbo].[tDSVFACextractedData] ([id])
)
