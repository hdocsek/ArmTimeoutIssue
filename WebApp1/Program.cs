namespace WebApp1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args);

        builder.Services
            .AddLogging()
            .AddControllers();
        
        var app = builder.Build();
        
        app.MapControllers();
        app.MapGet("/", context =>
        {
            context.Response.Redirect("home");
            return Task.CompletedTask;
        });    
        app.Run();
    }
}
