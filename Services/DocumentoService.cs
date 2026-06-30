using MesaPartesDigital.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MesaPartesDigital.Services
{
    public class DocumentoService
    {
        private readonly string _rutaLocalPC = @"C:\MesaDePartesLocal\Archivos";
        private readonly string _connectionString;

        public DocumentoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // 🟢 ACTUALIZADO: Guarda el archivo principal estructurado por Año/Mes para optimizar el almacenamiento
        public async Task<string> GuardarArchivoEnPCAsync(IBrowserFile archivo)
        {
            if (archivo == null) return null;

            try
            {
                // 1. Crear subestructura dinámica cronológica (Ej: C:\MesaDePartesLocal\Archivos\2026\06)
                string anioActual = DateTime.Now.ToString("yyyy");
                string mesActual = DateTime.Now.ToString("MM");
                string rutaDestinoFinal = Path.Combine(_rutaLocalPC, anioActual, mesActual);

                if (!Directory.Exists(rutaDestinoFinal))
                {
                    Directory.CreateDirectory(rutaDestinoFinal);
                }

                // 2. Sanitizar y preparar nombre único
                string extension = Path.GetExtension(archivo.Name);
                string nombreLimpio = Path.GetFileNameWithoutExtension(archivo.Name).Replace(" ", "_");
                string nombreUnicoArchivo = $"{Guid.NewGuid()}__{nombreLimpio}{extension}";
                string rutaCompletaPC = Path.Combine(rutaDestinoFinal, nombreUnicoArchivo);

                long maxFileSize = 50 * 1024 * 1024; // 50MB

                // 3. Transmisión nativa del stream
                using var streamInput = archivo.OpenReadStream(maxFileSize);
                using var streamOutput = File.Create(rutaCompletaPC);

                await streamInput.CopyToAsync(streamOutput);

                return rutaCompletaPC;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar archivo en PC: {ex.Message}");
                throw;
            }
        }

        // 🟢 ACTUALIZADO: Para los Anexos en Base64 (Alineado a la misma estructura Año/Mes)
        public async Task<string> GuardarArchivoEnPCAsync(string nombreArchivo, string base64Data)
        {
            if (string.IsNullOrEmpty(base64Data)) return null;

            try
            {
                // 1. Crear la misma subestructura dinámica cronológica
                string anioActual = DateTime.Now.ToString("yyyy");
                string mesActual = DateTime.Now.ToString("MM");
                string rutaDestinoFinal = Path.Combine(_rutaLocalPC, anioActual, mesActual);

                if (!Directory.Exists(rutaDestinoFinal))
                {
                    Directory.CreateDirectory(rutaDestinoFinal);
                }

                // 2. Limpiar cabecera Data URL si existiese
                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                // 3. Decodificar y guardar
                byte[] archivoBytes = Convert.FromBase64String(base64Data);
                string nombreLimpio = nombreArchivo.Replace(" ", "_");
                string nombreUnico = $"{Guid.NewGuid()}_{nombreLimpio}";
                string rutaCompleta = Path.Combine(rutaDestinoFinal, nombreUnico);

                await File.WriteAllBytesAsync(rutaCompleta, archivoBytes);
                return rutaCompleta;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentoService] 🚨 Error al decodificar y guardar Base64: {ex.Message}");
                throw;
            }
        }

        public async Task<RegistroDocumentoResponse> RegistroPersonaNatural_Home(RegistroDocumentoRequest request) // USP_RegistroPersonaNatural
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("USP_RegistroPersonaNatural", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 120; // 2 minutos de tolerancia

            command.Parameters.AddWithValue("@iCodTipoDocPer", request.ICodTipoDocPer);
            command.Parameters.AddWithValue("@vDocPer", request.VDocPer);
            command.Parameters.AddWithValue("@vNombres", request.VNombres);
            command.Parameters.AddWithValue("@vApellidoPaterno", request.VApellidoPaterno);
            command.Parameters.AddWithValue("@vApellidoMaterno", request.VApellidoMaterno);
            command.Parameters.AddWithValue("@vEmail", request.VEmail);
            command.Parameters.AddWithValue("@vTelefono", request.VTelefono);
            command.Parameters.AddWithValue("@vDireccion", request.VDireccion);
            command.Parameters.AddWithValue("@vCodDistrito", request.VCodDistrito);
            command.Parameters.AddWithValue("@iCodAsunto", request.ICodAsunto);
            command.Parameters.AddWithValue("@vRutaDoc", (object)request.VRutaDoc ?? DBNull.Value);
            command.Parameters.AddWithValue("@iCodTipoDoc", request.ICodTipoDoc);
            command.Parameters.AddWithValue("@vNroDoc", request.VNroDoc);
            command.Parameters.AddWithValue("@dFecDoc", request.DFecDoc);
            command.Parameters.AddWithValue("@vReferencia", request.VReferencia);
            command.Parameters.AddWithValue("@vNroPagFolios", request.VNroPagFolios);
            command.Parameters.AddWithValue("@btipo", request.BTipo);
            command.Parameters.AddWithValue("@vLink", (object)request.VLink ?? DBNull.Value);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RegistroDocumentoResponse
                {
                    ICodDoc = Convert.ToInt32(reader["iCodDoc"]),
                    ICodAsunto = Convert.ToInt32(reader["iCodAsunto"]),
                    Status = reader["Status"]?.ToString() ?? "",
                    MailSeguimiento = reader["MailSeguimiento"]?.ToString() ?? "",
                    VAutoGenerado = reader["vAutoGenerado"]?.ToString() ?? ""
                };
            }

            throw new Exception("No se pudo obtener la respuesta del documento registrado.");
        }

        // 🏢 MANTENIDO: Integración del Store Procedure para Empresas
        public async Task<RegistroDocumentoResponse> RegistrarPersonaJuridicaAsync(RegistroDocumentoRequest request, string rucEmpresa, string razonSocial) // USP_RegistroPersonaJuridica
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("USP_RegistroPersonaJuridica", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 120;

            // I. DATOS DE LA EMPRESA
            command.Parameters.AddWithValue("@vRucEmpresa", rucEmpresa);
            command.Parameters.AddWithValue("@vRazonSocial", razonSocial);

            // II. DATOS DEL REPRESENTANTE LEGAL
            command.Parameters.AddWithValue("@iCodTipoDocRep", request.ICodTipoDocPer);
            command.Parameters.AddWithValue("@vDocRep", request.VDocPer);
            command.Parameters.AddWithValue("@vNombresRep", request.VNombres);
            command.Parameters.AddWithValue("@vApellidoPaternoRep", request.VApellidoPaterno);
            command.Parameters.AddWithValue("@vApellidoMaternoRep", request.VApellidoMaterno);
            command.Parameters.AddWithValue("@vEmailRep", request.VEmail);
            command.Parameters.AddWithValue("@vTelefonoRep", (object)request.VTelefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@vDireccionRep", (object)request.VDireccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@vCodDistritoRep", (object)request.VCodDistrito ?? DBNull.Value);

            // III. DATOS DEL DOCUMENTO
            command.Parameters.AddWithValue("@iCodAsunto", request.ICodAsunto);
            command.Parameters.AddWithValue("@vRutaDoc", (object)request.VRutaDoc ?? DBNull.Value);
            command.Parameters.AddWithValue("@iCodTipoDoc", request.ICodTipoDoc);
            command.Parameters.AddWithValue("@vNroDoc", request.VNroDoc);
            command.Parameters.AddWithValue("@dFecDoc", request.DFecDoc);
            command.Parameters.AddWithValue("@vReferencia", request.VReferencia);
            command.Parameters.AddWithValue("@vNroPagFolios", request.VNroPagFolios);
            command.Parameters.AddWithValue("@btipo", request.BTipo);
            command.Parameters.AddWithValue("@vLink", (object)request.VLink ?? DBNull.Value);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RegistroDocumentoResponse
                {
                    ICodDoc = Convert.ToInt32(reader["iCodDoc"]),
                    ICodAsunto = Convert.ToInt32(reader["iCodAsunto"]),
                    Status = reader["Status"]?.ToString() ?? "",
                    MailSeguimiento = reader["MailSeguimiento"]?.ToString() ?? "",
                    VAutoGenerado = reader["vAutoGenerado"]?.ToString() ?? ""
                };
            }

            throw new Exception("No se pudo obtener la respuesta del trámite jurídico registrado.");
        }
    }
}