using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using PWC.Subvenciones.MensajeriaSolicitud.Modelos;
using PWC.Subvenciones.MensajeriaSolicitud.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PWC.Subvenciones.MensajeriaSolicitud.Controlador
{
    public class EnvioCorreoController: EnviarCorreoMensajeriaBaseController
    {
        public EnvioCorreoController(IOrganizationService service, IPluginExecutionContext contextoEjecucion): base(service, contextoEjecucion)
        {
            Service = service;
            ContextoEjecucion = contextoEjecucion;
        }

        public void EjecutarControlador()
        {
            string usuarioSystem = ObtenerUsuarioSystem();
            Entity notas = (Entity)ContextoEjecucion.InputParameters["Target"];

            if (notas.Attributes.Contains("notetext"))
            {
                if (notas.Attributes["notetext"].ToString().Contains("*WEB*") && ((EntityReference)notas.Attributes["createdby"]).Id.ToString() != usuarioSystem)
                {
                    EnvioCorreoMensajeriaSolicitud(notas);
                }
            }
        }

        private string ObtenerUsuarioSystem()
        {
            string usuarioSystemId = string.Empty;
            QueryExpression solicitud = new QueryExpression("systemuser");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("systemuserid");
            solicitud.Criteria.AddCondition("fullname", ConditionOperator.Equal, "SYSTEM");

            EntityCollection respuesta = Service.RetrieveMultiple(solicitud);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                usuarioSystemId = respuesta.Entities[0].Id.ToString();
            }

            return usuarioSystemId;
        }

        private void EnvioCorreoMensajeriaSolicitud(Entity notas)
        {
            EntityReference objectId = (EntityReference)notas.Attributes["objectid"];

            if (objectId.LogicalName == "crcd6_solicitudsub")
            {
                SolicitudModelo solicitud = ObtenerInformacionSolicitud(objectId.Id);
                EnviarCorreoElectronico(solicitud, notas);
            }
        }

        private void EnviarCorreoElectronico(SolicitudModelo solicitud, Entity notas)
        {
            string mensaje = notas.Attributes["notetext"].ToString().Replace("*WEB*", "");
            mensaje = ConvertirHtmlAPlano.Convertir(mensaje);
            EntityCollection parametrosTwilio = ObtenerParametrosGmailApi();
            string automateUrl = BuscarParametro(parametrosTwilio, "GMAIL-AUTOMATEURL");
            string asunto = BuscarParametro(parametrosTwilio, "GMAIL-ASUNTOMENSAJERIAGESTOR");
            string cuerpo = BuscarParametro(parametrosTwilio, "GMAIL-CUERPOMENSAJERIAGESTOR");
            string urlDetalle = BuscarParametro(parametrosTwilio, "GMAIL-URLDETALLESOLICITUD");
            asunto = asunto.Replace("{numeroSolicitud}", solicitud.NumeroSolicitud);
            urlDetalle = urlDetalle.Replace("{idSolicitud}", solicitud.Id);
            cuerpo = cuerpo.Replace("{mensaje}", mensaje);
            cuerpo = cuerpo.Replace("{urlDetalle}", urlDetalle);
            cuerpo = cuerpo.Replace("\"", "'");
            RequestCorreo peticion = new RequestCorreo();
            peticion.emailaddress = solicitud.CorreoElectronico;
            peticion.emailSubject = asunto;
            peticion.emailBody = cuerpo;
            string parametroEntrada = JsonConvert.SerializeObject(peticion);
            string respuesta = ConsumoAPI.EjecutarAPI(automateUrl, parametroEntrada);
        }

        private SolicitudModelo ObtenerInformacionSolicitud(Guid id)
        {
            SolicitudModelo solicitudModelo = new SolicitudModelo();
            QueryExpression solicitud = new QueryExpression("crcd6_solicitudsub");
            solicitud.NoLock = true;
            solicitud.ColumnSet = new ColumnSet("crcd6_solicitudsubid", "crcd6_solicitud");
            solicitud.Criteria.AddCondition("crcd6_solicitudsubid", ConditionOperator.Equal, id.ToString());
            LinkEntity joinCliente = JoinClientes();
            solicitud.LinkEntities.Add(joinCliente);

            EntityCollection respuesta = Service.RetrieveMultiple(solicitud);

            if (respuesta != null && respuesta.Entities.Count > 0)
            {
                Entity entidadSolcitud = respuesta.Entities[0];
                solicitudModelo.Id = entidadSolcitud.Id.ToString();
                solicitudModelo.NumeroSolicitud = entidadSolcitud.GetAttributeValue<string>("crcd6_solicitud");
                solicitudModelo.CorreoElectronico = entidadSolcitud.GetAttributeValue<AliasedValue>("Contacto.emailaddress1").Value.ToString();
            }

            return solicitudModelo;
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

            joinContact.Columns = new ColumnSet("emailaddress1");

            return joinContact;
        }
    }
}
