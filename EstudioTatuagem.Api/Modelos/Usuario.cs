namespace EstudioTatuagem.Api.Modelos;

public class Usuario
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Papel { get; private set; } = "cliente";

    private Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string papel = "cliente")
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) throw new ArgumentException("E-mail inválido.");
        if (string.IsNullOrWhiteSpace(senhaHash)) throw new ArgumentException("Hash da senha é obrigatório.");

        Nome = nome;
        Email = email.ToLowerInvariant();
        SenhaHash = senhaHash;
        Papel = papel.ToLowerInvariant();
    }
}