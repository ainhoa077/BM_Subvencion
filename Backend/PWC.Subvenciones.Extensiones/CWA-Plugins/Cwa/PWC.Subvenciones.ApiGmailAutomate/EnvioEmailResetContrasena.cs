using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using PWC.Subvenciones.ApiGmailAutomate.Controlador;
using System;
using System.Activities;

namespace PWC.Subvenciones.ApiGmailAutomate
{
    public class EnvioEmailResetContrasena : CodeActivity
    {
        [Input("CallbackUrl")]
        public InArgument<string> CallbackUrl { get; set; }

        [Input("Codigo")]
        public InArgument<string> Codigo { get; set; }

        public EnvioEmailResetContrasena()
        {

        }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext contextoEjecucion = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService servicioCrm = serviceFactory.CreateOrganizationService(null);

            try
            {
                string resetUrl = CallbackUrl.Get(context);
                EnviarEmailResetContrasenaController enviarEmailResetContrasenaController = new EnviarEmailResetContrasenaController(servicioCrm, contextoEjecucion);
                enviarEmailResetContrasenaController.EjecutarControlador(contextoEjecucion.PrimaryEntityId, resetUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}
