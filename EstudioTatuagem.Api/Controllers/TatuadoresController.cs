using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstudioTatuagem.Api.Dados;
using EstudioTatuagem.Api.Dtos;
using EstudioTatuagem.Api.Modelos;

namespace EstudioTatuagem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TatuadoresController : ControllerBase
{
    private readonly AppDbContext _context;

    public TatuadoresController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/tatuadores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TatuadorRespostaDto>>> ObterTodos()
    {
        var tatuadores = await _context.Tatuadores
            .Where(t => t.Ativo)
            .Select(t => new TatuadorRespostaDto(t.Id, t.Nome, t.Especialidade, t.ValorHora, t.Ativo))
            .ToListAsync();

        return Ok(tatuadores);
    }

    // GET: api/tatuadores/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TatuadorRespostaDto>> ObterPorId(int id)
    {
        var tatuador = await _context.Tatuadores.FindAsync(id);

        if (tatuador == null)
            return NotFound(new { mensagem = "Tatuador não encontrado." });

        return Ok(new TatuadorRespostaDto(tatuador.Id, tatuador.Nome, tatuador.Especialidade, tatuador.ValorHora, tatuador.Ativo));
    }

    // POST: api/tatuadores
    [HttpPost]
    public async Task<ActionResult<TatuadorRespostaDto>> Criar([FromBody] CriarTatuadorDto dto)
    {
        try
        {
            var tatuador = new Tatuador(dto.Nome, dto.Especialidade, dto.ValorHora);
            _context.Tatuadores.Add(tatuador);
            await _context.SaveChangesAsync();

            var resposta = new TatuadorRespostaDto(tatuador.Id, tatuador.Nome, tatuador.Especialidade, tatuador.ValorHora, tatuador.Ativo);
            return CreatedAtAction(nameof(ObterPorId), new { id = tatuador.Id }, resposta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // PUT: api/tatuadores/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarTatuadorDto dto)
    {
        var tatuador = await _context.Tatuadores.FindAsync(id);

        if (tatuador == null)
            return NotFound(new { mensagem = "Tatuador não encontrado." });

        try
        {
            tatuador.Atualizar(dto.Nome, dto.Especialidade, dto.ValorHora);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // DELETE: api/tatuadores/5 (Desativação lógica)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desativar(int id)
    {
        var tatuador = await _context.Tatuadores.FindAsync(id);

        if (tatuador == null)
            return NotFound(new { mensagem = "Tatuador não encontrado." });

        tatuador.Desativar();
        await _context.SaveChangesAsync();

        return NoContent();
    }
}