using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace FitnesAPI.Database;

[Table("classes")]
public class Class
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    [Column("name", TypeName = "varchar(255)")]
    public string? Name { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    [Column("instructor", TypeName = "varchar(255)")]
    public string? Instructor { get; set; }
    [Column("starts_at")]
    public DateTime StartsAt { get; set; }
    [Column("people_limit")]
    public int? PeopleLimit { get; set; }

    [Column("repetition", TypeName = "varchar(50)")]
    public ClassRepetition Repetition { get; set; } = ClassRepetition.None;
}

public enum ClassRepetition
{
    None,
    Daily,
    Weekly,
    Every2Weeks,
    Monthly,
}