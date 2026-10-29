namespace EstudioTatuagem.Api.Dtos;

public record CriarSessaoDto(DateTime DataHoraInicio, DateTime DataHoraFim, string DescricaoTatuagem, int TatuadorId);
public record RespostaSessaoDto(int Id, DateTime DataHoraInicio, DateTime DataHoraFim, string DescricaoTatuagem, int TatuadorId, string NomeTatuador, int ClienteId, string NomeCliente);