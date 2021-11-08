if (window.jQuery) {
    (function ($) {
       if (typeof (entityFormClientValidate) != 'undefined') {
          var originalValidationFunction = entityFormClientValidate;
          if (originalValidationFunction && typeof (originalValidationFunction) == "function") {
            entityFormClientValidate = function() {
                originalValidationFunction.apply(this, arguments);
                debugger;
                const existeValidacionNIFCIF = FormularioRegistroCliente.ExisteNIFCIF();

                if(existeValidacionNIFCIF){
                   return false;
                }

                return true;
             };
          }
       }
    }(window.jQuery));
 }
 
 const FormularioRegistroCliente = {
   ExisteNIF() {
      let nif = $("#crcd6_nif").val();
      let respuesta = FormularioRegistroCliente.ConsultarNIF(nif);
      if (respuesta.length > 0) {
        return true;
      }
      return false;
    },
    ConsultarNIF: function (nif) {
      let resultado = [];
      $.ajax({
        method: "GET",
        dataType: "json",
        async: false,
        url: "/consultar-nif?nif=" + nif,
      }).done(function (data) {
        resultado = data.results;
      });
      return resultado;
    },
    ExisteCIF() {
      let cif = $("#crcd6_cif").val();
      let respuesta = FormularioRegistroCliente.ConsultarCIF(cif);
      if (respuesta.length > 0) {
        return true;
      }
      return false;
    },
    ConsultarCIF: function (cif) {
      let resultado = [];
      $.ajax({
        method: "GET",
        dataType: "json",
        async: false,
        url: "/consultar-cif?cif=" + cif,
      }).done(function (data) {
        resultado = data.results;
      });
      return resultado;
    },
    ExisteNIFCIF(){
      valorEsAutonomo = $("#crcd6_esautonomo")
                        .find(":radio:checked")
                        .first()
                        .attr("value");
    
      if (valorEsAutonomo === RespuestaEsAutonomo.No) {
         const existeCIF = FormularioRegistroCliente.ExisteCIF();
         FormularioRegistroCliente.ValidacionCIF(existeCIF);
         return existeCIF;
      }
      else{
         const existeNIF = FormularioRegistroCliente.ExisteNIF();
         FormularioRegistroCliente.ValidacionNIF(existeNIF);
         return existeNIF;
      }
    },
    ValidacionCIF: function(realizaValidacion){
         if(realizaValidacion){
            $("#idExisteCIF").show();
         }
         else{
            $("#idExisteCIF").hide();
         }
   },
   ValidacionNIF: function(realizaValidacion){
      if(realizaValidacion){
         $("#idExisteNIF").show();
      }else{
         $("#idExisteNIF").hide();
      }
   },
 }