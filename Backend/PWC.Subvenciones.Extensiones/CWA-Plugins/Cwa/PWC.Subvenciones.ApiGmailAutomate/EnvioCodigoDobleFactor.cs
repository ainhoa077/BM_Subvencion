using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using PWC.Subvenciones.ApiGmailAutomate.Controlador;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.ApiGmailAutomate
{
    public class EnvioCodigoDobleFactor: CodeActivity
    {
        [Input("Codigo")]
        public InArgument<string> Codigo { get; set; }

        public EnvioCodigoDobleFactor()
        {

        }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext contextoEjecucion = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService servicioCrm = serviceFactory.CreateOrganizationService(null);

            try
            {
                string codigo = Codigo.Get(context);
                EnviarCodigoEmailController enviarCodigoEmailController = new EnviarCodigoEmailController(servicioCrm, contextoEjecucion);
                enviarCodigoEmailController.EjecutarControlador(contextoEjecucion.PrimaryEntityId, codigo);
            }
            catch (Exception ex)
            {
            }
        }        
    }
}
