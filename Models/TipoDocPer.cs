using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MesaPartesDigital.Models
{
    [Table("T_TipoDocPer")] // 👈 Nombre de tu tabla real
    public class TipoDocPer
    {
        [Key]
        [Column("iCodTipoDocPer")] // 👈 Tu Llave Primaria
        public int ICodTipoDocPer { get; set; }

        [Column("vDescTipoDoc")] // 👈 Ojo aquí, usaste "vDescTipoDoc" según tu esquema
        public string VDescTipoDoc { get; set; } = string.Empty;

        [Column("bActivo")]
        public bool BActivo { get; set; }
    }
}