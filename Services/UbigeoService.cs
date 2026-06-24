using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using MesaPartesDigital.Models;

namespace MesaPartesDigital.Services
{
    public class UbigeoService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        // Clave base fija para los departamentos
        private const string CacheDepartamentosKey = "Ubigeo_Departamentos";

        public UbigeoService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        // 1. Departamentos (Cachea el listado completo)
        public async Task<List<Departamento>> ObtenerDepartamentosAsync()
        {
            try
            {
                // Intentamos recuperar de la caché
                if (!_cache.TryGetValue(CacheDepartamentosKey, out List<Departamento>? departamentos))
                {
                    // Si no existe, hacemos la llamada HTTP
                    var respuesta = await _httpClient.GetFromJsonAsync<List<Departamento>>("apiubigeo/departamentos");
                    departamentos = respuesta ?? new List<Departamento>();

                    // Guardamos en caché por 24 horas (el Ubigeo rara vez cambia)
                    var opcionesCache = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(24))
                        .SetSlidingExpiration(TimeSpan.FromHours(4)); // Si nadie lo usa en 4h, se libera

                    _cache.Set(CacheDepartamentosKey, departamentos, opcionesCache);
                    Console.WriteLine("[UbigeoService] 🌐 Departamentos cargados desde la API y guardados en Caché.");
                }
                else
                {
                    Console.WriteLine("[UbigeoService] ⚡ Departamentos recuperados desde la Caché.");
                }

                return departamentos ?? new List<Departamento>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error departamentos: {ex.Message}");
                return new List<Departamento>();
            }
        }

        // 2. Provincias (Pasa el código de departamento en la URL y crea una caché dinámica por código)
        public async Task<List<Provincia>> ObtenerProvinciasAsync(string codDepartamento)
        {
            if (string.IsNullOrEmpty(codDepartamento)) return new List<Provincia>();

            string cacheKey = $"Ubigeo_Provincias_{codDepartamento}";

            try
            {
                if (!_cache.TryGetValue(cacheKey, out List<Provincia>? provincias))
                {
                    var respuesta = await _httpClient.GetFromJsonAsync<List<Provincia>>($"apiubigeo/provincias/{codDepartamento}");
                    provincias = respuesta ?? new List<Provincia>();

                    // Guardamos en caché las provincias de este departamento específico por 12 horas
                    _cache.Set(cacheKey, provincias, TimeSpan.FromHours(12));
                    Console.WriteLine($"[UbigeoService] 🌐 Provincias para Dep: {codDepartamento} cargadas desde la API.");
                }
                else
                {
                    Console.WriteLine($"[UbigeoService] ⚡ Provincias para Dep: {codDepartamento} recuperadas desde la Caché.");
                }

                return provincias ?? new List<Provincia>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error provincias: {ex.Message}");
                return new List<Provincia>();
            }
        }

        // 3. Distritos (Pasa el código de provincia en la URL y crea una caché dinámica por código)
        public async Task<List<Distrito>> ObtenerDistritosAsync(string codProvincia)
        {
            if (string.IsNullOrEmpty(codProvincia)) return new List<Distrito>();

            string cacheKey = $"Ubigeo_Distritos_{codProvincia}";

            try
            {
                if (!_cache.TryGetValue(cacheKey, out List<Distrito>? distritos))
                {
                    var respuesta = await _httpClient.GetFromJsonAsync<List<Distrito>>($"apiubigeo/distritos/{codProvincia}");
                    distritos = respuesta ?? new List<Distrito>();

                    // Guardamos en caché los distritos de esta provincia específica por 12 horas
                    _cache.Set(cacheKey, distritos, TimeSpan.FromHours(12));
                    Console.WriteLine($"[UbigeoService] 🌐 Distritos para Prov: {codProvincia} cargados desde la API.");
                }
                else
                {
                    Console.WriteLine($"[UbigeoService] ⚡ Distritos para Prov: {codProvincia} recuperados desde la Caché.");
                }

                return distritos ?? new List<Distrito>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error distritos: {ex.Message}");
                return new List<Distrito>();
            }
        }
    }
}