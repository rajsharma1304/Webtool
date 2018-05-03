/* 
 SICT
 */

var Utils = {
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
        // var me = this, jsonDoc = communion, url = getJsonInfoAction(SESSIONVALIDATE);
        // me.sessionWorkerObj = undefined;
        // if (typeof (Worker) !== "undefined") {
            // me.sessionWorkerObj = new Worker("js/sessionValidator.js");
            // me.sessionWorkerObj.onmessage = function(e) {
                // me.handlesessionValidation(e.data);
            // };
            // me.sessionWorkerObj.onerror = function(e) {

            // };
            // me.sessionWorkerObj.postMessage(url);
        // }
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
                    setTimeout(function(){
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
        if (user && sessionId && sessionlen && user != "" && sessionId != "")
            $(".user_menu .dropdown .dropdown-toggle").html("<img src='img/user_avatar.png' class='user_avatar'>" + user + "<b class='caret'></b>");
        else
            window.location.href = "login.html";
    }
};
