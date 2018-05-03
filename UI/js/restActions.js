/* 
 SICT
 */
var 	SESSIONVALIDATE = 0,
        SUBMITDEPARTUREFORM = 1,
		VIEWFORMS = 2, 
		VIEWFORMSDELETE = 3,
		UPDATEDEPARTUREFORM = 4;

function getJsonInfoAction(apiIdx) {
    var jsonDoc = communion, data, svc, api, action, addVersionNo = false, addSessionId = false;
    sessionStorage.serverUrl = jsonDoc.ipAddress;
    switch (apiIdx) {
        case SESSIONVALIDATE:
            data = jsonDoc.SessionValidateDetails;
            api = data.ValidateDetails;
            addVersionNo = false;
            addSessionId = true;
            break;
		case SUBMITDEPARTUREFORM:
			data = jsonDoc.DepartureForm;
			api = data.submitForm;
			break;
		case VIEWFORMS:
            data = jsonDoc.ViewFormDataTables;
            api = data.departureDetails;
            addVersionNo = false;
            addSessionId = false;
            break;
        case VIEWFORMSDELETE:
            data = jsonDoc.ViewFormDataTables;
            api = data.deleteAirline;
            addVersionNo = false;
            addSessionId = false;
            break;   
		case UPDATEDEPARTUREFORM:
			data = jsonDoc.DepartureForm;
			api = data.updateForm;
			break;			
        default:
            break;
    }
    svc = data.svc;
    action = "http://" + sessionStorage.serverUrl + "/" + svc + "/" + api;
    if (addVersionNo)
        action += "/" + versionNumber;
    if (addSessionId)
        action += "/" + sessionStorage.sessionId;
    return action;
}

function authLoginAction(uname, pword) {
    var jsonDoc = communion,
            loginData = jsonDoc.login,
            svc = loginData.encSvc,
            api = loginData.encMethod,
            url = "http://" + jsonDoc.ipAddress + "/" + svc + "/" + api,
            obj = {};
    obj.HashString = $.md5(uname + '&' + pword); // $.md5(username + '&' + password, null, true);

    $.ajax({
        type: 'POST',
        url: url,
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify(obj),
        processdata: true,
        error: function(err) {
            loginFailure(err);
        },
        success: function(res) {
            if (res && res.LoginResult)
                loginSuccess(res.LoginResult);
        }
    });
}

function sendDepartureForm(callback, isUpdate) {
    var jsonDoc = communion, url = getJsonInfoAction(SUBMITDEPARTUREFORM),
            sessionId = sessionStorage.sessionId,
            version = jsonDoc.Version,
            formDepObj = newformDeparture.depFormData,
            obj = {},
			type='PUT';
	if(isUpdate){
		type='POST';
		url = getJsonInfoAction(UPDATEDEPARTUREFORM);
	}
    if (newformDeparture.isFormSubmittable && sessionId && version && sessionId !== "" && version != "" && !($.isEmptyObject(formDepObj))) {
        obj.SessionId = sessionId,
		obj.Version = version,
		obj.FormDetails = formDepObj;
    }

    $.ajax({
        type: type,
        url: url,
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify(obj),
        error: function(err) {
            newformDeparture.triggerError(err);
        },
        success: function(res) {
			if(null === callback || undefined === callback)
				$("#Submit").button("reset");
            if (res && res.ReturnCode > 0 && res.AirlineDetails)
                newformDeparture.triggerSuccess(res, callback);
            else
                newformDeparture.triggerError(res);
        }
    });
}

// Post data to server
function ajaxPost(postUrl, postData, message, callback, attr, config, type) {
    // If no config, enpty config
    if (!isDefinedAndNotNull(config))
        config = {};
    // If no loader message, set to SAVING
    if (!isDefinedAndNotNull(config.loaderTitle) || typeof config.loaderTitle !== "string" || $.trim(config.loaderTitle) === "")
        config.loaderTitle = "SAVING";
    if (!isDefinedAndNotNull(config.loaderText) || typeof config.loaderText !== "string" || $.trim(config.loaderText) === "")
        config.loaderText = "";
    // If no loader, show loader
    if (!isDefinedAndNotNull(config.displayLoader) || config.displayLoader === true)
        //showLoader(config.loaderTitle, config.loaderText);

    $.ajax({
        type: type, //GET or POST or PUT or DELETE verb
        url: postUrl, // Location of the service
        data: JSON.stringify(postData), //Data sent to server
        contentType: "application/json", // content type sent to server
        //dataType: "jsonp",
        crossDomain: true,
        success: function(data) {
            //hideLoader();

            // Check for session Expiry
            if (typeof data.ReturnCode === "undefined")
                console.log("Return code missing: " + postUrl);
            else {
                if (data.ReturnCode === 0) {
                    //showAlert(globalMessage.title,globalMessage.sessionExpired,'OK');
                    // Take back to login screen
                    //logout();
                    callCustomPopUp(globalMessage.title, globalMessage.sessionExpired, logOut());
                    return;
                }
            }

            // Callback
            if (attr) {
                if (typeof callback === "function")
                    callback(data, true, attr);
            } else {
                if (typeof callback === "function")
                    callback(data, true);
            }

        },
        error: function(data, jqXHR, textStatus, errorThrown) {
            //hideLoader();            
            if (typeof callback === "function")
                callback(data, false);
        }
    });
}

// Undefined and Null checker
function isDefinedAndNotNull(element) {
    if (typeof element !== "undefined" && element !== null)
        return true;
    else
        return false;
}
