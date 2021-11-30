using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using PWC.Subvenciones.PortalApi.Modelos;
using PWC.Subvenciones.PortalApi.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonSerializer = PWC.Subvenciones.PortalApi.Utilidades.JsonSerializer;

namespace PWC.Subvenciones.PortalApi.Acciones
{
    public class AccionConsultarPago: AccionRedsysBase
    {
        private const string ESTAOPENDIENTEPAGO = "4";
        public AccionConsultarPago(IOrganizationService service, IPluginExecutionContext contextoEjecucion) : base(service, contextoEjecucion)
        {
        }

        public string Ejecutar(string parametros)
        {
            ParametroEntradaConsultaPago parametroEntradaConsultaPago = JsonSerializer.Deserialize<ParametroEntradaConsultaPago>(parametros);

            if (!string.IsNullOrEmpty(parametroEntradaConsultaPago.transaccionPagoId))
            {
                Entity transaccionPago = ObtenerTransaccionPago(parametroEntradaConsultaPago.transaccionPagoId);
                return ConsumoApiConsultaRedsys(transaccionPago, parametroEntradaConsultaPago.transaccionExitosa);
            }
            else
            {
                ParametrosSalidaGenerarUrlPago parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPago();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 1;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "Request inválido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPago>(parametrosSalidaGenerarUrlPagoSabadell);
            }
        }

        private Entity ObtenerTransaccionPago(string transaccionPagoId)
        {
            Entity entidadTransaccionPago = null;
            QueryExpression solicitud = new QueryExpression("crcd6_transaccionespagos");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("crcd6_nombre", "crcd6_solicitudid");
            solicitud.Criteria.AddCondition("crcd6_transaccionespagosid", ConditionOperator.Equal, transaccionPagoId);
            EntityCollection respuesta = Service.RetrieveMultiple(solicitud);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                entidadTransaccionPago = respuesta.Entities[0];
            }

            return entidadTransaccionPago;
        }

        private string ConsumoApiConsultaRedsys(Entity transaccionPago, bool transaccionExitosa)
        {
            if (transaccionPago != null)
            {
                ParametrosSalidaConsultaPago respuestaConsultaPago = new ParametrosSalidaConsultaPago();
                respuestaConsultaPago.errorCode = 0;
                respuestaConsultaPago.payment = new ConsultaPagoDetalle();
                respuestaConsultaPago.payment.order = transaccionPago.Attributes["crcd6_nombre"].ToString();
                ActualizacionUrlPagoTransaccionPago(transaccionPago, respuestaConsultaPago, transaccionExitosa);
                return JsonConvert.SerializeObject(respuestaConsultaPago, Formatting.None);
            }
            else
            {
                ParametrosSalidaGenerarUrlPago parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPago();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 2;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "El id de transacción de pago no es válido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPago>(parametrosSalidaGenerarUrlPagoSabadell);
            }
        }

        private Entity ConsultarIdEstadoSolicitud(string codigoEstado)
        {
            Entity entidadEstado = null;
            QueryExpression estado = new QueryExpression("crcd6_estado");
            estado.NoLock = true;
            estado.ColumnSet = new ColumnSet("crcd6_estadoid");
            estado.Criteria.AddCondition("crcd6_codigo", ConditionOperator.Equal, codigoEstado);
            EntityCollection respuesta = Service.RetrieveMultiple(estado);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                entidadEstado = respuesta.Entities[0];
            }

            return entidadEstado;
        }

        private void ActualizacionEstadoSolicitudPendientePago(Entity transaccionPago)
        {
            Entity estado = ConsultarIdEstadoSolicitud(ESTAOPENDIENTEPAGO);
            Entity solicitudActualiza = new Entity("crcd6_solicitudsub");
            solicitudActualiza.Id = ((EntityReference)transaccionPago.Attributes["crcd6_solicitudid"]).Id;
            solicitudActualiza.Attributes["crcd6_estado"] = new EntityReference("crcd6_estado", estado.Id);
            Service.Update(solicitudActualiza);
        }

        private void ActualizacionUrlPagoTransaccionPago(Entity transaccionPago, ParametrosSalidaConsultaPago respuestConsultaPago, bool transaccionExitosa)
        {
            if (respuestConsultaPago != null && respuestConsultaPago.errorCode == 0)
            {
                Entity transaccionPagoActualizar = new Entity("crcd6_transaccionespagos");
                transaccionPagoActualizar.Id = transaccionPago.Id;

                if (transaccionExitosa)
                {
                    transaccionPagoActualizar.Attributes["crcd6_codigoestado"] = 1;
                    transaccionPagoActualizar.Attributes["crcd6_nombreestado"] = "Completo";
                    transaccionPagoActualizar.Attributes["crcd6_fechatransaccionpago"] = DateTime.UtcNow;
                    ActualizacionEstadoSolicitudPendientePago(transaccionPago);
                }
                else
                {
                    transaccionPagoActualizar.Attributes["crcd6_codigoestado"] = 0;
                    transaccionPagoActualizar.Attributes["crcd6_nombreestado"] = "Fallido";
                    transaccionPagoActualizar.Attributes["crcd6_fechatransaccionpago"] = DateTime.UtcNow;
                }

                Service.Update(transaccionPagoActualizar);
            }
        }
    }
}
