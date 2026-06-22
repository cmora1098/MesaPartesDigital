using MesaPartesDigital.Data;
using MesaPartesDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace MesaPartesDigital.Services
{
    public class TipoDocumentoService
    {
        private readonly ApplicationDbContext _context;

        public TipoDocumentoService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 TU NUEVO GET: Trae todos los tipos de documento
        public async Task<List<TipoDocumento>> ObtenerTiposDocumentoAsync()
        {
            return await _context.TipoDocumentos.ToListAsync();
        }
    }
}