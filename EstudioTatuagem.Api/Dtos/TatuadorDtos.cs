namespace EstudioTatuagem.Api.Dtos;

public record CriarTatuadorDto(string Nome, string Especialidade, decimal ValorHora);
public record AtualizarTatuadorDto(string Nome, string Especialidade, decimal ValorHora);
public record TatuadorRespostaDto(int Id, string Nome, string Especialidade, decimal ValorHora, bool Ativo);