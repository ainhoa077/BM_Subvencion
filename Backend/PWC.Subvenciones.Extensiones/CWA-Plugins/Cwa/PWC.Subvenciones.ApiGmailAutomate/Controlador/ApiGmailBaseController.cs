using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.ApiGmailAutomate.Controlador
{
    public class ApiGmailBaseController
    {
        public IOrganizationService Service { get; set; }
        public IWorkflowContext ContextoEjecucion { get; set; }

        public ApiGmailBaseController(IOrganizationService service, IWorkflowContext contextoEjecucion)
        {
            Service = service;
            ContextoEjecucion = contextoEjecucion;
        }

        protected EntityCollection ObtenerParametrosGmailApi()
        {
            QueryExpression parametrosTwilio = new QueryExpression("adx_sitesetting");
            parametrosTwilio.NoLock = true;
            parametrosTwilio.ColumnSet = new ColumnSet("adx_name", "adx_value");
            parametrosTwilio.Criteria.AddCondition("adx_name", ConditionOperator.Like, "%GMAIL%");
            return Service.RetrieveMultiple(parametrosTwilio);
        }

        protected Entity ObtenerEmail(Guid contactoId)
        {
            Entity entidadSolcitud = null;
            QueryExpression solicitud = new QueryExpression("contact");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("emailaddress1", "adx_identity_username");
            solicitud.Criteria.AddCondition("contactid", ConditionOperator.Equal, contactoId.ToString());
            EntityCollection respuesta = Service.RetrieveMultiple(solicitud);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                entidadSolcitud = respuesta.Entities[0];
            }

            return entidadSolcitud;
        }

        protected string BuscarParametro(EntityCollection parametrosTwilio, string keyParametro)
        {
            string ValorParametro = string.Empty;

            if (parametrosTwilio != null && parametrosTwilio.Entities.Count > 0)
            {
                Entity parametro = parametrosTwilio.Entities.Where(x => x.Attributes["adx_name"].ToString().Equals(keyParametro)).FirstOrDefault();
                ValorParametro = parametro.Attributes["adx_value"].ToString();
            }

            return ValorParametro;
        }
    }
}
