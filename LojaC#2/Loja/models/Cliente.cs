namespace Loja.models
{
    public class Cliente
    {
        public int Id { get; set; }
        public String Nome { get; set; } = string.Empty;
        public String Cpf { get; set; } = string.Empty;
        public String Email { get; set; } = string.Empty;
    }
}