using Microsoft.Xrm.Sdk;
using PWC.Subvenciones.Buscador.Controlador;
using PWC.Subvenciones.Buscador.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.Buscador
{
    public class Contratado : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext contextoEjecucion = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService servicioCrm = serviceFactory.CreateOrganizationService(null);

            try
            {
                Entity solicitud = (Entity)contextoEjecucion.InputParameters["Target"];
                IBuscadorContratado buscadorContratadoController = new BuscadorContratadoController(servicioCrm, contextoEjecucion);
                buscadorContratadoController.Ejecutar(solicitud);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
