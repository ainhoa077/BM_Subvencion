"use strict";  
var Sdk = window.Sdk || {};  

Sdk.getClientUrl = function () {  
    var context;  
    if (typeof GetGlobalContext != "undefined")  
    { context = GetGlobalContext(); }  
    else  
    {  
        if (typeof Xrm != "undefined") {  
            context = Xrm.Page.context;  
        }  
        else { throw new Error("Context is not available."); }  
    }  
    return context.getClientUrl();  
}  


var clientUrl = Sdk.getClientUrl();
var webAPIPath = "/api/data/v8.1";     
var account1Uri;                        
var contact1Uri;  

Sdk.request = function (action, uri, data, formattedValue, maxPageSize) {  
    if (!RegExp(action, "g").test("POST PATCH PUT GET DELETE")) { // Expected action verbs.  
        throw new Error("Sdk.request: action parameter must be one of the following: " +  
            "POST, PATCH, PUT, GET, or DELETE.");  
    }  
    if (!typeof uri === "string") {  
        throw new Error("Sdk.request: uri parameter must be a string.");  
    }  
    if ((RegExp(action, "g").test("POST PATCH PUT")) && (data === null || data === undefined)) {  
        throw new Error("Sdk.request: data parameter must not be null for operations that create or modify data.");  
    }  
    if (maxPageSize === null || maxPageSize === undefined) {  
        maxPageSize = 10; // Default limit is 10 entities per page.  
    }  
  
    // Construct a fully qualified URI if a relative URI is passed in.  
    if (uri.charAt(0) === "/") {  
        uri = clientUrl + webAPIPath + uri;  
    }  
  
    return new Promise(function (resolve, reject) {  
        var request = new XMLHttpRequest();   
        request.open(action, encodeURI(uri), true);  
        request.setRequestHeader("OData-MaxVersion", "4.0");  
        request.setRequestHeader("OData-Version", "4.0");  
        request.setRequestHeader("Accept", "application/json");  
        request.setRequestHeader("Content-Type", "application/json; charset=utf-8");  
        request.setRequestHeader("Prefer", "odata.maxpagesize=" + maxPageSize);  
        if (formattedValue) {  
            request.setRequestHeader("Prefer",  
                "odata.include-annotations=OData.Community.Display.V1.FormattedValue");  
        }  
        request.onreadystatechange = function () {  
            if (this.readyState === 4) {  
                request.onreadystatechange = null;  
                switch (this.status) {  
                    case 200: // Success with content returned in response body.  
                    case 204: // Success with no content returned in response body.  
                        resolve(this);  
                        break;  
                    default: // All other statuses are unexpected so are treated like errors.  
                        var error;  
                        try {  
                            error = JSON.parse(request.response).error;  
                        } catch (e) {  
                            error = new Error("Unexpected Error");  
                        }  
                        reject(error);  
                        break;  
                }  
            }  
        };  
        request.send(JSON.stringify(data));  
    });  
}; 

Sdk.output = function (collection, properties) {  
    var prop = [];  
    collection.forEach(function (row, i) {  
        properties.forEach(function (p) {  
            var f = p + "@OData.Community.Display.V1.FormattedValue";  
            prop.push((row[f] ? row[f] : row[p]));
        })  
    });  
    return prop;  
} 

async function ObtenerCodigoEstado(formContext){
    var idEstado = formContext.getAttribute("crcd6_estado").getValue()[0].id;
    var fetchXML = "<fetch mapping=\"logical\" output-format=\"xml-platform\" version=\"1.0\" distinct=\"false\">" + 
    "<entity name=\"crcd6_estado\">" +  
    "    <attribute name=\"crcd6_codigo\" />" +
    "    <order descending=\"true\" attribute=\"crcd6_codigo\" />" + 
    "    <filter type=\"and\">" +
    "    <condition value=\"" + idEstado + "\" attribute=\"crcd6_estadoid\" operator=\"eq\" />" +
    "    </filter>" +
    "</entity>" +
    "</fetch> ";  

    var respuestaEstado = await Sdk.request("GET", "/crcd6_estados?fetchXml=" + encodeURIComponent(fetchXML), null, true);  
    var collection = JSON.parse(respuestaEstado.response).value;  
    var contactProperties = ["crcd6_codigo"];  
    return Sdk.output(collection, contactProperties)[0];
}

async function OnLoad(executionContext){
    var formContext = executionContext.getFormContext();
    formContext.data.process.addOnStageChange(onStageChange);
    debugger;
    await validateStageProcess(formContext);
    
}

async function validateStageProcess(formContext){
    debugger;
    var obtenerEstado = parseInt(await ObtenerCodigoEstado(formContext));
    var stage = formContext.data.process.getSelectedStage();
    var stageName = stage.getName();

    switch(stageName){
        /*case "Elaboración presupuesto": {
            if(obtenerEstado < 2){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 2){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }*/
        case "Pendiente aceptación de presupuesto": {
            if(obtenerEstado < 3){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 3){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }
        /*case "Pendiente de pago": {
            if(obtenerEstado < 4){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 4){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }
        case "Pendiente de documentación del cliente": {
            if(obtenerEstado < 5){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 5){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }
        case "Documentación en revisión por el asesor": {
            if(obtenerEstado < 6){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 6){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }
        case "Presentación de la ayuda ante la AAPP": {
            if(obtenerEstado < 7){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 7){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }
        case "En trámite por la AAPP": {
            if(obtenerEstado < 8){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 8){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }
        case "Pendiente de subsanación": {
            if(obtenerEstado < 9){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 9){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }
        case "Solicitud cerrada": {
            if(obtenerEstado < 10){
                formContext.data.process.movePrevious(moveNext);
            }
            else if(obtenerEstado > 10){
                formContext.data.process.moveNext(moveNext);
            }
            break;
        }*/
    }
}

async function onStageChange(executionContext){
    var formContext = executionContext.getFormContext();
    await validateStageProcess(formContext);
}

function moveNext(data){

}

function guardar(executionContext){
    debugger;
    var formContext = executionContext.getFormContext();
    var stage = formContext.data.process.getSelectedStage();
    formContext.data.process.moveNext(moveNext);
}