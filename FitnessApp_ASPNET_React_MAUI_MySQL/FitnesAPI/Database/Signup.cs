using System.ComponentModel.DataAnnotations.Schema;

namespace FitnesAPI.Database;

[Table("signups")]
public class Signup
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("uid")]
    public int UserId { get; set; }
    [Column("cid")]
    public int ClassId { get; set; }
}