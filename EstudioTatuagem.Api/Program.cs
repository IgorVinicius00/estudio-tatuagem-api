using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Health checks
builder.Services.AddHealthChecks();

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

// Middlewares
app.UseSwagger();
app.UseSwaggerUI();

// Redireciona a raiz para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Endpoint de saúde exigido (/health)
app.MapHealthChecks("/health");

// Endpoint de versão exigido (/api/versao)
app.MapGet("/api/versao", () => Results.Ok(new
{
    commit = Environment.GetEnvironmentVariable("RENDER_GIT_COMMIT") ?? "local",
    ambiente = app.Environment.EnvironmentName
}));

app.UseAuthorization();
app.MapControllers();

app.Run();

// Classe pública para suporte aos testes de integração no xUnit
public partial class Program { }