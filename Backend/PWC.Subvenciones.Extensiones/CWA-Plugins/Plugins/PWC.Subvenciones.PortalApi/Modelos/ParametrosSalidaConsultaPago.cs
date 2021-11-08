using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    public class ParametrosSalidaConsultaPago
    {
        public int errorCode { get; set; }
        public string messageError { get; set; }
        public ConsultaPagoDetalle payment { get; set; }
    }
}
