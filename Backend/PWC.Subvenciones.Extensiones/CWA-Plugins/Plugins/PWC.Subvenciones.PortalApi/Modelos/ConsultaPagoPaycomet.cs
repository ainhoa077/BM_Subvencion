using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    [DataContract]
    public class ConsultaPagoPaycomet
    {
        [DataMember(Name = "payment", IsRequired = false)]
        public ConsultaPago payment { get; set; }
    }
}
