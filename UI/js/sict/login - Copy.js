/*<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> login.js </FileName>
 <Author> Raghavendra G.N, Akhilesh M.S, Vivek.A </Author>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
function login() {
    var username = $("#username"),
            userVal = $("#username").val().trim(),
            password = $("#password"),
            passVal = $("#password").val(),
            checkbox = $("#remember_me"),
            pa_usr = username.parents(".input-prepend"),
            pa_pass = password.parents(".input-prepend");
    pa_usr.removeClass("f_error");
    pa_pass.removeClass("f_error");
    pa_usr.find(".error").remove();
    pa_pass.find(".error").remove();
    if (checkbox[0].checked) {
        localStorage.setItem('uName', userVal);
        localStorage.setItem('pWord', passVal);
    }
    else {
        localStorage.removeItem('uName');
        localStorage.removeItem('pWord');
        localStorage.clear();
    }
    if (username && password && userVal.length > 0 && passVal.length > 0) {
        $('#spinnerIndicator').show();
        authLoginAction(userVal.toLowerCase(), passVal.toLowerCase());
        return true;
    }
    else {
        if (userVal != "") {
            if (userVal.length < 0) {
                pa_usr.addClass("f_error");
                pa_usr.append('<label for="username" generated="true" class="error">Fields cannot be empty</label>');
            }
        }
        else {
            pa_usr.addClass("f_error");
            pa_usr.append('<label for="username" generated="true" class="error">Mandatory field</label>');
        }
        if (passVal == "") {
            pa_pass.addClass("f_error");
            pa_pass.append('<label for="username" generated="true" class="error">Mandatory field</label>');
        }
        return false;
    }
}

function loginSuccess(res) {
    var username = $("#username"),
            password = $("#password"),
            pa_usr = username.parents(".input-prepend"),
            pa_pass = password.parents(".input-prepend");
    pa_usr.removeClass("f_error");
    pa_pass.removeClass("f_error");
    pa_usr.find(".error").remove();
    pa_pass.find(".error").remove();
    try {
        if (!res.IsValidUser) {//Not a Valid user.
            $('#spinnerIndicator').hide();
            pa_usr.addClass("f_error");
            pa_pass.addClass("f_error");
            pa_pass.append('<label for="username" generated="true" class="error">Invalid Username / Password</label>');
        }
        else {//Valid User.//SessionStorage is allowed to Store Only Numbers and String data type.
            sessionStorage.sessionId = res.SessionId;
            sessionStorage.airportName = res.AirportName;
            sessionStorage.airportLoginId = res.AirportLoginId
            sessionStorage.depFormAccess = res.DepartureFormAccess;
            sessionStorage.arivalFormAccess = res.ArivalFormAccess;
            sessionStorage.validUser = res.IsValidUser;
            sessionStorage.roleId = res.RoleId;
            sessionStorage.username = $("#username").val().trim();
            sessionStorage.isSuperAdmin = res.IsSuperAdmin;
            if (res && res.LastUploadDate) {//mm/dd/yyyy/hh/mm
                var date = res.LastUploadDate.split("/"),
                        lastuploaded = moment(date[2] + "-" + date[0] + "-" + date[1] + " " + date[3] + ":" + date[4]).fromNow(), //Give input as yyyymmdd
                        datetime = (new Date(date[2] + "/" + date[0] + "/" + date[1] + " " + date[3] + ":" + date[4])).getTime();
                swiss = moment(datetime).zone("+02:00").format("MMMM Do YYYY, H:mm");//CET Time
                sessionStorage.lastUpload = lastuploaded;
                if (swiss)
                    sessionStorage.lastUploadSwissTime = swiss;
            }
            window.location.href = "dashboard.html";
        }
    }
    catch (e) {
        sessionStorage.clear();
        location.reload();
    }
}

function loginFailure(er) {
    alert("Network Error. Please try again.");
}

function logoff() {
    //Clear all Sessions.
    sessionStorage.clear();
    window.location.href = "login.html";
}

function retreiveLoginDetails() {
    var checkBox = $('#remember_me'),
            usernameField = $('#username'),
            passwordField = $('#password'),
            username = localStorage.getItem('uName'),
            isUserNameExist = false;

    if ((username != null || username != undefined)) {
        isUserNameExist = true;
    }

    if (isUserNameExist) {
        usernameField.val(username).focus();
        passwordField.val(localStorage.getItem('pWord'));
        checkBox.attr('checked', true);
    }
    else {
        usernameField.val('').focus();
        passwordField.val('');
        checkBox.attr('checked', false);
    }
}

function validateForgotPassword() {

    var username = $("#FPUsername"),
            userVal = $("#FPUsername").val().trim(),
            emailID = $("#FPEmailId"),
            emailVal = $("#FPEmailId").val().trim(),
            pa_usr = username.parents(".input-prepend"),
            pa_email = emailID.parents(".input-prepend");

    pa_usr.removeClass("f_error");
    pa_email.removeClass("f_error");
    pa_usr.find(".error").remove();
    pa_email.find(".error").remove();
    var canContinue = true;
    if (userVal != "") {
        if (userVal.length < 0) {
            pa_usr.addClass("f_error");
            pa_usr.append('<label for="username" generated="true" class="error">Fields cannot be empty</label>');
        }
    }
    else {
        canContinue = false;
        pa_usr.addClass("f_error");
        pa_usr.append('<label for="username" generated="true" class="error">Mandatory field</label>');
    }
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    var isValid = regex.test(emailVal);

    if (!isValid) {
        pa_email.addClass("f_error");
        pa_email.append('<label for="username" generated="true" class="error">Enter valid email id</label>');
        canContinue = false;
    }
    if (emailVal == "") {
        pa_email.addClass("f_error");
        pa_email.append('<label for="username" generated="true" class="error">Mandatory field</label>');
        canContinue = false;
    }

    if (canContinue) {
	sendForgotPasswordMail(userVal,emailVal);
        // $('#infoMessage').html('An email has been sent to id, please have a look.');
        // setTimeout(function() {
            // $('#backToLogin').click();
            // $('#infoMessage').html('Please enter your email address and Login ID. Admin will contact you with the required information.');
        // }, 2000);

    }
    return canContinue;

}

function sendForgotPasswordMail(uname, emailId) {
    var jsonDoc = communion,
            loginData = jsonDoc.login,
            svc = loginData.encSvc,
            service = jsonDoc.Service,
            api = "UserManagement/ForgotPassword",
            url = "http://" + jsonDoc.ipAddress + "/" + service + "/" + svc + "/" + api,
            objMail = {};   
    objMail.Instance = getInstance();
	objMail.Version="V1";	
	objMail.UserName =uname;
	objMail.MailId=emailId;

    $.ajax({
        type: 'POST',
        url: url,
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify(objMail),
        processdata: true,
        error: function() {
            sendForgotPasswordMailFailure();
        },
        success: function(res) {
            if (res && res.ForgotPasswordResult)
                sendForgotPasswordMailSuccess(res.ForgotPasswordResult);
        }
    });
}

function sendForgotPasswordMailFailure() {
    $('#infoMessage').html('Sending email to Admin failed please try after sometime.');
}

function sendForgotPasswordMailSuccess(res) {
if(res.ReturnCode>0)
     $('#infoMessage').html('An email has been sent to Admin.');
	 else
	 $('#infoMessage').html('Sending email to Admin failed please try after sometime.');
}

