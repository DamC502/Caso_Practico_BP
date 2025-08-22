using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;

namespace Frontend.Pages
{
    public class IndexModel : PageModel
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public List<Colaborador> colaboradores { get; set;  }
        public List<DefPuesto> puestos { get; set;}
        
        public string ErrorMessage { get; set; }
        public string baseAPIURL { get; set;  }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger,
            IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }


        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            baseAPIURL = _configuration["ApiSettings:BaseUrl"];

            try
            {
                
                colaboradores = await client.GetFromJsonAsync<List<Colaborador>>("/api/Colaborador");

                Dictionary<long?, bool> CodsColaboradores = new Dictionary<long?, bool>();
                foreach (var colaborador in colaboradores)
                {
                    if (colaborador.Codigo_Jefe is not null && colaborador.Codigo_Jefe != -1 )
                    {
                        if (!CodsColaboradores.TryGetValue(colaborador.Codigo_Jefe, out _))
                        {
                            CodsColaboradores.Add(colaborador.Codigo_Jefe, true);
                        }
                    }
                    
                }

                foreach (var colaborador in colaboradores)
                {
                    if (CodsColaboradores.TryGetValue(colaborador.Codigo_Colaborador, out _))
                    {
                        colaborador.HasChild = true; 
                    }
                }

                puestos = await client.GetFromJsonAsync<List<DefPuesto>>("/api/Colaborador/GetPuestos");
                Console.WriteLine("sop");
            }
            catch (HttpRequestException ex)
            {
                // Maneja errores de red o del servidor
                ErrorMessage = $"Error al conectar con la API: {ex.Message}";
            }
            catch (JsonException ex)
            {
                // Maneja errores de deserialización JSON
                ErrorMessage = $"Error al procesar la respuesta de la API: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Maneja otros errores
                ErrorMessage = $"Ocurrió un error inesperado: {ex.Message}";
            }

        }
    }
}
