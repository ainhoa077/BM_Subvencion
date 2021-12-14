using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.MensajeriaSolicitud.Controlador
{
    public class EnviarCorreoMensajeriaBaseController
    {
        public IOrganizationService Service { get; set; }
        public IPluginExecutionContext ContextoEjecucion { get; set; }

        public EnviarCorreoMensajeriaBaseController(IOrganizationService service, IPluginExecutionContext contextoEjecucion)
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
