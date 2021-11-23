using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Acciones
{
    public class AccionRedsysBase
    {
        public IOrganizationService Service { get; set; }
        public IPluginExecutionContext ContextoEjecucion { get; set; }

        public AccionRedsysBase(IOrganizationService service, IPluginExecutionContext contextoEjecucion)
        {
            Service = service;
            ContextoEjecucion = contextoEjecucion;
        }

        public EntityCollection ObtenerParametrosRedsys()
        {
            QueryExpression parametrosPaycomet = new QueryExpression("adx_sitesetting");
            parametrosPaycomet.NoLock = true;
            parametrosPaycomet.ColumnSet = new ColumnSet("adx_name", "adx_value");
            parametrosPaycomet.Criteria.AddCondition("adx_name", ConditionOperator.Like, "%REDSYS%");
            return Service.RetrieveMultiple(parametrosPaycomet);
        }

        public string BuscarParametro(EntityCollection parametrosPaycomet, string keyParametro)
        {
            string ValorParametro = string.Empty;

            if (parametrosPaycomet != null && parametrosPaycomet.Entities.Count > 0)
            {
                Entity parametro = parametrosPaycomet.Entities.Where(x => x.Attributes["adx_name"].ToString().Equals(keyParametro)).FirstOrDefault();
                ValorParametro = parametro.Attributes["adx_value"].ToString();
            }

            return ValorParametro;
        }
    }
}
