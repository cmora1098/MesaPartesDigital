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
            _connectionString = configuration.GetConnectionString("CadenaConexion")!;
        }

        // 🟢 CORREGIDO: Para el archivo Principal (Usa IBrowserFile de forma nativa y eficiente)
        public async Task<string> GuardarArchivoEnPCAsync(IBrowserFile archivo)
        {
            if (archivo == null) return null;

            try
            {
                if (!Directory.Exists(_rutaLocalPC))
                {
                    Directory.CreateDirectory(_rutaLocalPC);
                }

                string extension = Path.GetExtension(archivo.Name);
                string nombreLimpio = Path.GetFileNameWithoutExtension(archivo.Name).Replace(" ", "_");
                string nombreUnicoArchivo = $"{Guid.NewGuid()}__{nombreLimpio}{extension}";
                string rutaCompletaPC = Path.Combine(_rutaLocalPC, nombreUnicoArchivo);

                long maxFileSize = 50 * 1024 * 1024; // 50MB

                // 🛠️ FIX: Se eliminó el bucle manual innecesario que corrompía la transferencia secundaria
                using var streamInput = archivo.OpenReadStream(maxFileSize);
                using var streamOutput = File.Create(rutaCompletaPC);

                await streamInput.CopyToAsync(streamOutput); // .NET maneja el buffer de 80KB de forma óptima automáticamente

                return rutaCompletaPC;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar archivo en PC: {ex.Message}");
                throw;
            }
        }

        // 🟢 MANTENIDO: Para los Anexos (En caso de que envíes strings en Base64)
        public async Task<string> GuardarArchivoEnPCAsync(string nombreArchivo, string base64Data)
        {
            try
            {
                if (!Directory.Exists(_rutaLocalPC))
                {
                    Directory.CreateDirectory(_rutaLocalPC);
                }

                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                byte[] archivoBytes = Convert.FromBase64String(base64Data);
                string nombreUnico = $"{Guid.NewGuid()}_{nombreArchivo}";
                string rutaCompleta = Path.Combine(_rutaLocalPC, nombreUnico);

                await File.WriteAllBytesAsync(rutaCompleta, archivoBytes);
                return rutaCompleta;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DocumentoService] 🚨 Error al decodificar y guardar Base64: {ex.Message}");
                throw;
            }
        }

        public async Task<RegistroDocumentoResponse> RegistrarArchivoMesaPartesAsync(RegistroDocumentoRequest request)
        {
            // 🛠️ Sintaxis C# moderna para usar bloques using más legibles y planos
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("USP_RegistroPersonaNatural", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 120; // 2 minutos de tolerancia para archivos pesados

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
    }
}