using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Acciones
{
    public class AccionGenerarUrlPagoSabadell
    {
        public IOrganizationService Service { get; set; }
        public IPluginExecutionContext ContextoEjecucion { get; set; }

        public AccionGenerarUrlPagoSabadell(IOrganizationService service, IPluginExecutionContext contextoEjecucion)
        {
            Service = service;
            ContextoEjecucion = contextoEjecucion;
        }

        public string Ejecutar(string parametros)
        {
            //ParametrosConsultaAdjuntarDocumento parametrosAdjuntarDocumento = JsonConvert.DeserializeObject<ParametrosConsultaAdjuntarDocumento>(parametros);

            //OrganizationRequest organizationRequest = new OrganizationRequest("epm_td_adjuntardocumento")
            //{
            //    Parameters =
            //    {
            //        {
            //            "idRadicado", parametrosAdjuntarDocumento.idRadicado
            //        },
            //        {
            //            "NombreImagen", parametrosAdjuntarDocumento.NombreImagen
            //        },
            //        {
            //            "ImagenEncodeBase64", parametrosAdjuntarDocumento.ImagenEncodeBase64
            //        },
            //    }
            //};

            //OrganizationResponse response = ProxyCrm.ServicioCrm.Execute(organizationRequest);

            //ParametrosSalidaAdjuntarDocumento objsalida =
            //    new ParametrosSalidaAdjuntarDocumento
            //    {
            //        HuboErrorAdjuntar = (bool)response.Results["HuboError"],
            //        MensajeErrorAdjuntar = response.Results["MensajeError"].ToString()
            //    };

            //string resultado = JsonConvert.SerializeObject(objsalida);
            string resultado = "Exitoso";

            return resultado;
        }
    }
}
