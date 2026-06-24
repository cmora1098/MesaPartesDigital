namespace MesaPartesDigital.Models
{
    public class RegistroDocumentoRequest
    {
        // Remitente
        public int ICodTipoDocPer { get; set; }
        public string VDocPer { get; set; } = string.Empty;
        public string VNombres { get; set; } = string.Empty;
        public string VApellidoPaterno { get; set; } = string.Empty;
        public string VApellidoMaterno { get; set; } = string.Empty;
        public string VEmail { get; set; } = string.Empty;
        public string VTelefono { get; set; } = string.Empty;
        public string VDireccion { get; set; } = string.Empty;
        public string VCodDistrito { get; set; } = string.Empty;

        // Documento
        public int ICodAsunto { get; set; }
        public string VRutaDoc { get; set; } = string.Empty;
        public int ICodTipoDoc { get; set; }
        public string VNroDoc { get; set; } = string.Empty;
        public DateTime DFecDoc { get; set; }
        public string VReferencia { get; set; } = string.Empty;
        public string VNroPagFolios { get; set; } = string.Empty;
        public bool BTipo { get; set; } 
        public string VLink { get; set; } = string.Empty;

        
    }

    public class RegistroDocumentoResponse
    {
        public int ICodDoc { get; set; }
        public int ICodAsunto { get; set; }
        public string Status { get; set; } = string.Empty;
        public string MailSeguimiento { get; set; } = string.Empty;
        public string VAutoGenerado { get; set; } = string.Empty;
    }
}