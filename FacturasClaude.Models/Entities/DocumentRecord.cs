using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturasClaude.Models.Entities;

[Table("tDSVFACdocument")]
public class DocumentRecord
{
    public int Id { get; set; }

    
    public string FileName { get; set; } = string.Empty;

   
    public string MediaType { get; set; } = string.Empty;

    public long FileSize { get; set; }

   
    public DateTime UploadedAt { get; set; }
}
