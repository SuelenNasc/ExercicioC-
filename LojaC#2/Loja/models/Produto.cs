namespace loja.models
{
    public class Produto{
        public int Id {get; set;}
        public String Nome {get; set;} = string.Empty;
        public Double Preco {get; set;}
        public String Fornecedor {get; set;} = string.Empty;
    }
}