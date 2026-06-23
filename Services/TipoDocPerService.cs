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

         public async Task<List<TipoDocPer>> ObtenerTiposDocPerAsync()
        {
            return await _context.TipoDocPers.ToListAsync();
        }

        public async Task<PersonaBusquedaDto?> BuscarPersonaPorDocumentoAsync(int iCodTipoDocPer, string vDocPer)
        {
            var resultado = await _context.ObtenerPersonaPorDocumentoAsync(iCodTipoDocPer, vDocPer);
            return resultado.FirstOrDefault();
        }
    }
}