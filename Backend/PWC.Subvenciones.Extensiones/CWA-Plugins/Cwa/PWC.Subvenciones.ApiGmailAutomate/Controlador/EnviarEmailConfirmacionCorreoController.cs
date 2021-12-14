using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using PWC.Subvenciones.ApiGmailAutomate.Modelos;
using PWC.Subvenciones.ApiGmailAutomate.Utilidades;
using System;

namespace PWC.Subvenciones.ApiGmailAutomate.Controlador
{
    public class EnviarEmailConfirmacionCorreoController : ApiGmailBaseController
    {
        public EnviarEmailConfirmacionCorreoController(IOrganizationService service, IWorkflowContext contextoEjecucion): base(service, contextoEjecucion)
        {
        }

        public void EjecutarControlador(Guid contactoId, string callbackUrl)
        {
            Entity contacto = ObtenerEmail(contactoId);

            if (contacto.Attributes.ContainsKey("emailaddress1"))
            {
                EntityCollection parametrosTwilio = ObtenerParametrosGmailApi();
                string automateUrl = BuscarParametro(parametrosTwilio, "GMAIL-AUTOMATEURL");
                string asunto = BuscarParametro(parametrosTwilio, "GMAIL-ASUNTOCOMFIRMACION");
                string cuerpo = BuscarParametro(parametrosTwilio, "GMAIL-CUERPOCOMFIRMACION");
                cuerpo = cuerpo.Replace("{callbackUrl}", callbackUrl);
                RequestCorreo peticion = new RequestCorreo();
                peticion.emailaddress = contacto.Attributes["emailaddress1"].ToString();
                peticion.emailSubject = asunto;
                peticion.emailBody = cuerpo;
                string parametroEntrada = JsonConvert.SerializeObject(peticion).Replace(@"\", "");
                Entity motivo = new Entity("crcd6_motivollamada");
                motivo.Attributes["crcd6_comentarios"] = parametroEntrada;
                Service.Create(motivo);
                string respuesta = ConsumoAPI.EjecutarAPI(automateUrl, parametroEntrada);
                Entity motivo1 = new Entity("crcd6_motivollamada");
                motivo1.Attributes["crcd6_comentarios"] = respuesta;
                Service.Create(motivo1);
            }
        }
    }
}
