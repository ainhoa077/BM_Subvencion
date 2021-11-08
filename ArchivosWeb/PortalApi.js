var PortalApi = {
    Api: function (api, peticion, atributos, cacheString) {
        let attributesModificado = atributos.join(",");
        let timeStampInMs = window.performance && window.performance.now && window.performance.timing && window.performance.timing.navigationStart ? window.performance.now() + window.performance.timing.navigationStart : Date.now();

        let request;
        let url = "/portalapi?api=" + api + "&peticion=" + this.prepareParameters(peticion) + "&atributos=" + attributesModificado + "&time=" + timeStampInMs;
        if (cacheString) {
            url += "&cacheString=" + cacheString;
        }

        let result = null;
        $.ajax({
            async: false,
            type: "GET",
            dataType: "json",
            url: url,
            beforeSend: function (xhr) {
                request = xhr;
            }
        }).done(function (data) {
            result = data;
        })
            .fail(function (ex) {
                result = ex.responseText;
            });

        return result;
    },

    prepareParameters: function (parameters) {
        return btoa(unescape(encodeURIComponent(JSON.stringify(parameters))));
    }
};