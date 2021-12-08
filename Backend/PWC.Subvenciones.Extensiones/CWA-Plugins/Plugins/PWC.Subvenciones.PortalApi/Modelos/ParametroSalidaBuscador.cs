using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    public class ParametroSalidaBuscador
    {
        public string UrlFandit { get; set; }
        public int ErrorCode { get; set; }
        public string MessageError { get; set; }
    }
}
