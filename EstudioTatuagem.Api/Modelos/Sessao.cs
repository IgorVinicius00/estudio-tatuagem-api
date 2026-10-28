namespace EstudioTatuagem.Api.Modelos;

public class Sessao
{
    public int Id { get; private set; }
    public DateTime DataHoraInicio { get; private set; }
    public DateTime DataHoraFim { get; private set; }
    public string DescricaoTatuagem { get; private set; } = string.Empty;

    public int TatuadorId { get; private set; }
    public Tatuador? Tatuador { get; private set; }

    public int ClienteId { get; private set; }
    public Usuario? Cliente { get; private set; }

    private Sessao() { }

    public Sessao(DateTime dataHoraInicio, DateTime dataHoraFim, string descricaoTatuagem, int tatuadorId, int clienteId)
    {
        if (dataHoraInicio.Kind != DateTimeKind.Utc || dataHoraFim.Kind != DateTimeKind.Utc)
            throw new ArgumentException("As datas devem estar no formato UTC.");

        if (dataHoraFim <= dataHoraInicio)
            throw new ArgumentException("Data de fim deve ser posterior à data de início.");

        if (string.IsNullOrWhiteSpace(descricaoTatuagem))
            throw new ArgumentException("Descrição da tatuagem é obrigatória.");

        DataHoraInicio = dataHoraInicio;
        DataHoraFim = dataHoraFim;
        DescricaoTatuagem = descricaoTatuagem;
        TatuadorId = tatuadorId;
        ClienteId = clienteId;
    }
}
