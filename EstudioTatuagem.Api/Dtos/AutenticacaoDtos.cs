namespace EstudioTatuagem.Api.Dtos;

public record RegistoUsuarioDto(string Nome, string Email, string Senha);
public record LoginDto(string Email, string Senha);
public record RespostaLoginDto(string Token, string Nome, string Email, string Papel);