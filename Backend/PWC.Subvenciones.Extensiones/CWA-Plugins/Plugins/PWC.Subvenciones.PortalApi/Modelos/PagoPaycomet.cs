using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    public class PagoPaycomet
    {
        public int operationType { get; set; }
        public string language { get; set; }
        public int terminal { get; set; }
        public Pago payment { get; set; }
    }
}
