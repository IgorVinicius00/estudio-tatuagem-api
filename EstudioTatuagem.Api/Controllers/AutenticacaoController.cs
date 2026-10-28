using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstudioTatuagem.Api.Dados;
using EstudioTatuagem.Api.Dtos;
using EstudioTatuagem.Api.Modelos;
using EstudioTatuagem.Api.Servicos;

namespace EstudioTatuagem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AutenticacaoController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // POST: api/autenticacao/registro
    [HttpPost("registro")]
    public async Task<ActionResult<RespostaLoginDto>> Registrar([FromBody] RegistoUsuarioDto dto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email.ToLower()))
            return BadRequest(new { mensagem = "E-mail já cadastrado." });

        try
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
            var usuario = new Usuario(dto.Nome, dto.Email, senhaHash, "cliente");

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var segredoJwt = _configuration["Jwt:Segredo"] ?? Environment.GetEnvironmentVariable("Jwt__Segredo") ?? "SegredoPadraoParaDesenvolvimento123456789";
            var token = TokenService.GerarToken(usuario, segredoJwt);

            return Ok(new RespostaLoginDto(token, usuario.Nome, usuario.Email, usuario.Papel));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // POST: api/autenticacao/login
    [HttpPost("login")]
    public async Task<ActionResult<RespostaLoginDto>> Login([FromBody] LoginDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        var segredoJwt = _configuration["Jwt:Segredo"] ?? Environment.GetEnvironmentVariable("Jwt__Segredo") ?? "SegredoPadraoParaDesenvolvimento123456789";
        var token = TokenService.GerarToken(usuario, segredoJwt);

        return Ok(new RespostaLoginDto(token, usuario.Nome, usuario.Email, usuario.Papel));
    }
}