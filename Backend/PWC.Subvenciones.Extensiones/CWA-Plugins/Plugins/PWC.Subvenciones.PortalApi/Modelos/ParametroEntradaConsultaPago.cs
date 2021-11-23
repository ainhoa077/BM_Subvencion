using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    [DataContract]
    public class ParametroEntradaConsultaPago
    {
        public string transaccionPagoId { get; set; }
        public bool transaccionExitosa { get; set; }
    }
}
