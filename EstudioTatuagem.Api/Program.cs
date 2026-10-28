using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using EstudioTatuagem.Api.Dados;
using EstudioTatuagem.Api.Modelos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configuração do DbContext com Npgsql / PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("Padrao");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Configure a variável de ambiente ConnectionStrings__Padrao.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// Swashbuckle / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Estúdio de Tatuagem API",
        Version = "v1",
        Description = "Projeto escolar — empresa fictícia"
    });
});

var app = builder.Build();

// Migrations automáticas e Seed de Dados na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // Seed de Utilizadores
    if (!await db.Usuarios.AnyAsync())
    {
        var adminSenha = Environment.GetEnvironmentVariable("Seed__AdminSenha") ?? "Admin@2026";
        var adminHash = BCrypt.Net.BCrypt.HashPassword(adminSenha);
        var clienteHash = BCrypt.Net.BCrypt.HashPassword("Cliente@2026");

        var admin = new Usuario("Admin do Estúdio", "admin@estudio.com", adminHash, "admin");
        var cliente1 = new Usuario("Lucas Silva", "lucas@exemplo.com", clienteHash, "cliente");
        var cliente2 = new Usuario("Mariana Costa", "mariana@exemplo.com", clienteHash, "cliente");

        db.Usuarios.AddRange(admin, cliente1, cliente2);
        await db.SaveChangesAsync();
    }

    // Seed de Tatuadores (Pelo menos 8 registos realistas)
    if (!await db.Tatuadores.AnyAsync())
    {
        var tatuadores = new List<Tatuador>
        {
            new("Gabriel Santos", "Blackwork", 250m),
            new("Camila Rocha", "Realismo", 350m),
            new("Rodrigo Lima", "Old School", 220m),
            new("Beatriz Melo", "Aquarela", 280m),
            new("Felipe Xavier", "Neotraditional", 300m),
            new("Juliana Paes", "Fine Line", 260m),
            new("Thiago Martins", "Oriental", 320m),
            new("Amanda Prado", "Geométrico", 240m)
        };

        db.Tatuadores.AddRange(tatuadores);
        await db.SaveChangesAsync();
    }
}

// Middlewares
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapHealthChecks("/health");

app.MapGet("/api/versao", () => Results.Ok(new
{
    commit = Environment.GetEnvironmentVariable("RENDER_GIT_COMMIT") ?? "local",
    ambiente = app.Environment.EnvironmentName
}));

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }