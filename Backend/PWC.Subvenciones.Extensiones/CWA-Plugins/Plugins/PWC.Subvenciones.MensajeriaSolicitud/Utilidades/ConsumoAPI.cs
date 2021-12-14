using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.MensajeriaSolicitud.Utilidades
{
    public class ConsumoAPI
    {
        public static string EjecutarAPI(string url, dynamic content)
        {
            string apiResponse = string.Empty;

            using (var httpClientHandler = new HttpClientHandler())
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    using (var response = httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")).Result)
                    {
                        apiResponse = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }

            return apiResponse;
        }
    }
}
