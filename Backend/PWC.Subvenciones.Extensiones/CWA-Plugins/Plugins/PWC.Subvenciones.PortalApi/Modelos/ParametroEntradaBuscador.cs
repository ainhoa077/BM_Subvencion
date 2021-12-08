using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Modelos
{
    [DataContract]
    public class ParametroEntradaBuscador
    {
        [DataMember(Name = "contactId", IsRequired = false)]
        public string ContactId { get; set; }
    }
}
