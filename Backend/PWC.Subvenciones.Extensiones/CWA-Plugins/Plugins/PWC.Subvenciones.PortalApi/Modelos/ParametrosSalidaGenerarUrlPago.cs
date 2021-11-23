using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    public class ParametrosSalidaGenerarUrlPago
    {
        public string Parametros { get; set; }
        public string Firma { get; set; }
        public string UrlPago { get; set; }
        public int errorCode { get; set; }
        public string messageError { get; set; }
    }
}
