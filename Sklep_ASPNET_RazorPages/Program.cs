using Microsoft.EntityFrameworkCore;
using MyApi.Database;

namespace MyApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=data.db"));

        /*var serverVersion = new MySqlServerVersion(new Version(10, 4, 32));
        var connectionString = "server=localhost;user=root;password=;database=sklep";
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion);
        });*/

        builder.Services.AddSingleton<RuntimeStorageService>();
        
        var app = builder.Build();

        //init db if not created
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        app.UseStaticFiles();

        app.MapControllers();

        app.Run();
    }
}