/*<Copyright> Cross-Tab </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> utils.js </FileName>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var pageUtils = {
    sessionWorkerObj: undefined,
    msg_SUCCESS: "st-success",
    msg_ERROR: "st-error",
    msg_INFO: "st-info",
    msg_POSITION: {
        top: {
            RIGHT: "top-right",
            LEFT: "top-left",
            CENTER: "top-center"
        }
    },
    STICKY_CLOSE_TIMEOUT: 5000,
    TERMINATE_SESSION: 5000,
    TERMINATE_SESSION_MSG: "Session has expired.",
    notify: function(msg, position, type) {
        var me_ = this;
        if (msg && position && type)
            $.sticky(msg, {autoclose: me_.STICKY_CLOSE_TIMEOUT, position: position, type: type});
        else
            $.sticky(msg, {autoclose: me_.STICKY_CLOSE_TIMEOUT, position: me_.msg_POSITION.top.RIGHT, type: me_.msg_INFO});
    },
    triggerError: function(rCode) {
        var me_ = this;
        if (rCode === 0)
            me_.notify(me_.TERMINATE_SESSION_MSG, me_.msg_POSITION.top.RIGHT, me_.msg_INFO);
    },
    intializeSessionWorker: function() {
        var me = this, jsonDoc = communion, url = getJsonInfoAction(SESSIONVALIDATE);
        me.sessionWorkerObj = undefined;
        if (typeof (Worker) !== "undefined") {
            me.sessionWorkerObj = new Worker("js/sict/sessionValidator.js");
            me.sessionWorkerObj.onmessage = function(e) {
                me.handlesessionValidation(e.data);
            }
            me.sessionWorkerObj.onmessage = function(e) {
                me.handlesessionValidation(e.data);
            };
            me.sessionWorkerObj.onerror = function(e) {
            };
            me.sessionWorkerObj.postMessage(url);
        }
    },
    handlesessionValidation: function(msg) {
        var me = this, parsedData = JSON.parse(msg);
        if (typeof (Worker) !== 'undefined') {
            if (parsedData.ReturnCode === 1 && parsedData.ReturnMessage === "Session Valid") {
            }
            else {
                if (typeof me.sessionWorkerObj !== "undefined") {
                    me.sessionWorkerObj.terminate();
                    me.triggerError(parsedData.ReturnCode);
                    setTimeout(function() {
                        logoff();
                    }, me.TERMINATE_SESSION);
                    //kendoCustomMsg(jsonDoc.messages.SessionExpired, "Alert", "alert");
                }
            }
        }
        else {
            if (msg == "false") {
                //kendoCustomMsg(jsonDoc.messages.SessionExpired, "Alert", "alert");
            }
        }
    },
    initPage: function() {
        var user = sessionStorage.username, sessionId = sessionStorage.sessionId, sessionlen;
        if (typeof sessionId !== "undefined" && sessionId != "") {
            sessionlen = sessionStorage.sessionId.length;
        }

        if (user && sessionId && sessionlen && user != "" && sessionId != "") {
            user = (sessionStorage.roleId !== '1') ?
                    user.toUpperCase() + ' ( ' + sessionStorage.airportName + ' ) ' : user.toUpperCase();
            $(".user_menu .dropdown .dropdown-toggle").html("<img src='img/user_avatar.png' class='user_avatar'><strong>" + user + "</strong><b class='caret'></b>");
        }
        else
            window.location.href = "login.html";
    },
    confirmAccess: function() {
        if (sessionStorage.arivalFormAccess !== 'true') {
            $('#newArrivalForm').remove();
            $('#viewArrivalForm').remove();
        }
        else {
            $('#newArrivalForm').show();
            $('#viewArrivalForm').show();
        }

        if (sessionStorage.depFormAccess !== 'true') {
            $('#viewDepartureForm').remove();
            $('#newDepartureForm').remove();
        }
        else {
            $('#viewDepartureForm').show();
            $('#newDepartureForm').show();
        }
    },
    confirmIconAccess: function() {
        if (sessionStorage.arivalFormAccess !== 'true') {
            $('#newFomArrival').hide();
        }
        else {
            $('#newFomArrival').show();
        }

        if (sessionStorage.depFormAccess !== 'true') {
            $('#newFomDeparture').hide();
        }
        else {
            $('#newFomDeparture').show();
        }
    },
    loadMenu: function() {
        var me = this, data = communion, host = file_name = data ? data.hostInstance : "";
        if (sessionStorage.menuItem === undefined) {
            switch (file_name) {
                case "USI":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/menu/menu_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                case "USD":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/menu/menu_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                case "EUR":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/menu/menu_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                case "AIR":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/menu/menu_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                default:
                    window.location.href = "login.html";
                    break;
            }
            switch (sessionStorage.roleId) {
                case '0'://airport
                    sessionStorage.menuItem = airportNavBar;
                    break;
                case '1'://cross-tab //Mindset admin
                    if (sessionStorage.isSuperAdmin === "false")
                        sessionStorage.menuItem = mindsetNavBar;
                    else
                        sessionStorage.menuItem = adminNavBar;
                    break;
                case '2'://airport admin
                    sessionStorage.menuItem = airportAdminNavBar;
                    break;
                default:
                    window.location.href = "login.html";
                    break;
            }
        }
        $('#leftMenuPanel').append(sessionStorage.menuItem);
		//Changes for Special Airport
		if(sessionStorage["IsSpecialUser"] && sessionStorage["IsSpecialUser"] === "true"){
			$("a[href^=Feedbackreport]").parent("li").remove();
			$("a[href^=airportreport]").parent("li").remove();
			$("a[href^=searchpage]").parent("li").remove();
		}
        if ((sessionStorage.arivalFormAccess !== 'true') && (sessionStorage.depFormAccess !== 'true')) {
            $('#newFormsMenu').remove();
            $('#viewFormsMenu').remove();
        }
        else {
            try {
                if (sessionStorage.roleId === "1" && sessionStorage.isSuperAdmin === "true") {//Cross-tab Over
                    $('#newFormsMenu').remove();
                    $('#viewFormsMenu').show();
                }
                else if ((sessionStorage.roleId === "1" || sessionStorage.roleId === "2" || sessionStorage.roleId === "3") && sessionStorage.isSuperAdmin === "false") {//Mindset,  Airport Admin
                    if (host === "USI" || host === "AIR") {
                        if (sessionStorage.roleId === "1") {//Mindset
                            //[#72660] Europe - Menu level combination is not as per the specification for the Mindset user.
                            $('#newFormsMenu').remove();
                            $('#viewFormsMenu').remove();
                        }
                        else {//Airport Admin
                            me.confirmAccess();
                        }
                    }
                    else {//Other Host
                        $('#newFormsMenu').remove();
                        $('#viewFormsMenu').remove();
                    }
                }
                else {//Airport Login
                    //[#72654] Europe - Drop down is not coming for the New Forms and View forms for Europe instance. CDG login
                    me.confirmAccess();
                }
            }
            catch (e) {
                debugger;
                console.log(e);
            }
        }
    },
    loadDashboardIcons: function() {
        var me = this, data = communion, file_name = host = (data ? data.hostInstance : "");
        if (sessionStorage.dashboardIcons === undefined) {
            switch (file_name) {
                case "USI":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/dashboard/dashboard_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                case "USD":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/dashboard/dashboard_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                case "EUR":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/dashboard/dashboard_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                case "AIR":
                    $("<script/>", {type: 'text/javascript', src: 'js/cache/dashboard/dashboard_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                    break;
                default:
                    window.location.href = "login.html";
                    break;
            }
            switch (sessionStorage.roleId) {
                case '0'://airport
                    sessionStorage.dashboardIcons = airportIcons;
                    break;
                case '1'://cross-tab //Mindset Admin
                    if (sessionStorage.isSuperAdmin === "false")
                        sessionStorage.dashboardIcons = mindsetIcons;
                    else
                        sessionStorage.dashboardIcons = adminIcons;
                    break;
                case '2'://airport admin
                    sessionStorage.dashboardIcons = airportadminIcons;
                    break;
					
					 case '3'://airport admin
                    sessionStorage.dashboardIcons = airportadminIcons;
                    break;
					
                default:
                    window.location.href = "login.html";
                    break;
            }
        }
        me.updateLastUpload();
        $('#dashboardIconsContainer').append(sessionStorage.dashboardIcons);
		if(sessionStorage["IsSpecialUser"] && sessionStorage["IsSpecialUser"] === "true"){
			$("a[href='airportreport.html']").parent('li').remove();
		}
        if ((sessionStorage.arivalFormAccess !== 'true') && (sessionStorage.depFormAccess !== 'true')) {
            $('#newFomArrival').hide();
            $('#newFomDeparture').hide();
        }
        else {
            try {
                if (sessionStorage.roleId === "1" && sessionStorage.isSuperAdmin === "true") {//Cross-tab Over
                    $('#newFomArrival').hide();
                    $('#newFomDeparture').hide();
                }
                else if ((sessionStorage.roleId === "1" || sessionStorage.roleId === "2" || sessionStorage.roleId === "3") && sessionStorage.isSuperAdmin === "false") {//Mindset,  Airport Admin
                    if (host === "USI" || host === "AIR") {
                        if (sessionStorage.roleId === "1") {//Mindset
                            $('#newFomArrival').hide();
                            $('#newFomDeparture').hide();
                        }
                        else {//Airport Admin
                            me.confirmIconAccess();
                        }
                    }
                    else {//Other Host
                        $('#newFomArrival').hide();
                        $('#newFomDeparture').hide();
                    }
                }
                else {//Airport Login
                    //[#72654] Europe - Drop down is not coming for the New Forms and View forms for Europe instance. CDG login
                    me.confirmIconAccess();
                }
            }
            catch (e) {
                debugger;
                console.log(e);
            }
        }
    },
    loadReportControls: function() {
        var data = communion, file_name = data ? data.hostInstance : "";
        switch (file_name) {
            case "USI":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/formcontrols/formcontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            case "USD":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/formcontrols/formcontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            case "EUR":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/formcontrols/formcontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            case "AIR":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/formcontrols/formcontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            default:
                window.location.href = "login.html";
                break;
        }
    },
    loadManageControls: function() {
        var data = communion, file_name = data ? data.hostInstance : "";
        switch (file_name) {
            case "USI":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/manageControls/managecontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            case "USD":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/manageControls/managecontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            case "EUR":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/manageControls/managecontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            case "AIR":
                $("<script/>", {type: 'text/javascript', src: 'js/cache/manageControls/managecontrols_' + file_name + '.jsonp'}).appendTo("#maincontainer");
                break;
            default:
                window.location.href = "login.html";
                break;
        }
    },
    updateLastUpload: function() {
        if (typeof sessionStorage && sessionStorage.lastUpload && sessionStorage.lastUploadSwissTime) {
            var lastUpload = sessionStorage.lastUpload + " / " + sessionStorage.lastUploadSwissTime + " (Swiss Time)";
            if (lastUpload)
                $("#lastupload").html('Uploaded:&nbsp;' + '<small>' + lastUpload + '</small>');
        }
    },
    returnServerDate: function(input) {
        var date = new Array();
        date = input.split('/');
        return date[2] + '-' + date[0] + '-' + date[1];
    },
    returnValue: function(value) {
        var val;
        if ((value === 'select') || (value === '') || (value === null))
            val = '-1';
        else
            val = value;
        return val;
    },
    ensureReturnValue: function(value, name) {
        if ((value === 'select') || (value === '-1') || (value === '')) {
            pageUtils.notify('Please select a valid option for ' + name, 'top-right', 'Flight Combinations');
            return false;
        } else {
            return true;
        }
    },
    parseValue: function(val){
        if(val)
            return val.replace(/'/g, '&#39').replace(/"/g, '&#34;').trim();
    }
};

$(document).ready(function() {
    pageUtils.loadMenu();
    pageUtils.initPage();
    pageUtils.intializeSessionWorker();
    /*selectnav('mobile-nav', {
            indent: '-'
    });*/
});