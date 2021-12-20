using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PWC.Subvenciones.PortalApi.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using JsonSerializer = PWC.Subvenciones.PortalApi.Utilidades.JsonSerializer;

namespace PWC.Subvenciones.PortalApi.Acciones
{
    public class AccionSesionFandit
    {
        private readonly IOrganizationService ServicioCRM;
        private const string SERVICIOANUALBUSCADOR = "522960007";

        public AccionSesionFandit(IOrganizationService service, IPluginExecutionContext contextoEjecucion)
        {
            ServicioCRM = service;
        }

        public string Ejecutar(string parametros)
        {
            ParametroEntradaBuscador parametroEntradaBuscador = JsonSerializer.Deserialize<ParametroEntradaBuscador>(parametros);
            ParametroSalidaBuscador parametroSalidaBuscador = new ParametroSalidaBuscador();
            string respuesta = string.Empty;

            if (!string.IsNullOrEmpty(parametroEntradaBuscador.ContactId))
            {
                ContactoModelo existeServicioContratrado = ExisteServicioBuscadorContratado(parametroEntradaBuscador.ContactId);

                if (existeServicioContratrado != null)
                {
                    EntityCollection parametrosFandit = ObtenerParametrosFandit();
                    string urlSessionFandit = CrearSesionFandit(existeServicioContratrado, parametrosFandit);

                    if (!string.IsNullOrEmpty(urlSessionFandit))
                    {
                        parametroSalidaBuscador.ErrorCode = 0;
                        parametroSalidaBuscador.UrlFandit = urlSessionFandit;
                    }
                    else
                    {
                        parametroSalidaBuscador.ErrorCode = 1;
                        parametroSalidaBuscador.MessageError = "No se pudo autenticar con el buscador";
                    }
                }
            }
            else
            {
                parametroSalidaBuscador.ErrorCode = 2;
                parametroSalidaBuscador.MessageError = "La estructura del request es incorrecta";
            }

            respuesta = JsonSerializer.Serialize<ParametroSalidaBuscador>(parametroSalidaBuscador);
            return respuesta;
        }

        private string CrearSesionFandit(ContactoModelo contacto, EntityCollection parametrosFandit)
        {
            string urlLogin = BuscarParametro(parametrosFandit, "FANDIT-URL-LOGIN");
            string urlFanditPwc = BuscarParametro(parametrosFandit, "FANDIT-URL-PWC");
            string cookie = BuscarParametro(parametrosFandit, "FANDIT-COOKIE");
            string session = BuscarParametro(parametrosFandit, "FANDIT-F1MN");

            var client = new HttpClient();
            var requestContent = new MultipartFormDataContent();
            client.DefaultRequestHeaders.Add("Cookie", cookie);
            client.DefaultRequestHeaders.Add("F1MN", session);
            var values = new Dictionary<string, string>
            {
                { "email", contacto.CorreoElectronico },
                { "password", contacto.ContrasenaFandit },
            };

            foreach (var keyValuePair in values)
            {
                requestContent.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            }
            var response = client.PostAsync(urlLogin, requestContent).Result;

            if(response.StatusCode == HttpStatusCode.OK)
            {
                return urlFanditPwc.Replace("{token}", response.Content.ReadAsStringAsync().Result);
            }

            return string.Empty;
        }

        private ContactoModelo ExisteServicioBuscadorContratado(string contactId)
        {
            DateTime fechaActual = DateTime.Now.ToLocalTime();
            ContactoModelo contactoModelo = new ContactoModelo();
            QueryExpression solicitud = new QueryExpression("crcd6_solicitudsub");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("crcd6_solicitudsubid", "crcd6_solicitud");
            solicitud.Criteria.AddCondition("crcd6_fechacontratacionsuscripcionbuscador", ConditionOperator.OnOrBefore, fechaActual);
            solicitud.Criteria.AddCondition("crcd6_fechaexpiracionsuscripcionbuscador", ConditionOperator.OnOrAfter, fechaActual);
            LinkEntity joinCliente = JoinClientes(contactId);
            solicitud.LinkEntities.Add(joinCliente);
            LinkEntity joinServicioOfertado = JoinServiciosOfertados();
            solicitud.LinkEntities.Add(joinServicioOfertado);

            EntityCollection respuesta = ServicioCRM.RetrieveMultiple(solicitud);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                Entity entidadSolcitud = respuesta.Entities[0];
                contactoModelo.NombresApellidos = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.fullname").Value.ToString();
                contactoModelo.CorreoElectronico = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.emailaddress1").Value.ToString();
                contactoModelo.Username = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.adx_identity_username").Value.ToString();
                contactoModelo.ContactoId = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.contactid").Value.ToString();
                contactoModelo.ContrasenaFandit = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.crcd6_contrasenafandit").Value.ToString();
            }
            else
            {
                contactoModelo = null;
            }

            return contactoModelo;
        }

        private LinkEntity JoinServiciosOfertados()
        {
            LinkEntity joinServiciosOfertados = new LinkEntity("crcd6_solicitudsub", "crcd6_servicioofertado", "crcd6_serviciosofertados", "crcd6_servicioofertadoid", JoinOperator.Inner)
            {
                EntityAlias = "ServiciosOfertados"
            };

            joinServiciosOfertados.Columns = new ColumnSet("crcd6_codigo");
            joinServiciosOfertados.LinkCriteria = new FilterExpression();
            joinServiciosOfertados.LinkCriteria.FilterOperator = LogicalOperator.And;
            joinServiciosOfertados.LinkCriteria.AddCondition("crcd6_codigo", ConditionOperator.Equal, SERVICIOANUALBUSCADOR);

            return joinServiciosOfertados;
        }

        private LinkEntity JoinClientes(string contactId)
        {
            LinkEntity joinCliente = new LinkEntity("crcd6_solicitudsub", "crcd6_cliente", "crcd6_cliente", "crcd6_clienteid", JoinOperator.Inner)
            {
                EntityAlias = "Clientes"
            };

            LinkEntity joinContacto = JoinContactos(contactId);
            joinCliente.LinkEntities.Add(joinContacto);

            return joinCliente;
        }

        private LinkEntity JoinContactos(string contactId)
        {
            LinkEntity joinContact = new LinkEntity("crcd6_cliente", "contact", "crcd6_contactoid", "contactid", JoinOperator.Inner)
            {
                EntityAlias = "Contacto"
            };

            joinContact.Columns = new ColumnSet("emailaddress1", "fullname", "adx_identity_username", "crcd6_contrasenafandit", "contactid");
            joinContact.LinkCriteria = new FilterExpression();
            joinContact.LinkCriteria.FilterOperator = LogicalOperator.And;
            joinContact.LinkCriteria.AddCondition("contactid", ConditionOperator.Equal, contactId);

            return joinContact;
        }

        public EntityCollection ObtenerParametrosFandit()
        {
            QueryExpression parametrosPaycomet = new QueryExpression("adx_sitesetting");
            parametrosPaycomet.NoLock = true;
            parametrosPaycomet.ColumnSet = new ColumnSet("adx_name", "adx_value");
            parametrosPaycomet.Criteria.AddCondition("adx_name", ConditionOperator.Like, "%FANDIT%");
            return ServicioCRM.RetrieveMultiple(parametrosPaycomet);
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
