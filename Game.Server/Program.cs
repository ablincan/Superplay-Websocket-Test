
using Game.Core.Interfaces;
using Game.Infrastructure.Database.Context;
using Game.Infrastructure.Database.Repositories;
using Game.Infrastructure.Database.Seeders;
using Game.Infrastructure.Handlers;
using Game.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net.WebSockets;
using System.Text;

namespace Game.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            builder.Host.UseSerilog();

            var dbPath = builder.Environment.IsDevelopment()
                ? Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "Game.Infrastructure", "Database", "superplay.db"))
                : Path.Combine("/app", "Database", "superplay.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlite(
                    $"Data Source={dbPath}",
                    sql => sql.MigrationsAssembly("Game.Infrastructure")
                )
            );

            // Add services to the container.
            builder.Services.Scan(scan => scan
                .FromAssembliesOf(typeof(LoginHandler))
                .AddClasses(classes => classes.AssignableTo(typeof(IMessageHandler<,,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );
            builder.Services.AddSingleton(Log.Logger);
            builder.Services.AddSingleton<HandlerRegistry>();
            builder.Services.AddSingleton<ConnectionManager>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddScoped<WebSocketDispatcher>();

            builder.Services.AddScoped<PlayersRepository>();
            builder.Services.AddScoped<ResourceBalanceRepository>();
            builder.Services.AddScoped<ResourceTypeRepository>();
            builder.Services.AddScoped<GiftTransactionRepository>();

            builder.Services.AddTransient<LoginHandler>();
            builder.Services.AddTransient<UpdateResourceHandler>();
            builder.Services.AddTransient<GiftTransactionHandler>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    db.Database.Migrate();

                    var resourceTypeSeeder = new ResourceTypeSeeder(db);
                    await resourceTypeSeeder.SeedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError("Could not apply database migrations. \n{ex}", ex);
                }
            }

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            app.UseWebSockets(webSocketOptions);

            app.MapGet("/ping", () => Results.Text("pong"));
            app.Map("/game-server", async context =>
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var cancellationToken = context.RequestAborted;

                var dispatcher = context.RequestServices.GetRequiredService<WebSocketDispatcher>();
                await dispatcher.DispatchAsync(webSocket, cancellationToken);
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
