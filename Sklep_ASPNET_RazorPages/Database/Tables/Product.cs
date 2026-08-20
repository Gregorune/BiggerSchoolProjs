using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyApi.Database.Tables;

public class Product
{
    [JsonPropertyName("id")]
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get;set;}

    [JsonPropertyName("imageUrl")]
    [Column("imageUrl")]
    public string ImageUrl {get;set;} = "";

    [JsonPropertyName("title")]
    [Column("title")]
    public string Title {get;set;}= "";

    [JsonPropertyName("description")]
    [Column("description")]
    public string Description {get;set;} = "";

    [JsonPropertyName("price")]
    [Column("price")]
    public double Price {get;set;}
}