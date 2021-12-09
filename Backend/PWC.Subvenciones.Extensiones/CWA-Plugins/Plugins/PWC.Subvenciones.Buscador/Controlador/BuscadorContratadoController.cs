using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PWC.Subvenciones.Buscador.Interfaces;
using PWC.Subvenciones.Buscador.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.Buscador.Controlador
{
    public class BuscadorContratadoController : IBuscadorContratado
    {
        private const string PASSWORD_SEED = "PWC-2021";
        private const string SERVICIOANUALBUSCADOR = "522960007";
        public IOrganizationService Service { get; set; }
        public IPluginExecutionContext ContextoEjecucion { get; set; }

        public BuscadorContratadoController(IOrganizationService service, IPluginExecutionContext contextoEjecucion)
        {
            Service = service;
            ContextoEjecucion = contextoEjecucion;
        }

        public void Ejecutar(Entity solicitud)
        {
            ContactoModelo contacto = ObtenerContactoSolicitud(solicitud.Id);

            if (contacto != null && solicitud.Attributes.Contains("crcd6_pagorecibido") && solicitud.GetAttributeValue<bool>("crcd6_pagorecibido"))
            {
                ActualizarFechasContratacionBuscador(solicitud);
                EntityCollection parametrosFandit = ObtenerParametrosFandit();
                CrearUsuarioFandit(contacto, parametrosFandit);
            }
        }

        private void CrearUsuarioFandit(ContactoModelo contacto, EntityCollection parametrosFandit)
        {
            string urlLogin = BuscarParametro(parametrosFandit, "FANDIT-URL-LOGIN");
            string urlRegistration = BuscarParametro(parametrosFandit, "FANDIT-URL-REGISTRATION");
            string cookie = BuscarParametro(parametrosFandit, "FANDIT-COOKIE");
            string session = BuscarParametro(parametrosFandit, "FANDIT-F1MN");
            string platForm = BuscarParametro(parametrosFandit, "FANDIT-PLATFORM");

            var client = new HttpClient();
            var requestContent = new MultipartFormDataContent();
            client.DefaultRequestHeaders.Add("Cookie", cookie);
            client.DefaultRequestHeaders.Add("F1MN", session);
            var values = new Dictionary<string, string>
            {
                { "email", contacto.CorreoElectronico },
                { "first_name", contacto.NombresApellidos },
                { "password1", $"{contacto.Username}-{PASSWORD_SEED}" },
                { "password2", $"{contacto.Username}-{PASSWORD_SEED}" },
                { "platform", platForm },
            };

            foreach (var keyValuePair in values)
            {
                requestContent.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            }
            var response = client.PostAsync(urlRegistration, requestContent).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private ContactoModelo ObtenerContactoSolicitud(object id)
        {
            ContactoModelo contactoModelo = new ContactoModelo();
            QueryExpression solicitud = new QueryExpression("crcd6_solicitudsub");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("crcd6_solicitudsubid", "crcd6_solicitud");
            solicitud.Criteria.AddCondition("crcd6_solicitudsubid", ConditionOperator.Equal, id.ToString());
            LinkEntity joinCliente = JoinClientes();
            solicitud.LinkEntities.Add(joinCliente);
            LinkEntity joinServicioOfertado = JoinServiciosOfertados();
            solicitud.LinkEntities.Add(joinServicioOfertado);

            EntityCollection respuesta = Service.RetrieveMultiple(solicitud);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                Entity entidadSolcitud = respuesta.Entities[0];
                contactoModelo.NombresApellidos = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.fullname").Value.ToString();
                contactoModelo.CorreoElectronico = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.emailaddress1").Value.ToString();
                contactoModelo.Username = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.adx_identity_username").Value.ToString();
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

        private LinkEntity JoinClientes()
        {
            LinkEntity joinCliente = new LinkEntity("crcd6_solicitudsub", "crcd6_cliente", "crcd6_cliente", "crcd6_clienteid", JoinOperator.Inner)
            {
                EntityAlias = "Clientes"
            };

            LinkEntity joinContacto = JoinContactos();
            joinCliente.LinkEntities.Add(joinContacto);

            return joinCliente;
        }

        private LinkEntity JoinContactos()
        {
            LinkEntity joinContact = new LinkEntity("crcd6_cliente", "contact", "crcd6_contactoid", "contactid", JoinOperator.Inner)
            {
                EntityAlias = "Contacto"
            };

            joinContact.Columns = new ColumnSet("emailaddress1", "fullname", "adx_identity_username");

            return joinContact;
        }

        private void ActualizarFechasContratacionBuscador(Entity solicitud)
        {
            solicitud.Attributes["crcd6_fechacontratacionsuscripcionbuscador"] = DateTime.Now.ToLocalTime();
            solicitud.Attributes["crcd6_fechaexpiracionsuscripcionbuscador"] = DateTime.Now.AddYears(1).ToLocalTime();
        }

        private EntityCollection ObtenerParametrosFandit()
        {
            QueryExpression parametrosPaycomet = new QueryExpression("adx_sitesetting");
            parametrosPaycomet.NoLock = true;
            parametrosPaycomet.ColumnSet = new ColumnSet("adx_name", "adx_value");
            parametrosPaycomet.Criteria.AddCondition("adx_name", ConditionOperator.Like, "%FANDIT%");
            return Service.RetrieveMultiple(parametrosPaycomet);
        }

        private string BuscarParametro(EntityCollection parametrosPaycomet, string keyParametro)
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
