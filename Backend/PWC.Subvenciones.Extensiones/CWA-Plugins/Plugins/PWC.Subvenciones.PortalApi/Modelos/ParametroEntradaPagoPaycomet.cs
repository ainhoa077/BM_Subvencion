using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    [DataContract]
    public class ParametroEntradaPagoPaycomet
    {
        [DataMember(Name = "solicitudid", IsRequired = false)]
        public string solicitudid { get; set; }
    }
}
