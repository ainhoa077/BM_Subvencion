using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using PWC.Subvenciones.ApiGmailAutomate.Modelos;
using PWC.Subvenciones.ApiGmailAutomate.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.ApiGmailAutomate.Controlador
{
    public class EnviarCodigoEmailController : ApiGmailBaseController
    {
        public EnviarCodigoEmailController(IOrganizationService service, IWorkflowContext contextoEjecucion): base(service, contextoEjecucion)
        {
        }
        public void EjecutarControlador(Guid contactoId, string codigo)
        {
            Entity contacto = ObtenerEmail(contactoId);

            if (contacto.Attributes.ContainsKey("emailaddress1"))
            {
                EntityCollection parametrosTwilio = ObtenerParametrosGmailApi();
                string automateUrl = BuscarParametro(parametrosTwilio, "GMAIL-AUTOMATEURL");
                string asunto = BuscarParametro(parametrosTwilio, "GMAIL-ASUNTODOBLEFACTOR");
                string cuerpo = BuscarParametro(parametrosTwilio, "GMAIL-CUERPODOBLEFACTOR");
                cuerpo = cuerpo.Replace("{codigo}", codigo);
                RequestCorreo peticion = new RequestCorreo();
                peticion.emailaddress = contacto.Attributes["emailaddress1"].ToString();
                peticion.emailSubject = asunto;
                peticion.emailBody = cuerpo;
                string parametroEntrada = JsonConvert.SerializeObject(peticion).Replace(@"\", "");
                string respuesta = ConsumoAPI.EjecutarAPI(automateUrl, parametroEntrada);
            }
        }
    }
}
