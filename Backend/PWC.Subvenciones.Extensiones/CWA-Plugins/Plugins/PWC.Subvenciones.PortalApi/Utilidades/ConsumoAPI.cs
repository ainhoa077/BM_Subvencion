using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Utilidades
{
    public class ConsumoAPI
    {
        public static string EjecutarAPI(string url, string token, dynamic content)
        {
            string apiResponse = string.Empty;

            using (var httpClientHandler = new HttpClientHandler())
            {
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Add("Cookie", "csrftoken=AObtlIXSAQpV2WZfurP3LJnMdey9UdXsRmbtEJC4Btbfg8fRw2VisA9qGdz4");
                        httpClient.DefaultRequestHeaders.Add("F1MN", "sessionid=686jjtdqp46fzsd2hn77l1e9a8ipqnky");
                    }

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
