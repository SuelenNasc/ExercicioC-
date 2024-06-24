using loja.data;
using loja.models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loja.models;

namespace loja.services
{
    public class VendaService
    {
        private readonly LojaDbContext _context;

        public VendaService(LojaDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> AddVendaAsync(Venda venda)
        {
            var cliente = await _context.Clientes.FindAsync(venda.ClienteId);
            var produto = await _context.Produtos.FindAsync(venda.ProdutoId);

            if (cliente == null || produto == null)
            {
                return null;
            }

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();
            return venda;
        }

        public async Task<IEnumerable<Venda>> GetVendasByProdutoIdDetalhadaAsync(int produtoId)
        {
            return (IEnumerable<Venda>)await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.ProdutoId == produtoId)
                .Select(v => new {
                    v.Produto.Nome,
                    v.DataVenda,
                    v.Id,
                    ClienteNome = v.Cliente.Nome,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByProdutoIdSumarizadaAsync(int produtoId)
        {
            return await _context.Vendas
                .Where(v => v.ProdutoId == produtoId)
                .GroupBy(v => v.Produto.Nome)
                .Select(g => new {
                    ProdutoNome = g.Key,
                    QuantidadeTotalVendida = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotalCobrados = g.Sum(v => v.QuantidadeVendida * v.PrecoUnitario)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Venda>> GetVendasByClienteIdDetalhadaAsync(int clienteId)
        {
            return (IEnumerable<Venda>)await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Where(v => v.ClienteId == clienteId)
                .Select(v => new {
                    v.Produto.Nome,
                    v.DataVenda,
                    v.Id,
                    v.QuantidadeVendida,
                    v.PrecoUnitario
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetVendasByClienteIdSumarizadaAsync(int clienteId)
        {
            return await _context.Vendas
                .Where(v => v.ClienteId == clienteId)
                .GroupBy(v => v.Produto.Nome)
                .Select(g => new {
                    ProdutoNome = g.Key,
                    QuantidadeTotalVendida = g.Sum(v => v.QuantidadeVendida),
                    PrecoTotalCobrados = g.Sum(v => v.QuantidadeVendida * v.PrecoUnitario)
                })
                .ToListAsync();
        }
    }
}
