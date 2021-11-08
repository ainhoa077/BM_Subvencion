using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    public class ParametrosSalidaGenerarUrlPagoSabadell
    {
        public string challengeUrl { get; set; }
        public int errorCode { get; set; }
        public string messageError { get; set; }
    }
}
