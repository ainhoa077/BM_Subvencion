using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using PWC.Subvenciones.ApiGmailAutomate.Controlador;
using System;
using System.Activities;

namespace PWC.Subvenciones.ApiGmailAutomate
{
    public class EnvioEmailConfirmacionCorreo : CodeActivity
    {
        [Input("CallbackUrl")]
        public InArgument<string> CallbackUrl { get; set; }

        [Input("Codigo")]
        public InArgument<string> Codigo { get; set; }

        public EnvioEmailConfirmacionCorreo()
        {

        }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext contextoEjecucion = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService servicioCrm = serviceFactory.CreateOrganizationService(null);

            try
            {
                string callbackUrl = CallbackUrl.Get(context);
                EnviarEmailConfirmacionCorreoController enviarEmailConfirmacionCorreoController = new EnviarEmailConfirmacionCorreoController(servicioCrm, contextoEjecucion);
                enviarEmailConfirmacionCorreoController.EjecutarControlador(contextoEjecucion.PrimaryEntityId, callbackUrl);
            }
            catch (Exception ex)
            {
                
            }
        }        
    }
}
