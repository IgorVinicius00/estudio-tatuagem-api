using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using EstudioTatuagem.Api.Dados;
using EstudioTatuagem.Api.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("Padrao") 
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__Padrao") 
    ?? throw new InvalidOperationException("Configure a variável de ambiente ConnectionStrings__Padrao.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configuração de Autenticação JWT
var segredoJwt = builder.Configuration["Jwt:Segredo"] 
    ?? Environment.GetEnvironmentVariable("Jwt__Segredo") 
    ?? "SegredoPadraoParaDesenvolvimento123456789";

var key = Encoding.ASCII.GetBytes(segredoJwt);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger com suporte a JWT Header (.NET 10 / Microsoft.OpenApi v2.0)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Estúdio de Tatuagem API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT neste formato: Bearer {seu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

var app = builder.Build();

// Executa Migrations e Seed automaticamente no startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var adminSenha = builder.Configuration["Seed:AdminSenha"] 
        ?? Environment.GetEnvironmentVariable("Seed__AdminSenha") 
        ?? "Admin@2026";

    // Seed de Admin
    if (!await db.Usuarios.AnyAsync(u => u.Email == "admin@estudio.com"))
    {
        var adminHash = BCrypt.Net.BCrypt.HashPassword(adminSenha);
        db.Usuarios.Add(new Usuario("Administrador", "admin@estudio.com", adminHash, "admin"));
        await db.SaveChangesAsync();
    }

    // Seed de Tatuadores
    if (!await db.Tatuadores.AnyAsync())
    {
        db.Tatuadores.AddRange(
            new Tatuador("Carlos 'Oeste' Silva", "Realismo e Preto e Cinza", 250),
            new Tatuador("Mariana Costa", "Aquarela e Fine Line", 200),
            new Tatuador("Lucas 'Kage' Tanaka", "Oriental e Irezumi", 280),
            new Tatuador("Beatriz Rocha", "Old School e Neotradicional", 220),
            new Tatuador("Rafael 'Gordo' Mendonça", "Tribal e Blackwork", 190),
            new Tatuador("Camila Alcantara", "Geométrico e Pontilhismo", 230),
            new Tatuador("Rodrigo 'Xis' Lima", "Lettering e Chicano", 180),
            new Tatuador("Fernanda 'Nanda' Souza", "Minimalista e Micro-realismo", 210)
        );
        await db.SaveChangesAsync();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapHealthChecks("/health");

app.MapGet("/api/versao", () => Results.Ok(new
{
    commit = Environment.GetEnvironmentVariable("RENDER_GIT_COMMIT") ?? "local",
    ambiente = app.Environment.EnvironmentName
}));

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }