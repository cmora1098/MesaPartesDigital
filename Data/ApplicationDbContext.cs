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

        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<TipoDocPer> TipoDocPers { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<TipoPersona> TipoPersonas { get; set; }
        public DbSet<PersonaBusquedaDto> PersonaBusquedas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔽 Le decimos a EF que este modelo no tiene una tabla física con Primary Key
            modelBuilder.Entity<PersonaBusquedaDto>().HasNoKey();
        }

        // 🔽 Modificamos el método para que use el nuevo objeto
        public async Task<List<PersonaBusquedaDto>> ObtenerPersonaPorDocumentoAsync(int iCodTipoDocPer, string vDocPer)
        {
            return await this.PersonaBusquedas
                .FromSqlInterpolated($"EXEC [dbo].[USP_Persona_ObtenerPorDocumento] @iCodTipoDocPer={iCodTipoDocPer}, @vDocPer={vDocPer}")
                .ToListAsync();
        }

    }
}