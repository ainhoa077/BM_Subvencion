using Microsoft.Xrm.Sdk;
using PWC.Subvenciones.PortalApi.Acciones;
using PWC.Subvenciones.PortalApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.Extensiones.Controlador
{
    public class ActionOrquestationApiController: IActionOrquestationApi
    {
        private readonly IDictionary<string, Func<string, string>> Api;
        private Dictionary<string, string> RequestParameters { get; }
        public IOrganizationService Service { get; set; }
        public IPluginExecutionContext ContextoEjecucion { get; set; }

        public ActionOrquestationApiController(IOrganizationService service, IPluginExecutionContext contextoEjecucion, Dictionary<string, string> requestParameters)
        {
            Service = service;
            ContextoEjecucion = contextoEjecucion;
            RequestParameters = requestParameters;

            Api = new Dictionary<string, Func<string, string>>()
            {
                { "AccionGenerarUrlPago", new AccionGenerarUrlPago(service, contextoEjecucion).Ejecutar },
                { "AccionConsultarPago", new AccionConsultarPago(service, contextoEjecucion).Ejecutar },
                { "AccionSesionFandit", new AccionSesionFandit(service, contextoEjecucion).Ejecutar },
            };
        }
        public void EjecutarControlador()
        {
            this.EjecutarAccionCompuesta(this.RequestParameters);
        }

        private void EjecutarAccionCompuesta(Dictionary<string, string> requestParameters)
        {
            if (!requestParameters.ContainsKey("crcd6_api") ||
                !requestParameters.ContainsKey("crcd6_peticion") ||
                !Api.ContainsKey(requestParameters["crcd6_api"]))
            {
                return;
            }

            string peticion = Encoding.UTF8.GetString(Convert.FromBase64String(requestParameters["crcd6_peticion"]));
            string respuesta = Api[requestParameters["crcd6_api"]](peticion);
            EntityCollection restulCollection = ContextoEjecucion.OutputParameters["BusinessEntityCollection"] as EntityCollection;

            if (restulCollection == null)
            {
                return;
            }
  
            Guid id = Guid.NewGuid();
            restulCollection.Entities.Add(
                new Entity("crcd6_portalapi")
                {
                    Id = id,
                    Attributes =
                    {
                        { "crcd6_portalapiid", id },
                        { "crcd6_respuesta", respuesta }
                    }
                });
        }
    }
}
