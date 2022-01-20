if (window.jQuery) {
  (function ($) {
     if (typeof (entityFormClientValidate) != 'undefined') {
        var originalValidationFunction = entityFormClientValidate;
        if (originalValidationFunction && typeof (originalValidationFunction) == "function") {
          entityFormClientValidate = function() {
              originalValidationFunction.apply(this, arguments);
              PagoTransferencia.PagoRevisadoTranferencia();
              return true;
           };
        }
     }
  }(window.jQuery));
}

const ServiciosOfertados = {
  SeguimientoJustificacion: "522960002",
  TramitacionPremium: "522960003",
  Tramitacion: "522960000",
  ServicioAsesoramiento: "522960005",
  NecesitoElegibilidad: "522960006"
  }
const PagoTransferencia = {
  EventosOnload: function () {
      $('#crcd6_pagorecibido').hide();
      $('#crcd6_pagorecibido_label').hide();
      $('#crcd6_pagorecibidofraccionado').hide();
      $('#crcd6_pagorecibidofraccionado_label').hide();
      $('#crcd6_justificanteadjuntado').hide();
      $('#crcd6_justificanteadjuntado_label').hide();
      $('#notescontrol').hide();
  },
  ConsultarPagosTransferencia: function() {
      let resultado = [];
      $.ajax({
          method: 'GET',
          dataType: 'json',
          async: false,
          url: "/consulta-pagos-transferencia-bancaria?idsolicitud=" + PagoTransferencia.ObtenerParametroUrl('id')
      }).done(function(data) {
          resultado = data.results;
      });
      return resultado;
  },
  PagoRevisadoTranferencia: function(){
      const pagosTransferencia = PagoTransferencia.ConsultarPagosTransferencia(); 

      if(pagosTransferencia !== null && pagosTransferencia.length > 0){
        if(pagosTransferencia[0].codigoServicio !== ServiciosOfertados.TramitacionPremium && 
          pagosTransferencia[0].pagoRecibido == "false" && pagosTransferencia[0].justificanteAdjuntado == "false"){
            $("#crcd6_justificanteadjuntado_0").prop('checked', false);
            $("#crcd6_justificanteadjuntado_1").prop('checked', true);
            $('#crcd6_justificanteadjuntado_label').parent().parent().show();
            $('#crcd6_justificanteadjuntado').parent().parent().show();
        }
        else if(pagosTransferencia[0].codigoServicio === ServiciosOfertados.TramitacionPremium && 
          pagosTransferencia[0].pagoRecibido == "false"  && pagosTransferencia[0].justificanteAdjuntado == "false"){
            $("#crcd6_justificanteadjuntado_0").prop('checked', false);
            $("#crcd6_justificanteadjuntado_1").prop('checked', true);
            $('#crcd6_justificanteadjuntado_label').parent().parent().show();
            $('#crcd6_justificanteadjuntado').parent().parent().show();
        }
        else if(pagosTransferencia[0].codigoServicio === ServiciosOfertados.TramitacionPremium && pagosTransferencia[0].justificanteAdjuntado == "false" &&
          (pagosTransferencia[0].pagoRecibido == "true" && pagosTransferencia[0].pagoRecibidoFraccionado == "false")){
            $("#crcd6_justificanteadjuntado_0").prop('checked', false);
            $("#crcd6_justificanteadjuntado_1").prop('checked', true);
            $('#crcd6_justificanteadjuntado_label').parent().parent().show();
            $('#crcd6_justificanteadjuntado').parent().parent().show();
        }
      }
  },
  ObtenerParametroUrl: function(sParam){
      var sPageURL = window.location.search.substring(1);
      var sURLVariables = sPageURL.split('&');
      for (var i = 0; i < sURLVariables.length; i++) 
      {
          var sParameterName = sURLVariables[i].split('=');
          if (sParameterName[0] == sParam) 
          {
              return sParameterName[1];
          }
      }
  },
};