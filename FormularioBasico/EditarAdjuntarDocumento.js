$(document).ready(function () {
    $('#crcd6_documentomodificado_label').parent().parent().hide();
	$('#crcd6_documentomodificado').parent().parent().hide();
    $("#AttachFile").after('<p id="formatoValido">Los formatos validos son: <b>.doc, .docx, .pdf, .png, .jpg</b></p>');
});


if (window.jQuery) {
    (function($) {
      if (typeof(entityFormClientValidate) != 'undefined') {
        var originalValidationFunction = entityFormClientValidate;
        if (originalValidationFunction && typeof(originalValidationFunction) == "function") {
            entityFormClientValidate = function() {
            originalValidationFunction.apply(this, arguments);
            $("#crcd6_documentomodificado_0").prop('checked', false);
            $("#crcd6_documentomodificado_1").prop('checked', true);
            $('#crcd6_documentomodificado_label').parent().parent().show();
	        $('#crcd6_documentomodificado').parent().parent().show();
            return true;
          };
        }
      }
    }(window.jQuery));
  }
