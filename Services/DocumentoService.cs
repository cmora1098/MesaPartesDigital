using MesaPartesDigital.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
 
namespace MesaPartesDigital.Services
{
    public class DocumentoService
    {
        // Ruta temporal en tu PC para validar los archivos
        private readonly string _rutaLocalPC = @"C:\MesaDePartesLocal";

        public async Task<string> GuardarArchivoEnPCAsync(IBrowserFile archivo)
        {
            if (archivo == null) return null;

            try
            {
                // Asegurarse de que el directorio local exista en tu PC
                if (!Directory.Exists(_rutaLocalPC))
                {
                    Directory.CreateDirectory(_rutaLocalPC);
                }

                // Generar un nombre único imitando el formato de tu BD (usando un GUID para evitar duplicados)
                string extension = Path.GetExtension(archivo.Name);
                string nombreLimpio = Path.GetFileNameWithoutExtension(archivo.Name).Replace(" ", "_");
                string nombreUnicoArchivo = $"{Guid.NewGuid()}__{nombreLimpio}{extension}";

                // Ruta completa de almacenamiento en el disco duro de tu computadora
                string rutaCompletaPC = Path.Combine(_rutaLocalPC, nombreUnicoArchivo);

                // Guardar el flujo del archivo (File Stream) al disco local
                // Ajustamos el tamaño máximo permitido según tu interfaz (50MB = 50 * 1024 * 1024)
                long maxFileSize = 50 * 1024 * 1024;
                using var streamInput = archivo.OpenReadStream(maxFileSize);
                using var streamOutput = File.Create(rutaCompletaPC);
                await streamInput.CopyToAsync(streamOutput);

                // Retornamos el formato string que se guardará en el parámetro @vRutaDoc de la base de datos
                // Ejemplo: REGDOC/c40e47c8-df91..._nombre.pdf
                return $"REGDOC/{nombreUnicoArchivo}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar archivo en PC: {ex.Message}");
                throw;
            }
        }


        private readonly string _connectionString;

        public DocumentoService(IConfiguration configuration)
        {
            // Lee la cadena de conexión desde tu appsettings.json
            _connectionString = configuration.GetConnectionString("CadenaConexion")!;
        }

        public async Task<RegistroDocumentoResponse> RegistrarArchivoMesaPartesAsync(RegistroDocumentoRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("USP_RegistroPersonaNatural", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // ... (Todos tus parámetros anteriores se quedan igual) ...
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

                    // 🛠️ NUEVO PARÁMETRO ENVIADO A SQL
                    command.Parameters.AddWithValue("@vLink", (object)request.VLink ?? DBNull.Value);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
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
                    }
                }
            }
            throw new Exception("No se pudo obtener la respuesta del documento registrado.");
        }


    }
}
