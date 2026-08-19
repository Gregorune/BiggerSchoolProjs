using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnesAPI.Database;

[Table("users")]
public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }
    [Column("email", TypeName =  "varchar(255)")]
    public string? Email { get; set; }
    [Column("passhash", TypeName = "varchar(255)")]
    public string? PasswordHash { get; set; }
    [Column("username", TypeName = "varchar(50)")]
    public string? Username { get; set; }
    
    [Column("refresh_token")]
    public string? RefreshToken { get; set; }
    [Column("refresh_token_expires")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
}