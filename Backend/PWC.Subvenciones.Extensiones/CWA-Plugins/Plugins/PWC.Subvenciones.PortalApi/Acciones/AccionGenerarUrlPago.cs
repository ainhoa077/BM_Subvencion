using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using PWC.Subvenciones.PortalApi.Api;
using PWC.Subvenciones.PortalApi.Modelos;
using PWC.Subvenciones.PortalApi.Utilidades;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using JsonSerializer = PWC.Subvenciones.PortalApi.Utilidades.JsonSerializer;

namespace PWC.Subvenciones.PortalApi.Acciones
{
    public class AccionGenerarUrlPago : AccionRedsysBase
    {
        private const string SIGLASORDEN = "0{0}";
        private readonly IOrganizationService ServicioCRM;

        public AccionGenerarUrlPago(IOrganizationService service, IPluginExecutionContext contextoEjecucion) : base(service, contextoEjecucion)
        {
            ServicioCRM = service;
        }

        public string Ejecutar(string parametros)
        {
            ParametroEntradaPago parametroEntradaPago = JsonSerializer.Deserialize<ParametroEntradaPago>(parametros);

            if (!string.IsNullOrEmpty(parametroEntradaPago.solicitudid))
            {
                Entity solicitud = ObtenerSolicitud(parametroEntradaPago);
                EntityCollection parametrosPaycomet = ObtenerParametrosRedsys();
                string tokenApi = BuscarParametro(parametrosPaycomet, "REDSYS-API-TOKEN");
                string merchantCode = BuscarParametro(parametrosPaycomet, "REDSYS-MERCHANTCODE");
                string merchantUrl = BuscarParametro(parametrosPaycomet, "REDSYS-MERCHANTURL");
                string terminal = BuscarParametro(parametrosPaycomet, "REDSYS-TERMINAL");
                string urlOK = BuscarParametro(parametrosPaycomet, "REDSYS-URLOK");
                string urlKO = BuscarParametro(parametrosPaycomet, "REDSYS-URLKO");
                string url = BuscarParametro(parametrosPaycomet, "REDSYS-URL-PAGO");
                string siglaOrder = SIGLASORDEN.Replace("{0}", DateTime.Now.ToString("ss"));
                string transaccionPagoId = CrearTransaccionPago(solicitud, siglaOrder);
                return ConsumoApiRedsys(url, tokenApi, terminal, urlOK, urlKO, solicitud, transaccionPagoId, merchantCode, merchantUrl, siglaOrder);
            }
            else
            {
                ParametrosSalidaGenerarUrlPago parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPago();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 1;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "Request inválido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPago>(parametrosSalidaGenerarUrlPagoSabadell);
            }
        }

        private void ActualizacionUrlPagoTransaccionPago(string transaccionPagoId, string respuestaUrlPago, string peticion, string urlOK, string urlKO)
        {
            Entity transaccionPagoActualizar = new Entity("crcd6_transaccionespagos");
            transaccionPagoActualizar.Id = Guid.Parse(transaccionPagoId);
            transaccionPagoActualizar.Attributes["crcd6_urlpago"] = respuestaUrlPago;
            transaccionPagoActualizar.Attributes["crcd6_peticion"] = peticion;
            transaccionPagoActualizar.Attributes["crcd6_urlok"] = urlOK;
            transaccionPagoActualizar.Attributes["crcd6_urlko"] = urlKO;
            Service.Update(transaccionPagoActualizar);
        }

        private string CrearTransaccionPago(Entity solicitud, string siglaOrder)
        {
            Entity transaccionPago = new Entity("crcd6_transaccionespagos");
            transaccionPago.Attributes["crcd6_nombre"] = $"{siglaOrder}{solicitud.Attributes["crcd6_solicitud"].ToString()}";
            transaccionPago.Attributes["crcd6_solicitudid"] = new EntityReference("crcd6_solicitudsub", solicitud.Id);
            Guid transaccionesPagoId = Service.Create(transaccionPago);
            return transaccionesPagoId.ToString();
        }

        private Entity ObtenerSolicitud(ParametroEntradaPago parametrosEntradaPagoPaycomet)
        {
            Entity entidadSolcitud = null;
            QueryExpression solicitud = new QueryExpression("crcd6_solicitudsub");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("crcd6_solicitud", "crcd6_importepresupuestado");
            solicitud.Criteria.AddCondition("crcd6_solicitudsubid", ConditionOperator.Equal, parametrosEntradaPagoPaycomet.solicitudid);
            EntityCollection respuesta = Service.RetrieveMultiple(solicitud);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                entidadSolcitud = respuesta.Entities[0];
            }

            return entidadSolcitud;
        }

        private string ConsumoApiRedsys(string url, string tokenApi, string terminal, string urlOK, string urlKO, Entity solicitud, string transaccionPagoId, string merchantCode, string merchantUrl, string siglaOrder)
        {
            if (solicitud != null)
            {
                urlOK = $"{urlOK}?transaccionId={transaccionPagoId}&solicitudId={solicitud.Id.ToString()}";
                urlKO = $"{urlKO}?transaccionId={transaccionPagoId}";
                int totalAmount = Decimal.ToInt32(((Money)solicitud.Attributes["crcd6_importepresupuestado"]).Value * 100);
                RedsysAPI api = new RedsysAPI();
                string key = tokenApi;
                string order = $"{siglaOrder}{solicitud.Attributes["crcd6_solicitud"].ToString()}";
                order = order.Replace("{0}", DateTime.Now.ToString("tt"));
                api.SetParameter("DS_MERCHANT_AMOUNT", totalAmount.ToString());
                api.SetParameter("DS_MERCHANT_ORDER", order.Replace("-", ""));
                api.SetParameter("DS_MERCHANT_MERCHANTCODE", merchantCode);
                api.SetParameter("DS_MERCHANT_CURRENCY", "978");
                api.SetParameter("DS_MERCHANT_TRANSACTIONTYPE", "0");
                api.SetParameter("DS_MERCHANT_TERMINAL", terminal);
                api.SetParameter("DS_MERCHANT_MERCHANTURL", merchantUrl);
                api.SetParameter("DS_MERCHANT_URLOK", urlOK);
                api.SetParameter("DS_MERCHANT_URLKO", urlKO);
                string parametros = api.createMerchantParameters();
                string firma = api.createMerchantSignature(key);
                ParametrosSalidaGenerarUrlPago respuestaUrlPago = new ParametrosSalidaGenerarUrlPago();
                respuestaUrlPago.Parametros = parametros;
                respuestaUrlPago.Firma = firma;
                respuestaUrlPago.UrlPago = url;
                respuestaUrlPago.errorCode = 0;
                respuestaUrlPago.messageError = string.Empty;
                ActualizacionUrlPagoTransaccionPago(transaccionPagoId, url, api.ToJson(api.m_keyvalues), urlOK, urlKO);
                return JsonConvert.SerializeObject(respuestaUrlPago, Formatting.None);
            }
            else
            {
                ParametrosSalidaGenerarUrlPago parametrosSalidaGenerarUrlPagoSabadell = new ParametrosSalidaGenerarUrlPago();
                parametrosSalidaGenerarUrlPagoSabadell.errorCode = 2;
                parametrosSalidaGenerarUrlPagoSabadell.messageError = "El id de solicitud no es válido";
                return JsonSerializer.Serialize<ParametrosSalidaGenerarUrlPago>(parametrosSalidaGenerarUrlPagoSabadell);
            }
        }
    }
}
