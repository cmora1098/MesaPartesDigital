using Microsoft.EntityFrameworkCore;
using MesaPartesDigital.Models;

namespace MesaPartesDigital.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TipoPersona> TipoPersonas { get; set; }
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<TipoDocPer> TipoDocPers { get; set; }

        public DbSet<Estado> Estados { get; set; }
    }
}