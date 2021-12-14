using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.ApiGmailAutomate.Modelos
{
    public class RequestCorreo
    {
        public string emailaddress { get; set; }
        public string emailSubject { get; set; }
        public string emailBody { get; set; }
    }
}
