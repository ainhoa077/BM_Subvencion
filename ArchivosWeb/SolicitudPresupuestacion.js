const PreguntaInicial = {
	IdeaProyecto: "522960000",
	ProyectoPropio: "522960001",
	NecesitoElaboracion: "522960002",
	NecesitoTramite: "522960003",
	Otros: "522960004",
	NecesitoSeguimiento: "522960005",
	NecesitoElegibilidad: "522960006",
	SuscripcionBuscadorAnual: "522960007",
	NecesitoKitDigital: "522960008"
}

const ServiciosOfertados = {
	SeguimientoJustificacion: "522960002",
	TramitacionPremium: "522960003",
	Tramitacion: "522960000",
	ServicioAsesoramiento: "522960005",
	ServicioElegibilidad: "522960006",
	ServicioSuscripcionBuscadorAnual: "522960007",
	ServicioKitDigital: "522960008"
}

const RespuestaPregunta = {
	Si: "1",
	No: "0"
}

let ServicioOfertadosConfigurados = [];

const SolicitudPresupuestacion = {
	EventosOnload: function(idContacto){
		ServicioOfertadosConfigurados = SolicitudPresupuestacion.ConsultarServiciosOfertados();
		SolicitudPresupuestacion.OcultarCampos();
		SolicitudPresupuestacion.DeseleccionarCampos();
		$("#cr908_preguntaprincipal").find('option*').removeAttr("selected");
		SolicitudPresupuestacion.EventosOnChange();
		SolicitudPresupuestacion.MapearClienteSolicitud(idContacto);
	},
	OcultarCampos: function(){
		$('#crcd6_descripcionproyecto').val('');
		$('#crcd6_descripcionproyecto_label').hide();
		$('#crcd6_descripcionproyecto').hide();
		$('#cr908_previoquintapregunta_label').text("¿Necesitas ayuda para conocer la elegibilidad de tu empresa y proyecto para una ayuda identificada?");
		$('#cr908_previoquintapregunta_label').parent().parent().hide();
		$('#cr908_previoquintapregunta').parent().parent().hide();
		$('#cr908_previoquintapregunta_0').parent().parent().hide();
		$('#cr908_previoquintapregunta_1').parent().parent().hide();
		$('#crcd6_previotercerapregunta_label').parent().parent().hide();
		$('#crcd6_previotercerapregunta').parent().parent().hide();
		$('#crcd6_previotercerapregunta_0').parent().parent().hide();
		$('#crcd6_previotercerapregunta_1').parent().parent().hide();
		$('#crcd6_previocuartapregunta_label').parent().parent().hide();
		$('#crcd6_previocuartapregunta').parent().parent().hide();
		$('#crcd6_previocuartapregunta_0').parent().parent().hide();
		$('#crcd6_previocuartapregunta_1').parent().parent().hide();
		$('#crcd6_previoprimerapregunta_label').parent().parent().hide();
		$('#crcd6_previoprimerapregunta').parent().parent().hide();
		$('#crcd6_previoprimerapregunta_0').parent().parent().hide();
		$('#crcd6_previoprimerapregunta_1').parent().parent().hide();
		$('#crcd6_previosegundapregunta_label').parent().parent().hide();
		$('#crcd6_previosegundapregunta').parent().parent().hide();
		$('#crcd6_previosegundapregunta_0').parent().parent().hide();
		$('#crcd6_previosegundapregunta_1').parent().parent().hide();
		$('#crcd6_crcd6_servicio_label').parent().parent().hide();
		$('#crcd6_servicio').parent().parent().hide();
		$(':input[id="NextButton"]').prop('disabled', true);
		$('#crcd6_cliente_label').parent().parent().hide();
		$('#crcd6_cliente_name').parent().parent().hide();
		$('#crcd6_cliente').parent().parent().hide();
		$('#crcd6_cliente_entityname').parent().parent().hide();
		$('#crcd6_serviciosofertados_label').parent().parent().hide();
		$('#crcd6_serviciosofertados_name').parent().parent().hide();
		$('#crcd6_serviciosofertados').parent().parent().hide();
		$('#crcd6_serviciosofertados_entityname').parent().parent().hide();
		$('#crcd6_solicitudcompleta_label').parent().parent().hide();
		$('#crcd6_solicitudcompleta').parent().parent().hide();
		$('#crcd6_solicitudcompleta_0').parent().parent().hide();
		$('#crcd6_solicitudcompleta_1').parent().parent().hide();
	},
	DeseleccionarCampos: function(){
		$("#crcd6_previotercerapregunta_0").prop('checked', false);
		$("#crcd6_previocuartapregunta_0").prop('checked', false);
		$("#crcd6_previoprimerapregunta_0").prop('checked', false);
		$("#crcd6_previosegundapregunta_0").prop('checked', false);
		$("#cr908_previoquintapregunta_0").prop('checked', false);
		$("#crcd6_previotercerapregunta_1").prop('checked', false);
		$("#crcd6_previocuartapregunta_1").prop('checked', false);
		$("#crcd6_previoprimerapregunta_1").prop('checked', false);
		$("#crcd6_previosegundapregunta_1").prop('checked', false);
		$("#cr908_previoquintapregunta_1").prop('checked', false);
	},
	EventosOnChange: function(){
		$("#cr908_preguntaprincipal").change(function () {
			$(':input[id="NextButton"]').prop('disabled', true);
			SolicitudPresupuestacion.ResetearServicioOfertado();
			SolicitudPresupuestacion.OcultarCampos();
			SolicitudPresupuestacion.DeseleccionarCampos();
			SolicitudPresupuestacion.ValidacionIdeaProyecto();
			SolicitudPresupuestacion.ValidacionProyectoPropio();
			SolicitudPresupuestacion.ValidacionNecesitoElaboracionOTramite();
			SolicitudPresupuestacion.ValidacionOtros();
			SolicitudPresupuestacion.ValidarSeguimiento();
			SolicitudPresupuestacion.ValidacionElegibilidad();
			SolicitudPresupuestacion.ValidarSuscripcionAnual();
			SolicitudPresupuestacion.ValidarKitDigital();
		});
		
		$("#crcd6_previoprimerapregunta").change(function () {
			$(':input[id="NextButton"]').prop('disabled', true);
			SolicitudPresupuestacion.ResetearServicioOfertado();
			$("#crcd6_previosegundapregunta_1").prop('checked', false);
			$("#crcd6_previosegundapregunta_0").prop('checked', false);
			SolicitudPresupuestacion.RedactadaMemoria();
		});
		
		$("#crcd6_previosegundapregunta").change(function () {
			$(':input[id="NextButton"]').prop('disabled', true);
			SolicitudPresupuestacion.ResetearServicioOfertado();
			SolicitudPresupuestacion.AyudaRedaccion();
		});
		
		$("#crcd6_previotercerapregunta").change(function () {
			$(':input[id="NextButton"]').prop('disabled', true);
			SolicitudPresupuestacion.ResetearServicioOfertado();
			$("#crcd6_previosegundapregunta_1").prop('checked', false);
			$("#crcd6_previosegundapregunta_0").prop('checked', false);
			$("#crcd6_previoprimerapregunta_1").prop('checked', false);
			$("#crcd6_previoprimerapregunta_0").prop('checked', false);
			$("#crcd6_previocuartapregunta_0").prop('checked', false);
			$("#crcd6_previocuartapregunta_1").prop('checked', false);
			$("#cr908_previoquintapregunta_0").prop('checked', false);
			$("#cr908_previoquintapregunta_1").prop('checked', false);
			$('#crcd6_previosegundapregunta_label').parent().parent().hide();
			$('#crcd6_previosegundapregunta').parent().parent().hide();
			$('#crcd6_previosegundapregunta_0').parent().parent().hide();
			$('#crcd6_previosegundapregunta_1').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_label').parent().parent().hide();
			$('#crcd6_previoprimerapregunta').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_0').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_1').parent().parent().hide();
			$('#crcd6_previocuartapregunta_label').parent().parent().hide();
			$('#crcd6_previocuartapregunta').parent().parent().hide();
			$('#crcd6_previocuartapregunta_0').parent().parent().hide();
			$('#crcd6_previocuartapregunta_1').parent().parent().hide();
			$('#cr908_previoquintapregunta_label').parent().parent().hide();
			$('#cr908_previoquintapregunta').parent().parent().hide();
			$('#cr908_previoquintapregunta_0').parent().parent().hide();
			$('#cr908_previoquintapregunta_1').parent().parent().hide();
			SolicitudPresupuestacion.IdentificadaAyuda();
		});
		
		$("#crcd6_previocuartapregunta").change(function () {
			$(':input[id="NextButton"]').prop('disabled', true);
			SolicitudPresupuestacion.ResetearServicioOfertado();
			$("#crcd6_previosegundapregunta_1").prop('checked', false);
			$("#crcd6_previosegundapregunta_0").prop('checked', false);
			$("#crcd6_previoprimerapregunta_1").prop('checked', false);
			$("#crcd6_previoprimerapregunta_0").prop('checked', false);
			$('#crcd6_previosegundapregunta_label').parent().parent().hide();
			$('#crcd6_previosegundapregunta').parent().parent().hide();
			$('#crcd6_previosegundapregunta_0').parent().parent().hide();
			$('#crcd6_previosegundapregunta_1').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_label').parent().parent().hide();
			$('#crcd6_previoprimerapregunta').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_0').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_1').parent().parent().hide();
			SolicitudPresupuestacion.ConocesProyecto();
		});
		//REVISAR POR NO ENTENDER
		$("#cr908_previoquintapregunta").change(function () {
			$(':input[id="NextButton"]').prop('disabled', true);
			SolicitudPresupuestacion.ResetearServicioOfertado();
			$("#crcd6_previosegundapregunta_1").prop('checked', false);
			$("#crcd6_previosegundapregunta_0").prop('checked', false);
			$("#crcd6_previoprimerapregunta_1").prop('checked', false);
			$("#crcd6_previoprimerapregunta_0").prop('checked', false);
			$("#crcd6_previocuartapregunta_1").prop('checked', false);
			$("#crcd6_previocuartapregunta_0").prop('checked', false);
			$('#crcd6_previosegundapregunta_label').parent().parent().hide();
			$('#crcd6_previosegundapregunta').parent().parent().hide();
			$('#crcd6_previosegundapregunta_0').parent().parent().hide();
			$('#crcd6_previosegundapregunta_1').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_label').parent().parent().hide();
			$('#crcd6_previoprimerapregunta').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_0').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_1').parent().parent().hide();
			$('#crcd6_previocuartapregunta_label').parent().parent().hide();
			$('#crcd6_previocuartapregunta').parent().parent().hide();
			$('#crcd6_previocuartapregunta_0').parent().parent().hide();
			$('#crcd6_previocuartapregunta_1').parent().parent().hide();
			SolicitudPresupuestacion.QuieroElegibilidad();
		});
	},
	ValidarSuscripcionAnual: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		if(preguntaInicial === PreguntaInicial.SuscripcionBuscadorAnual){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioSuscripcionBuscadorAnual);
			SolicitudPresupuestacion.SetearSolucionCompleta();
			$(':input[id="NextButton"]').prop('disabled', false);
		}
	},
	ValidarSeguimiento: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		if(preguntaInicial === PreguntaInicial.NecesitoSeguimiento){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.SeguimientoJustificacion);
			$(':input[id="NextButton"]').prop('disabled', false);
		}
	},
	ValidacionIdeaProyecto: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		if(preguntaInicial === PreguntaInicial.IdeaProyecto){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioAsesoramiento);
			$(':input[id="NextButton"]').prop('disabled', false);
		}
	},
	ValidacionProyectoPropio: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		
		if(preguntaInicial === PreguntaInicial.ProyectoPropio){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioAsesoramiento);
			$(':input[id="NextButton"]').prop('disabled', false);
		}
	},
	ValidacionElegibilidad: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		if(preguntaInicial === PreguntaInicial.NecesitoElegibilidad){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioElegibilidad);
			$('#crcd6_previocuartapregunta').parent().parent().show();
			$('#crcd6_previocuartapregunta_0').parent().parent().show();
			$('#crcd6_previocuartapregunta_1').parent().parent().show();
			$('#crcd6_previocuartapregunta_label').parent().parent().show();
		}
	},
	ValidacionNecesitoElaboracionOTramite: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		if((preguntaInicial === PreguntaInicial.NecesitoElaboracion)
			|| (preguntaInicial === PreguntaInicial.NecesitoTramite)){
			$('#crcd6_previoprimerapregunta').parent().parent().show();
			$('#crcd6_previoprimerapregunta_0').parent().parent().show();
			$('#crcd6_previoprimerapregunta_1').parent().parent().show();
			$('#crcd6_previoprimerapregunta_label').parent().parent().show();
		}
	},
	ValidarKitDigital: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		if(preguntaInicial === PreguntaInicial.NecesitoKitDigital){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioKitDigital);
			/*
			$('#crcd6_primerapregkitdigital_label').parent().parent().show();
			$('#crcd6_primerapregkitdigital').parent().parent().show();
			$('#crcd6_primerapregkitdigital_0').parent().parent().show();
			$('#crcd6_primerapregkitdigital_1').parent().parent().show();
			$('#crcd6_primerapregkitdigital_label').parent().parent().show();
			$('#crcd6_segundapregkitdigital').parent().parent().show();
			$('#crcd6_segundapregkitdigital_0').parent().parent().show();
			$('#crcd6_segundapregkitdigital_1').parent().parent().show();
			$('#crcd6_segundapregkitdigital_label').parent().parent().show();
			$('#crcd6_tercerapregkitdigital').parent().parent().show();
			$('#crcd6_tercerapregkitdigital_0').parent().parent().show();
			$('#crcd6_tercerapregkitdigital_1').parent().parent().show();
			$('#crcd6_descripcionprimerapregkitdigital').text(DescripcionPreguntas.DescPrimeraPreg);
			$('#crcd6_descripcionsegundapregkitdigital').text(DescripcionPreguntas.DescSengundaPreg);
			$('#crcd6_descripciontercerapregkitdigital').text(DescripcionPreguntas.DescTerceraPreg);
			$('#crcd6_descripcionprimerapregkitdigital_label').parent().parent().show();
			$('#crcd6_descripcionprimerapregkitdigital').parent().parent().show();
			$('#crcd6_descripcionsegundapregkitdigital_label').parent().parent().show();
			$('#crcd6_descripcionsegundapregkitdigital').parent().parent().show();
			$('#crcd6_descripciontercerapregkitdigital_label').parent().parent().show();
			$('#crcd6_descripciontercerapregkitdigital').parent().parent().show();
			*/
			$(':input[id="NextButton"]').prop('disabled', false);	
		}
	},
	ValidacionOtros: function(){
		var preguntaInicial = $("#cr908_preguntaprincipal").val();
		
		if(preguntaInicial === PreguntaInicial.Otros){
			$('#crcd6_previotercerapregunta').parent().parent().show();
			$('#crcd6_previotercerapregunta_0').parent().parent().show();
			$('#crcd6_previotercerapregunta_1').parent().parent().show();
			$('#crcd6_previotercerapregunta_label').parent().parent().show();
		}
	},
	RedactadaMemoria: function(){
		valorRedactarMemoria = $("#crcd6_previoprimerapregunta").find(":radio:checked").first().attr('value');
		
		if(valorRedactarMemoria === RespuestaPregunta.Si){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.Tramitacion);
			$(':input[id="NextButton"]').prop('disabled', false);
			$('#crcd6_previosegundapregunta').parent().parent().hide();
			$('#crcd6_previosegundapregunta_0').parent().parent().hide();
			$('#crcd6_previosegundapregunta_1').parent().parent().hide();
			$('#crcd6_crcd6_servicio_label').parent().parent().hide();
		}
		else{
			$(':input[id="NextButton"]').prop('disabled', true);
			$('#crcd6_previosegundapregunta').parent().parent().show();
			$('#crcd6_previosegundapregunta_0').parent().parent().show();
			$('#crcd6_previosegundapregunta_1').parent().parent().show();
			$('#crcd6_previosegundapregunta_label').parent().parent().show();
		}
	},
	AyudaRedaccion: function(){
		valorAyudaRedaccion = $("#crcd6_previosegundapregunta").find(":radio:checked").first().attr('value');
		
		if(valorAyudaRedaccion === RespuestaPregunta.Si){
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.TramitacionPremium);

		}
		else{
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.Tramitacion);
		}
		
		$(':input[id="NextButton"]').prop('disabled', false);
	},
	IdentificadaAyuda: function(){
		valorIdentificadaAyuda = $("#crcd6_previotercerapregunta").find(":radio:checked").first().attr('value');
		
		if(valorIdentificadaAyuda === RespuestaPregunta.Si){
			$(':input[id="NextButton"]').prop('disabled', true);
			$('#cr908_previoquintapregunta').parent().parent().show();
			$('#cr908_previoquintapregunta_0').parent().parent().show();
			$('#cr908_previoquintapregunta_1').parent().parent().show();
			$('#cr908_previoquintapregunta_label').parent().parent().show();
		}
		else{
			$(':input[id="NextButton"]').prop('disabled', false);
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioAsesoramiento);
			$('#cr908_previoquintapregunta').parent().parent().hide();
			$('#cr908_previoquintapregunta_0').parent().parent().hide();
			$('#cr908_previoquintapregunta_1').parent().parent().hide();
			$('#cr908_previoquintapregunta_label').parent().parent().hide();
			$('#crcd6_descripcionproyecto').val('');
			$('#crcd6_descripcionproyecto_label').hide();
			$('#crcd6_descripcionproyecto').hide();
		}
	},
	ConocesProyecto: function(){
		valorConocesProyecto = $("#crcd6_previocuartapregunta").find(":radio:checked").first().attr('value');
		var preguntaInicial = $("#cr908_preguntaprincipal").val();

		if(preguntaInicial == PreguntaInicial.NecesitoElegibilidad){
			$(':input[id="NextButton"]').prop('disabled', false);
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioElegibilidad);
		}
		else if(valorConocesProyecto === RespuestaPregunta.Si){
			$(':input[id="NextButton"]').prop('disabled', true);
			$('#crcd6_previoprimerapregunta').parent().parent().show();
			$('#crcd6_previoprimerapregunta_0').parent().parent().show();
			$('#crcd6_previoprimerapregunta_1').parent().parent().show();
			$('#crcd6_previoprimerapregunta_label').parent().parent().show();
		}
		else{
			$(':input[id="NextButton"]').prop('disabled', false);
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.TramitacionPremium);
			$('#crcd6_previoprimerapregunta').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_0').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_1').parent().parent().hide();
			$('#crcd6_previoprimerapregunta_label').parent().parent().hide();
		}
	},
	//Nuevo
	QuieroElegibilidad: function(){
		valorQuieroElegibilidad = $("#cr908_previoquintapregunta").find(":radio:checked").first().attr('value');
		
		if(valorQuieroElegibilidad === RespuestaPregunta.Si){
			$(':input[id="NextButton"]').prop('disabled', false);
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioElegibilidad);
			$('#crcd6_descripcionproyecto').val('');
			$('#crcd6_descripcionproyecto_label').hide();
			$('#crcd6_descripcionproyecto').hide();
			$('#crcd6_previocuartapregunta').parent().parent().hide();
			$('#crcd6_previocuartapregunta_0').parent().parent().hide();
			$('#crcd6_previocuartapregunta_1').parent().parent().hide();
			$('#crcd6_previocuartapregunta_label').parent().parent().hide();
		}
		else{
			$(':input[id="NextButton"]').prop('disabled', true);
			SolicitudPresupuestacion.SetearServicioOfertado(ServiciosOfertados.ServicioAsesoramiento);
			$('#crcd6_descripcionproyecto_label').text("Indicanos por que motivo quieres solicitar la ayuda").val();
			$('#crcd6_descripcionproyecto_label').show();
			$('#crcd6_descripcionproyecto').show();
			$('#crcd6_previocuartapregunta').parent().parent().show();
			$('#crcd6_previocuartapregunta_0').parent().parent().show();
			$('#crcd6_previocuartapregunta_1').parent().parent().show();
			$('#crcd6_previocuartapregunta_label').parent().parent().show();
		}
	},
	//Fin nuevo
	MapearClienteSolicitud(idContacto){
        let respuesta = SolicitudPresupuestacion.ConsultarClientePorContacto(idContacto);

        if(respuesta.length > 0){
            $("#crcd6_cliente").val(respuesta[0].id);
			$("#crcd6_cliente_name").val(respuesta[0].nombre);
			$("#crcd6_cliente_entityname").val("crcd6_cliente");
        }
    },
	SetearServicioOfertado: function(servicioOfertado){
		const servicioDeducido = ServicioOfertadosConfigurados.find(x => x.codigo === servicioOfertado);
		$("#crcd6_serviciosofertados").val(servicioDeducido.id);
		$("#crcd6_serviciosofertados_name").val(servicioDeducido.nombre);
		$("#crcd6_serviciosofertados_entityname").val("crcd6_servicioofertado");
	},
	ResetearServicioOfertado: function(){
		$("#crcd6_serviciosofertados").val('');
		$("#crcd6_serviciosofertados_name").val('');
		$("#crcd6_serviciosofertados_entityname").val("crcd6_servicioofertado");
	},
	SetearSolucionCompleta: function(){
		$("#crcd6_solicitudcompleta_0").prop('checked', false);
		$("#crcd6_solicitudcompleta_1").prop('checked', true);
	},
	ConsultarClientePorContacto: function (idContacto) {
        let resultado = [];
        $.ajax({
                method: 'GET',
                dataType: 'json',
                async: false,
                url: "/consulta-cliente-usuario?idContacto=" + idContacto
            }).done(function(data) {
                resultado = data.results;
            });
        
        return resultado;
    },
	ConsultarServiciosOfertados: function() {
        let resultado = [];
        $.ajax({
                method: 'GET',
                dataType: 'json',
                async: false,
                url: "/consulta-servicios-ofertados/"
            }).done(function(data) {
                resultado = data.results;
            });
        
        return resultado;
    },
};