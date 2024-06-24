namespace Loja.models
{
    public class Fornecedor
    {
        public int Id {get; set;}
        public String Cnpj {get; set;} = string.Empty;
        public String Nome {get; set;} = string.Empty;
        public String Endereco {get; set;} = string.Empty;
        public String email {get; set;} = string.Empty;
        public String telefone {get; set;} = string.Empty;
    }
}