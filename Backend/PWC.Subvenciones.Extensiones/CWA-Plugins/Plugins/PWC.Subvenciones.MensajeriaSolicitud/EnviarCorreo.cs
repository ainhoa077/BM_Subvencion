using Microsoft.Xrm.Sdk;
using PWC.Subvenciones.MensajeriaSolicitud.Controlador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.MensajeriaSolicitud
{
    public class EnviarCorreo : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IOrganizationService servicioCrm = null; 

            try
            {
                IPluginExecutionContext contextoEjecucion = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                servicioCrm = serviceFactory.CreateOrganizationService(null);

                Entity entity = new Entity();
                if (contextoEjecucion.InputParameters.Contains("Target") && contextoEjecucion.InputParameters["Target"] is Entity)
                {
                    entity = (Entity)contextoEjecucion.InputParameters["Target"];

                    if (entity.LogicalName != "annotation")
                    {
                        return;
                    }
                }
             
                EnvioCorreoController envioCorreoController = new EnvioCorreoController(servicioCrm, contextoEjecucion);
                envioCorreoController.EjecutarControlador();
            }
            catch (Exception ex)
            {
                Entity motivo = new Entity("crcd6_motivollamada");
                motivo.Attributes["crcd6_comentarios"] = ex.Message;
                servicioCrm.Create(motivo);
            }
        }
    }
}
