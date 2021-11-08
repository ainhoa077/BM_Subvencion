using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PWC.Subvenciones.PortalApi.Modelos;
using PWC.Subvenciones.PortalApi.Utilidades;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PWC.Subvenciones.PortalApi.Acciones
{
    public class AccionGenerarUrlPago: AccionPaycometBase
    {
        private const string SIGLASORDEN = "PWC-";
        private readonly IOrganizationService ServicioCRM;

        public AccionGenerarUrlPago(IOrganizationService service, IPluginExecutionContext contextoEjecucion): base(service, contextoEjecucion)
        {
            ServicioCRM = service;
        }

        public string Ejecutar(string parametros)
        {
            ParametroEntradaPagoPaycomet parametroEntradaPagoPaycomet = JsonSerializer.Deserialize<ParametroEntradaPagoPaycomet>(parametros);

            if (!string.IsNullOrEmpty(parametroEntradaPagoPaycomet.solicitudid))
            {
                Entity solicitud = ObtenerSolicitud(parametroEntradaPagoPaycomet);
                EntityCollection parametrosPaycomet = ObtenerParametrosPycomet();
                string tokenApi = BuscarParametro(parametrosPaycomet, "PAYCOMET-API-TOKEN");
                string terminal = BuscarParametro(parametrosPaycomet, "PAYCOMET-TERMINAL");
                string urlOK = BuscarParametro(parametrosPaycomet, "PAYCOMET-URLOK");
                string urlKO = BuscarParametro(parametrosPaycomet, "PAYCOMET-URLKO");
                string url = BuscarParametro(parametrosPaycomet, "PAYCOMET-URL-PAGO");
                string transaccionPagoId = CrearTransaccionPago(solicitud);
                return ConsumoApiPayComent(url, tokenApi, terminal, urlOK, urlKO, solicitud, transaccionPagoId);
            }
            else
            {
                ParametrosSalidaGenerarUrlPagoSabadell parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPagoSabadell();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 1;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "Request inválido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPagoSabadell>(parametrosSalidaGenerarUrlPagoSabadell);
            }
        }

        private void ActualizacionUrlPagoTransaccionPago(string transaccionPagoId, ParametrosSalidaGenerarUrlPagoSabadell respuestaUrlPago, string peticion, string urlOK, string urlKO)
        {
            if (respuestaUrlPago != null && respuestaUrlPago.errorCode == 0)
            {
                Entity transaccionPagoActualizar = new Entity("crcd6_transaccionespagos");
                transaccionPagoActualizar.Id = Guid.Parse(transaccionPagoId);
                transaccionPagoActualizar.Attributes["crcd6_urlpago"] = respuestaUrlPago.challengeUrl;
                transaccionPagoActualizar.Attributes["crcd6_peticion"] = peticion;
                transaccionPagoActualizar.Attributes["crcd6_urlok"] = urlOK;
                transaccionPagoActualizar.Attributes["crcd6_urlko"] = urlKO;
                Service.Update(transaccionPagoActualizar);
            }
        }

        private string CrearTransaccionPago(Entity solicitud)
        {
            Entity transaccionPago = new Entity("crcd6_transaccionespagos");
            transaccionPago.Attributes["crcd6_nombre"] = $"{SIGLASORDEN}{solicitud.Attributes["crcd6_solicitud"].ToString()}";
            transaccionPago.Attributes["crcd6_solicitudid"] = new EntityReference("crcd6_solicitudsub", solicitud.Id);
            Guid transaccionesPagoId = Service.Create(transaccionPago);
            return transaccionesPagoId.ToString();
        }

        private Entity ObtenerSolicitud(ParametroEntradaPagoPaycomet parametrosEntradaPagoPaycomet)
        {
            Entity entidadSolcitud = null;
            QueryExpression solicitud = new QueryExpression("crcd6_solicitudsub");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("crcd6_solicitud", "crcd6_importepresupuestado");
            solicitud.Criteria.AddCondition("crcd6_solicitudsubid", ConditionOperator.Equal, parametrosEntradaPagoPaycomet.solicitudid);
            EntityCollection respuesta = Service.RetrieveMultiple(solicitud);

            if(respuesta != null && respuesta.Entities.Count > 0)
            {
                entidadSolcitud = respuesta.Entities[0];
            }

            return entidadSolcitud;
        }

        private string ConsumoApiPayComent(string url, string tokenApi, string terminal, string urlOK, string urlKO, Entity solicitud, string transaccionPagoId)
        {
            if (solicitud != null)
            {
                urlOK = $"{urlOK}?transaccionId={transaccionPagoId}&solicitudId={solicitud.Id.ToString()}";
                urlKO = $"{urlKO}?transaccionId={transaccionPagoId}";
                PagoPaycomet peticion = new PagoPaycomet();
                peticion.operationType = 1;
                peticion.language = "es";
                peticion.terminal = int.Parse(terminal);
                Pago pago = new Pago();
                pago.order = SIGLASORDEN + solicitud.Attributes["crcd6_solicitud"].ToString();
                pago.secure = 1;
                pago.terminal = int.Parse(terminal);
                pago.urlKo = urlKO;
                pago.urlOk = urlOK;
                pago.currency = "EUR";
                int totalAmount = Decimal.ToInt32(((Money)solicitud.Attributes["crcd6_importepresupuestado"]).Value * 100);
                pago.amount = totalAmount.ToString();
                peticion.payment = pago;
                string parametroEntrada = JsonSerializer.Serialize<PagoPaycomet>(peticion).Replace(@"\", "");
                string respuesta = ConsumoAPI.EjecutarAPI(url, tokenApi, parametroEntrada).Replace(@"\", "");
                ParametrosSalidaGenerarUrlPagoSabadell respuestaUrlPago = JsonSerializer.Deserialize<ParametrosSalidaGenerarUrlPagoSabadell>(respuesta);
                ActualizacionUrlPagoTransaccionPago(transaccionPagoId, respuestaUrlPago, parametroEntrada, urlOK, urlKO);
                return respuesta;
            }
            else
            {
                ParametrosSalidaGenerarUrlPagoSabadell parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPagoSabadell();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 2;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "El id de solicitud no es válido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPagoSabadell>(parametrosSalidaGenerarUrlPagoSabadell);
            }
        }
    }
}
