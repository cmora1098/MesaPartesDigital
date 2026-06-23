using System.Net.Http.Json;
using MesaPartesDigital.Models;

namespace MesaPartesDigital.Services
{
    public class UbigeoService
    {
        private readonly HttpClient _httpClient;

        public UbigeoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. Departamentos
        public async Task<List<Departamento>> ObtenerDepartamentosAsync()
        {
            try
            {
                var respuesta = await _httpClient.GetFromJsonAsync<List<Departamento>>("apiubigeo/departamentos");
                return respuesta ?? new List<Departamento>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error departamentos: {ex.Message}");
                return new List<Departamento>();
            }
        }

        // 2. Provincias (Pasa el código de departamento en la URL)
        public async Task<List<Provincia>> ObtenerProvinciasAsync(string codDepartamento)
        {
            try
            {
                var respuesta = await _httpClient.GetFromJsonAsync<List<Provincia>>($"apiubigeo/provincias/{codDepartamento}");
                return respuesta ?? new List<Provincia>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error provincias: {ex.Message}");
                return new List<Provincia>();
            }
        }

        // 3. Distritos (Pasa el código de provincia en la URL)
        public async Task<List<Distrito>> ObtenerDistritosAsync(string codProvincia)
        {
            try
            {
                var respuesta = await _httpClient.GetFromJsonAsync<List<Distrito>>($"apiubigeo/distritos/{codProvincia}");
                return respuesta ?? new List<Distrito>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error distritos: {ex.Message}");
                return new List<Distrito>();
            }
        }
    }
}