namespace EstudioTatuagem.Api.Modelos;

public class Tatuador
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Especialidade { get; private set; } = string.Empty;
    public decimal ValorHora { get; private set; }
    public bool Ativo { get; private set; } = true;

    public List<Sessao> Sessoes { get; private set; } = new();

    private Tatuador() { }

    public Tatuador(string nome, string especialidade, decimal valorHora)
    {
        Atualizar(nome, especialidade, valorHora);
    }

    public void Atualizar(string nome, string especialidade, decimal valorHora)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(especialidade)) throw new ArgumentException("Especialidade é obrigatória.");
        if (valorHora <= 0) throw new ArgumentException("Valor por hora deve ser maior que zero.");

        Nome = nome;
        Especialidade = especialidade;
        ValorHora = valorHora;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}