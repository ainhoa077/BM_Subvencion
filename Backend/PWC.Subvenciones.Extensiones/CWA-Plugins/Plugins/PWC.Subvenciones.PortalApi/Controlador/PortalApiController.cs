using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PWC.Subvenciones.Extensiones.Controlador;
using PWC.Subvenciones.PortalApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PWC.Subvenciones.PortalApi.Controlador
{
    public class PortalApiController
    {
        public IOrganizationService Service { get; set; }
        public IPluginExecutionContext ContextoEjecucion { get; set; }
        public IActionOrquestationApi ActionOrquestationApi { get; set; }

        public PortalApiController(IOrganizationService service, IPluginExecutionContext contextoEjecucion)
        {
            Service = service;
            ContextoEjecucion = contextoEjecucion;
        }

        public void EjecutarControlador()
        {
            FetchExpression currentQuery = ContextoEjecucion.InputParameters["Query"] as FetchExpression;

            if (currentQuery == null)
            {
                return;
            }

            XDocument parsedQuery = XDocument.Parse(currentQuery.Query);
            Dictionary<string, string> requestParameters = this.ComponerParametrosDeConsulta(parsedQuery);
            ActionOrquestationApi = InstanciarControladorConcreto(requestParameters);
            ActionOrquestationApi.EjecutarControlador();
        }

        private IActionOrquestationApi InstanciarControladorConcreto(Dictionary<string, string> requestParameters)
        {
                return new ActionOrquestationApiController(Service, ContextoEjecucion, requestParameters);
        }

        private Dictionary<string, string> ComponerParametrosDeConsulta(XDocument parsedQuery)
        {
            Dictionary<string, string> requestParameters = parsedQuery
                .Descendants("condition")
                .Where(e =>
                    e.Attribute("attribute") != null &&
                    e.Attribute("operator") != null &&
                    e.Attribute("value") != null &&
                    String.Equals(e.Attribute("operator").Value, "eq", StringComparison.InvariantCultureIgnoreCase))
                .ToDictionary(e => e.Attribute("attribute").Value, e => e.Attribute("value").Value);

            return requestParameters;
        }
    }
}
