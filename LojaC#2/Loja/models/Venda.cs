using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using loja.models;

namespace Loja.models
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public string NotaFiscal { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public int QuantidadeVendida { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}