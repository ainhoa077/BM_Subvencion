using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PWC.Subvenciones.PortalApi.Modelos;
using PWC.Subvenciones.PortalApi.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.PortalApi.Acciones
{
    public class AccionConsultarPago: AccionPaycometBase
    {
        private const string ESTAOPENDIENTEPAGO = "4";
        public AccionConsultarPago(IOrganizationService service, IPluginExecutionContext contextoEjecucion) : base(service, contextoEjecucion)
        {
        }

        public string Ejecutar(string parametros)
        {
            ParametroEntradaConsultaPagoPaycomet parametroEntradaConsultaPagoPaycomet = JsonSerializer.Deserialize<ParametroEntradaConsultaPagoPaycomet>(parametros);

            if (!string.IsNullOrEmpty(parametroEntradaConsultaPagoPaycomet.transaccionPagoId))
            {
                Entity transaccionPago = ObtenerTransaccionPago(parametroEntradaConsultaPagoPaycomet.transaccionPagoId);
                EntityCollection parametrosPaycomet = ObtenerParametrosPycomet();
                string tokenApi = BuscarParametro(parametrosPaycomet, "PAYCOMET-API-TOKEN");
                string terminal = BuscarParametro(parametrosPaycomet, "PAYCOMET-TERMINAL");
                string url = BuscarParametro(parametrosPaycomet, "PAYCOMET-URL-CONSULTA");
                return ConsumoApiConsultaPayComent(url, tokenApi, terminal, transaccionPago, parametroEntradaConsultaPagoPaycomet.transaccionExitosa);
            }
            else
            {
                ParametrosSalidaGenerarUrlPagoSabadell parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPagoSabadell();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 1;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "Request inválido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPagoSabadell>(parametrosSalidaGenerarUrlPagoSabadell);
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

        private string ConsumoApiConsultaPayComent(string url, string tokenApi, string terminal, Entity transaccionPago, bool transaccionExitosa)
        {
            if (transaccionPago != null)
            {
                ConsultaPagoPaycomet peticion = new ConsultaPagoPaycomet();
                peticion.payment = new ConsultaPago();
                peticion.payment.terminal = terminal;
                string parametroEntrada = JsonSerializer.Serialize<ConsultaPagoPaycomet>(peticion).Replace(@"\", "");
                string respuesta = ConsumoAPI.EjecutarAPI(url.Replace("{order}", transaccionPago.Attributes["crcd6_nombre"].ToString()), tokenApi, parametroEntrada).Replace(@"\", "");
                ParametrosSalidaConsultaPago respuestaConsultaPago = JsonSerializer.Deserialize<ParametrosSalidaConsultaPago>(respuesta);
                ActualizacionUrlPagoTransaccionPago(transaccionPago, respuestaConsultaPago, transaccionExitosa);
                return respuesta;
            }
            else
            {
                ParametrosSalidaGenerarUrlPagoSabadell parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPagoSabadell();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 2;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "El id de transacción de pago no es válido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPagoSabadell>(parametrosSalidaGenerarUrlPagoSabadell);
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
                    HistorialPago pagoExitoso = respuestConsultaPago.payment.history.Where(x => x.state == 1).FirstOrDefault();
                    transaccionPagoActualizar.Attributes["crcd6_codigoestado"] = pagoExitoso != null ? pagoExitoso.state : 0;
                    transaccionPagoActualizar.Attributes["crcd6_nombreestado"] = pagoExitoso != null ? pagoExitoso.stateName : string.Empty;
                    transaccionPagoActualizar.Attributes["crcd6_fechatransaccionpago"] = pagoExitoso != null ? DateTime.ParseExact(pagoExitoso.timestamp, "yyyyMMddHHmmss", null) : DateTime.UtcNow;

                    if (pagoExitoso != null)
                    {
                        ActualizacionEstadoSolicitudPendientePago(transaccionPago);
                    }
                }
                else
                {
                    HistorialPago pagoFallido = respuestConsultaPago.payment.history.Where(x => x.state == 0).FirstOrDefault();
                    transaccionPagoActualizar.Attributes["crcd6_codigoestado"] = pagoFallido != null ? pagoFallido.state : 0;
                    transaccionPagoActualizar.Attributes["crcd6_nombreestado"] = pagoFallido != null ? pagoFallido.stateName : string.Empty;
                    transaccionPagoActualizar.Attributes["crcd6_fechatransaccionpago"] = pagoFallido != null ? DateTime.ParseExact(pagoFallido.timestamp, "yyyyMMddHHmmss", null) : DateTime.UtcNow;
                }

                Service.Update(transaccionPagoActualizar);
            }
        }
    }
}
