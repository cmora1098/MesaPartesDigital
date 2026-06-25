using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MesaPartesDigital.Models
{
    [Table("T_TipoDocumento")]
    public class TipoDocumento
    {
        [Key]
        [Column("iCodTipoDoc")]
        public int ICodTipoDoc { get; set; }

        [Column("vNombreTipoDoc")]
        public string VNombreTipoDoc { get; set; } = string.Empty;

        [Column("bActivo")]
        public bool BActivo { get; set; }
    }
}