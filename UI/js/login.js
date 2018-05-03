/* 
 SICT
 */
function login() {
    var username = $("#username"),
            userVal = $("#username").val().trim(),
            password = $("#password"),
            passVal = $("#password").val().toLowerCase(),
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
        authLoginAction(userVal, passVal);
        return true;
    }
    else {
        if(userVal != ""){
            if(userVal.length < 0){
                pa_usr.addClass("f_error");
                pa_usr.append('<label for="username" generated="true" class="error">Fields cannot be empty</label>');
            }
        }
        else{
            pa_usr.addClass("f_error");
            pa_usr.append('<label for="username" generated="true" class="error">Mandatory field</label>');
        }
        if(passVal == ""){
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
            sessionStorage.airportLoginId = res.AirportLoginId
            sessionStorage.depFormAccess = res.DepartureFormAccess;
            sessionStorage.arivalFormAccess = res.ArivalFormAccess;
            sessionStorage.validUser = res.IsValidUser;
            sessionStorage.roleId = res.RoleId;
            sessionStorage.username = $("#username").val().trim();
            window.location.href = "dashboard.html";
        }
    }
    catch (e) {
        sessionStorage.clear();
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

function retreiveLoginDetails(){
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
