using Microsoft.Xrm.Sdk;
using PWC.Subvenciones.PortalApi.Controlador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi
{
    public class PortalApi : IPlugin
    {
        #region Constante
        public const string nombreIniciativa = "Portales";
        #endregion

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext contextoEjecucion = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService servicioCrm = serviceFactory.CreateOrganizationService(null);

            try
            {
                PortalApiController portalApiControlador = new PortalApiController(servicioCrm, contextoEjecucion);
                portalApiControlador.EjecutarControlador();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
