using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using loja.data;
using loja.models;
using Loja.models;
using Microsoft.EntityFrameworkCore;

namespace loja.services
{
    public class UsuarioService
    {
        private readonly LojaDbContext _context;

        public UsuarioService(LojaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task AddUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Usuario> GetUsuarioByEmailAndSenhaAsync(string email, string senha)
        {
            return await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == email && u.Senha == senha);
        }
    }
}