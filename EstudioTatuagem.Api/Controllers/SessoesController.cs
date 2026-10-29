using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstudioTatuagem.Api.Dados;
using EstudioTatuagem.Api.Dtos;
using EstudioTatuagem.Api.Modelos;

namespace EstudioTatuagem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SessoesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/sessoes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RespostaSessaoDto>>> ObterSessoes()
    {
        var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var papelClaim = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

        int usuarioId = int.Parse(usuarioIdClaim);

        var query = _context.Sessoes
            .Include(s => s.Tatuador)
            .Include(s => s.Cliente)
            .AsQueryable();

        // Se não for admin, só visualiza as próprias sessões
        if (papelClaim != "admin")
        {
            query = query.Where(s => s.ClienteId == usuarioId);
        }

        var sessoes = await query
            .Select(s => new RespostaSessaoDto(
                s.Id,
                s.DataHoraInicio,
                s.DataHoraFim,
                s.DescricaoTatuagem,
                s.TatuadorId,
                s.Tatuador != null ? s.Tatuador.Nome : "",
                s.ClienteId,
                s.Cliente != null ? s.Cliente.Nome : ""
            ))
            .ToListAsync();

        return Ok(sessoes);
    }

    // POST: api/sessoes
    [HttpPost]
    public async Task<ActionResult<RespostaSessaoDto>> AgendarSessao([FromBody] CriarSessaoDto dto)
    {
        var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

        int clienteId = int.Parse(usuarioIdClaim);

        var tatuador = await _context.Tatuadores.FindAsync(dto.TatuadorId);
        if (tatuador == null || !tatuador.Ativo)
            return BadRequest(new { mensagem = "Tatuador não encontrado ou inativo." });

        try
        {
            var sessao = new Sessao(dto.DataHoraInicio.ToUniversalTime(), dto.DataHoraFim.ToUniversalTime(), dto.DescricaoTatuagem, dto.TatuadorId, clienteId);
            _context.Sessoes.Add(sessao);
            await _context.SaveChangesAsync();

            var cliente = await _context.Usuarios.FindAsync(clienteId);

            var resposta = new RespostaSessaoDto(
                sessao.Id,
                sessao.DataHoraInicio,
                sessao.DataHoraFim,
                sessao.DescricaoTatuagem,
                sessao.TatuadorId,
                tatuador.Nome,
                sessao.ClienteId,
                cliente?.Nome ?? ""
            );

            return CreatedAtAction(nameof(ObterSessoes), new { id = sessao.Id }, resposta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}