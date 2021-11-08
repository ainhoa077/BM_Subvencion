const ValidarApoderadoEmpresa = {
    ConsultarRegistroCompleto: function (contactoId) {
        const idContacto = contactoId;
        let resultado = [];
        $.ajax({
                method: 'GET',
                dataType: 'json',
                async: false,
                url: "/registro-completo?idContacto=" + idContacto
            }).done(function(data) {
                resultado = data.results;
            });
        
        return resultado;
    },
	ConsultarApoderadoCompleto: function (contactoId) {
        const idContacto = contactoId;
        let resultado = [];
        $.ajax({
                method: 'GET',
                dataType: 'json',
                async: false,
                url: "/info-apoderado-completo?idContacto=" + idContacto
            }).done(function(data) {
                resultado = data.results;
            });
        
        return resultado;
    },
    EventosOnload: function(contactoId){
		const apoderadoCompleto = ValidarApoderadoEmpresa.ApoderadoEsCompleto(contactoId);

        if(!apoderadoCompleto){
            window.location.href = "/profile";
        }
		
        const registroCompleto = ValidarApoderadoEmpresa.RegistroEsCompleto(contactoId);

        if(!registroCompleto && !window.location.href.includes("Formulario-registro")){
            window.location.href = "/Formulario-registro";
        }
    },
    RegistroEsCompleto(contactoId){
        let respuesta = ValidarApoderadoEmpresa.ConsultarRegistroCompleto(contactoId);

        if(respuesta.length > 0){
            return false;
        }

        return true;
    },
    ApoderadoEsCompleto(contactoId){
        let respuesta = ValidarApoderadoEmpresa.ConsultarApoderadoCompleto(contactoId);

        if(respuesta.length > 0){
            return true;
        }

        return false;
    }
};
