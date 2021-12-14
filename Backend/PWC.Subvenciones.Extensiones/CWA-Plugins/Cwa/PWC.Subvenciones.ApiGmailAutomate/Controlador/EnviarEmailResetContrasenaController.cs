using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using PWC.Subvenciones.ApiGmailAutomate.Modelos;
using PWC.Subvenciones.ApiGmailAutomate.Utilidades;
using System;

namespace PWC.Subvenciones.ApiGmailAutomate.Controlador
{
    public class EnviarEmailResetContrasenaController : ApiGmailBaseController
    {
        public EnviarEmailResetContrasenaController(IOrganizationService service, IWorkflowContext contextoEjecucion): base(service, contextoEjecucion)
        {
        }

        public void EjecutarControlador(Guid contactoId, string resetUrl)
        {
            Entity contacto = ObtenerEmail(contactoId);

            if (contacto.Attributes.ContainsKey("emailaddress1"))
            {
                EntityCollection parametrosTwilio = ObtenerParametrosGmailApi();
                string automateUrl = BuscarParametro(parametrosTwilio, "GMAIL-AUTOMATEURL");
                string asunto = BuscarParametro(parametrosTwilio, "GMAIL-ASUNTORESET");
                string cuerpo = BuscarParametro(parametrosTwilio, "GMAIL-CUERPORESET");
                cuerpo = cuerpo.Replace("{resetUrl}", resetUrl);
                cuerpo = cuerpo.Replace("{nombreUsuario}", contacto.Attributes["adx_identity_username"].ToString());
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
