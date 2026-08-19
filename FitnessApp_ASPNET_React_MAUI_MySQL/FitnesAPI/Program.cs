using System.Text;
using FitnesAPI.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace FitnesAPI;

public class Program
{
    public const int Port = 2137;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        SetupCors(builder);
        SetupDatabase(builder);
        SetupJwt(builder);
        
        builder.Services.AddScoped<JwtHandler>();
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.WebHost.UseUrls($"http://*:{Port}");
        
        var app = builder.Build();
        app.UseCors();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }

    private static void SetupDatabase(WebApplicationBuilder builder)
    {
        string connectionStr = new DatabaseInfo().ToString();
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySql(connectionStr, ServerVersion.AutoDetect(connectionStr));
        });
    }

    private static void SetupCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });
    }

    private static void SetupJwt(WebApplicationBuilder builder)
    {
        var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false, 
                    ValidateAudience = false,
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}

readonly struct DatabaseInfo
{
    public DatabaseInfo(){}
    private readonly string _address = "127.0.0.1"; 
    private readonly int _port = 3306;
    private readonly string _dbName = "fitnesdb";
    private readonly string _user = "root";
    private readonly string _password = "";
    
    public override string ToString()
    {
        return $"server={_address};port={_port};database={_dbName};user={_user};password={_password}";
    }
}