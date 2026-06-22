using MesaPartesDigital.Data;
using MesaPartesDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace MesaPartesDigital.Services
{
    public class TipoDocPerService
    {
        private readonly ApplicationDbContext _context;

        public TipoDocPerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 TU TERCER GET
        public async Task<List<TipoDocPer>> ObtenerTiposDocPerAsync()
        {
            return await _context.TipoDocPers.ToListAsync();
        }
    }
}