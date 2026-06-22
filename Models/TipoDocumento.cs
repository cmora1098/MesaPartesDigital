using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MesaPartesDigital.Models
{
    [Table("T_TipoDocumento")] // 👈 Tu tabla real
    public class TipoDocumento
    {
        [Key]
        [Column("iCodTipoDoc")] // 👈 Tu PK
        public int ICodTipoDoc { get; set; }

        [Column("vNombreTipoDoc")]
        public string VNombreTipoDoc { get; set; } = string.Empty;

        [Column("bActivo")]
        public bool BActivo { get; set; }
    }
}