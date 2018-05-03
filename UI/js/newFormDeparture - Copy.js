/* 
 SICT
 */
var newformDeparture = {
    airlineCounter: 1,
    scrolled: 0,
    lang_data: [],
    isFormSubmittable: false,
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
    msg_LANG: {
        serial_exists: {
            msg: "Serial No. conflict in Airline",
        },
        error_notify: "Only 5 Languages is allowed per Airline."
    },
    depFormData: {},
    postSUBMISSION: [],
    initializeDepartureForm: function() {
        var me = this;
        //Intialize FieldWork with today's date.
        $('#fieldworkDate').datepicker({
            format: "yyyy-mm-dd",
            autoclose: true,
            orientation: "top left",
            todayHighlight: true
        }).on('changeDate', function(ev) {
            if ($('#fieldworkDate').valid()) {
                $('#fieldworkDate').parent('div').removeClass('sict_error');
            }
            else {
                $('#fieldworkDate').parent('div').addClass('sict_error');
            }
        }).datepicker('setValue', new Date());
        //Binding event for adding new Airlines.
        $('body').on("click", "#addAirlines", function() {
            newformDeparture.addnewAirlines();
        });

        $("input").focusin(function() {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('.sict_error');
        });

        $("#Submit").on("click", function() {
<<<<<<< .mine
            $("[id=newDepartureForm]").removeClass('sict_error').removeClass('sict_row_error');
=======
            $("body").removeClass('sict_error').removeClass('sict_row_error');
>>>>>>> .r40
        });

        initialiseStatics();
        me.checkValueisNumber("");
    },
    checkValueisNumber: function(elem) {
        if (elem != "") {
            var inputValue = elem[0].value.replace(/\s+/g, "");
            if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > 0) {
                    elem.parent('div').removeClass('sict_error');
                    return true;
                }
            }
            //	Add the error class to inform it is invalid
            elem.parent('div').addClass('sict_error');
            return false;
        }

        $("input[type=text]:not('#fieldworkDate')").focusout(function() {
            var inputValue = $(this)[0].value.replace(/\s+/g, "");
            ;
            if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > 0) {
                    $(this).parent().removeClass('sict_error');
                    return;
                }
            }
            //	Add the error class to inform it is invalid
            $(this).parent().addClass('sict_error');
        });
    },
    triggerError: function(resp) {
        var me_ = this;
        if (resp.ReturnCode > 0) {
        }
        else
            me_.notify(resp.ReturnMessage, me_.msg_POSITION.top.RIGHT, me_.msg_INFO);
    },
    notify: function(msg, position, type) {
        var me_ = this;
        if (msg && position && type)
            $.sticky(msg, {autoclose: 5000, position: position, type: type});
        else
            $.sticky(msg, {autoclose: 2000, position: me_.msg_POSITION.top.RIGHT, type: me_.msg_INFO});
    },
    triggerSuccess: function(resp) {
        var me_ = this;
        if (resp.ReturnCode > 0) {
            //1) If Submission is successful, but serial is wrong.
            //2) If Submission is successful, with not errors.
//            if (resp.AirlineDetails && resp.AirlineDetails.length > 0) {
//                $.each(resp.AirlineDetails, function(i, val) {
//                    var airline = val;
//                    if (!airline.IsSerialNoValid)
//                        me_.postSUBMISSION.push(i);
//                });
//                if (me_.postSUBMISSION.length > 0) {
//                    if (me_.postSUBMISSION.length === 1)
//                        me_.notify(me_.msg_LANG.serial_exists.ONE_AIRLINE + " " + me_.postSUBMISSION + ".", me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
//                    else {
//                        var temp = $.map(me_.postSUBMISSION, function(n, i) {
//                            return (i + 1);
//                        });
//                        me_.notify(me_.msg_LANG.serial_exists.MANY_AIRLINE + " " + temp.join(" ,") + ".", me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
//                        temp = undefined;
//                    }
//                }
//                else
//                    me_.notify(resp.ReturnMessage, me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
//
//            }
            if (resp.AirlineDetails && resp.AirlineDetails.length > 0) {
                var inValidAirlineLang = [], airline, langSection, langRow, airlineError = [];
                $.each(resp.AirlineDetails, function(i, airline) {
                    if (airline.IsSerialNoValid) {
                        inValidAirlineLang.push({});
                    }
                    else {
                        if (airline.InvalidLanguages === null)
                            return false;
                        inValidAirlineLang.push({airline: airline.AirlineId, invalidLang: airline.InvalidLanguages})
                        airlineError.push(i + 1);
                    }
                });
                console.log(inValidAirlineLang);
                $.each(inValidAirlineLang, function(i, obj) {
                    if (!$.isEmptyObject(obj)) {
                        airline = $(".airlinesChildContainer:eq(" + i + ")");
                        airline.addClass('sict_row_error');
                        langSection = airline.find("div[id^=languageAirline]");
                        $.each(obj.invalidLang, function(i, val) {
                            langRow = langSection.find(".row-fluid:eq(" + (parseInt(val, 10) - 1) + ")");
                            langRow.find("input[id^=fserial]").parent().addClass("sict_error");
                            langRow.find("input[id^=lserial]").parent().addClass("sict_error");
                        });
                    }
                });
                if (airlineError.length > 0) {
                    if (airlineError.length > 1)
                        me_.notify(me_.msg_LANG.serial_exists.msg + "s " + airlineError.join(" ,") + ".", me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
                    else
                        me_.notify(me_.msg_LANG.serial_exists.msg + " " + airlineError.join(" ,") + ".", me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
                }
                else {
                    if (resp.ReturnCode > 0)
                        me_.notify(resp.ReturnMessage, me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
                }
            }
            else {
                me_.postSUBMISSION = [];
                me_.notify(resp.ReturnMessage, me_.msg_POSITION.top.RIGHT, me_.msg_ERROR);
            }
        }//For Processing Invalid Session or Other DB Errors.
        else
            me_.notify(resp.ReturnMessage, me_.msg_POSITION.top.RIGHT, me_.msg_INFO);
    },
    checkInNewfDep: function() {
        return isDeparture;
    },
    submitFormDeparture: function(origin) {
        var origin = origin, me_ = this, isnewfDep = me_.checkInNewfDep(), parentEle = $("#airlinesContainer"), prevEle = parentEle.prev();
        if (prevEle.hasClass("alert"))
            prevEle.remove();
        if ($(".sict_error").length > 0)
            $(".sict_error").removeClass('sict_error');
        if (isnewfDep) {
            var depformp = $("#airlinesContainer"),
                    depformc = depformp.children(),
                    depformf = depformc.first(),
                    airportLabel = depformf.find("#AirportList").prev("label"),
                    airport = depformf.find("#AirportList option:selected").val(),
                    interviewerLabel = depformf.find("#InterviewerList").prev("label"),
                    interviewer = depformf.find("#InterviewerList option:selected").val(),
                    fieldworkDateLabel = depformf.find("#fieldworkDate").prev("label"),
                    fieldworkDate = $('#fieldworkDate'),
                    airlineChild = $(".airlinesChildContainer"),
                    airlineChildlen = airlineChild.length,
                    tempAirlines = [],
                    isFlightNoEmpty = false,
                    isFlightNoregexMatch = false,
                    isBusinessCardsEmpty = false,
                    isLangEmpty = false,
                    isFSEmpty = false,
                    isLSEmpty = false,
                    formErrorMsg = "",
                    fsLabel, lsLabel,
                    airlineLang = [],
                    depObj = {IsDepartureForm: "true", AirportId: "", FieldWorkDate: "", InterviewerId: "", Airlines: []};
            if (airport === "-1") {
//                airportLabel.append("<sup class='si-error'><i class='fa fa-circle'></i></sup>");
//                formErrorMsg += "Airport cannot be empty.<br>";
            }
            if (interviewer === "-1") {
//                interviewerLabel.append("<sup class='si-error'><i class='fa fa-circle'></i></sup>");
//                formErrorMsg += "Interviewer cannot be empty.<br>";
            }
            if (fieldworkDate.val() != "") {
                me_.isFormSubmittable = false;
            }


            depObj.AirportId = airport, depObj.FieldWorkDate = fieldworkDate, depObj.InterviewerId = interviewer;
            airlineChild.each(function(i, ele) {

                //Processing each Row
                var row = $(this), tempObj = {}, tempLang = [], airlineId, flightNo, destinationId, businessCards, code = "", languageRow = row.find("div[id^=languageAirline] .row-fluid"), isFSEmpty = false, isLSEmpty = false, langValidation = [];
                airlineId = row.find(".airlinesdrop option:selected").val();
                airlineIdLabel = row.find(".airlinesdrop").prev("label");
                flightNo = row.find("input[id^=flight]");
                flightNoLabel = row.find("input[id^=flight]").prev("label");
                destinationId = row.find("select[id^=destination] option:selected").val();
                destinationIdLabel = row.find("select[id^=destination]").prev("label");
                businessCards = row.find("input[id^=businesscard]");
                businessCardsLabel = row.find("input[id^=businesscard]").prev("label");
                code = row.find("input[id^=code]").val();
                codeLabel = row.find("input[id^=code]").prev("label"),
                        langODetails = [];

                if (!me_.checkValueisNumber(flightNo)) {
                    me_.isFormSubmittable = false;
                }
                if (!me_.checkValueisNumber(businessCards)) {
                    me_.isFormSubmittable = false;
                }
                if (me_.isFormSubmittable)
                    tempObj.AirlineId = airlineId, tempObj.FlightNumber = flightNo, tempObj.DestinationId = destinationId, tempObj.BCardsDistributed = businessCards, tempObj.OriginId = 1, tempObj.Route = "TATL", tempObj.Direction = "EAST"; //, me_.isFormSubmittable = true;//, tempObj.Code = code;

                //if (tempObj.Code === "")//Deleting the Code key temporarily
                delete tempObj["Code"];

                //Processing Language
                $.each(languageRow, function(j, val) {
                    var row = $(this), lang, fs, ls, tempLangObj = {}, langLabel, airlineLang = [];
                    lang = row.find("option:selected").val(),
                            fs = row.find("input[id^=fserial]"),
                            ls = row.find("input[id^=lserial]"),
                            langLabel = row.find("select").prev("label");
                    if (!me_.checkValueisNumber(fs) && !me_.checkValueisNumber(ls)) {
                        me_.isFormSubmittable = false;
                    }
                    else {
                        tempLangObj.LanguageId = lang, tempLangObj.FirstSerialNo = fs, tempLangObj.LastSerialNo = ls, me_.isFormSubmittable = true;
                        tempLang.push(tempLangObj);
                    }
                });
                tempObj.Languages = tempLang;
                depObj.Airlines.push(tempObj);
            });
            if ($(".sict_error").length > 0) {
                me_.isFormSubmittable = false;
            }
            else {
                me_.isFormSubmittable = true;
                me_.depFormData = depObj;
            }
        }


        //All fields has values, But do Language Validation here.
        if (me_.isFormSubmittable) {
            if ($(".sict_error").length > 0)
                return false;
            else {
                $("#Submit").button('loading');
                sendDepartureForm();
            }
            //me_.langaugeValidaiton();
        }
    },
    newAirline: function() {
        var nAir = '<div class="airlinesChildContainer row-fluid">' +
                '<div class="span12 formSep">' +
                '<i class="close airlineclose" onclick="newformDeparture.removeAirline(this);">&times;</i>' +
                '<div class="row-fluid">' +
                '<div class="span6">' +
                '<h4 id="airlineHeader{0}" class="sepH_c"> Airlines {0} </h4>' +
                '<div class="span6">' +
                '<label>Airline </label>' +
                '<select id="airlinesDD{0}" name="airlinesDD{0}" class="span10 airlinesdrop" onchange="callOnAirlinesChange({0})">' +
                airlineString +
                '</select>' +
                '</div>' +
                '<div class="span2" style="margin-left:23px;">' +
                '<label>Code</label>' +
                '<input id="code{0}" name="code{0}" disabled="disabled" type="text" class="span8" value="200">' +
                '</div>' +
                '<div class="span3">' +
                '<label>Flight No. </label>' +
                '<input id="flight{0}" name="flight{0}" type="text" class="span10">' +
                '</div>' +
                '<div class="span12">' +
                '<div class="span6">' +
                '<label>Destination</label>' +
                '<select class="span10 destdrop" id="destination2" name="destination2">' +
                firstAirlineDestinatons +
                '</select>' +
                '</div>' +
                '<div class="span3" style="margin-left:23px;">' +
                '<label>Business Cards</label>' +
                '<input type="text" class="span5" id="businesscard{0}" name="businesscard{0}">' +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div id="{0}" class="span6">' +
                '<button type="button" class="btn sepH_b btn-pink addLanguage" data-langcounter="1" onclick="newformDeparture.addLanguage(this);"> Add Language </button>' +
                '<div id="languageAirline{0}">' +
                '<div class="row-fluid" style="margin-top:-2px;">' +
                '<div class="span6">' +
                '<label>Language </label>' +
                '<select class="span10" id="language{0}_1" name="language{0}_1">' +
                languageString +
                '</select>' +
                '</div>' +
                '<div class="span3">' +
                '<label>First Serial No.</label>' +
                '<input type="text" class="span10" id="fserial{0}_1" name="fserial{0}_1">' +
                '</div>' +
                '<div class="span3">' +
                '<label>Last Serial No. </label>' +
                '<input type="text" class="span10" id="lserial{0}_1" name="lserial{0}_1">' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>';
        return nAir;
    },
    addLanguage: function(e) {
        var me_ = this,
                me = $(e),
                langcounterval = parseInt($(e).data("langcounter"), 10);
        $(e).data("langcounter", "" + (langcounterval + 1));
        var count = me.siblings().find(".row-fluid").length;
        if (count < 5) {
            var parentId = me.parent().attr('id');
            var languageRowString = '<div class="row-fluid" style="margin-top:0px;" >' +
                    '<div class="span6">' +
                    '<select class="span10" id="language' + parentId + '_' + (langcounterval + 1) + '" name="language' + parentId + '_' + (langcounterval + 1) + '' + (langcounterval + 1) + '">' +
                    languageString +
                    '</select>' +
                    '</div>' +
                    '<div class="span3">' +
                    '<input type="text" class="span10" id="fserial' + parentId + '_' + (langcounterval + 1) + '' + '" name="fserial' + parentId + '_' + (langcounterval + 1) + '"/>' +
                    '</div>' +
                    '<div class="span3">' +
                    '<input type="text" class="span10" id="lserial' + parentId + '_' + (langcounterval + 1) + '' + '" name=lserial' + parentId + '_' + (langcounterval + 1) + '' + '"/>' +
                    '</div>' +
                    '</div>';
            me.siblings().append(languageRowString);
            //Validation for First Serial and Last Serial.
            //$('input[id^=fserial' + parentId + '_' + (langcounterval + 1) + ']').rules("add", {required: true, number: true, messages: {required: ""}});
            var lserial = $('input[id^=lserial' + parentId + '_' + (langcounterval + 1) + ']');
            if (count > 0) {
                lserial.after('<i class="close langclose" onclick="newformDeparture.removeLangauage(this);">&times;</i>');
                //lserial.rules("add", {required: true, number: true, messages: {required: ""}});
            }
            // else {
            // lserial.rules("add", {required: true, number: true, messages: {required: ""}});
            // }
            if (count === 4) {
                me_.notify(me_.msg_LANG.error_notify, me_.msg_POSITION.top.RIGHT, me_.msg_ERROR);
                me.prop("disabled", "disabled");
            }
        } else {
            me.prop("disabled", "disabled");
        }
    },
    addnewAirlines: function() {
        var me = this;
        var template = jQuery.validator.format($.trim((me.newAirline())));
        $('#airlinesContainer').append($(template(++me.airlineCounter)));
        setTimeout(function() {
            me.scrollDown();
        }, 1500);
        // if (me.airlineCounter >= 3)
        // me.addvalidationRules(me.airlineCounter);
        me.checkValueisNumber("");
    },
    addvalidationRules: function(ct) {
        $('input[id^=flight' + ct + ']').rules("add", {required: true, number: true, min: 0, messages: {required: ""}});
        $('input[id^=businesscard' + ct + ']').rules("add", {required: true, number: true, min: 0, messages: {required: ""}});
        $('input[id^=fserial' + ct + '_' + 1 + '' + ']').rules("add", {required: true, number: true, min: 0, messages: {required: ""}});
        $('input[id^=lserial' + ct + '_' + 1 + '' + ']').rules("add", {required: true, number: true, min: 0, messages: {required: ""}});
    },
    removeLangauage: function(e) {
        var rows = $(e).parents("div[id^=languageAirline]").find(".row-fluid").length;
        if (rows <= 5)
            $(e).parents("div[id^=languageAirline]").prev('button').removeAttr('disabled');
        $(e).parent().parent().remove();
    },
    removeAirline: function(e) {
        $(e).parents(".airlinesChildContainer").remove();
    },
    scrollDown: function() {
        var me = this;
        me.scrolled = me.scrolled + 300;
        $(".main_content").animate({
            scrollTop: me.scrolled
        });
    }
};

function initialiseStatics() {
	var airportURL = "./cache/AirportList/AirportList_" + sessionStorage.airportLoginId + ".json";
	var interviewerURL = "./cache/InterviewerList/InterviewerList_" + sessionStorage.airportLoginId + ".json";
	if (sessionStorage.roleId === '0') {
		airportURL = "./cache/AirportList/AirportList_" + sessionStorage.airportLoginId + ".json";	
		interviewerURL = "./cache/InterviewerList/InterviewerList_" + sessionStorage.airportLoginId + ".json";
	} else if (sessionStorage.roleId === '1') {
        airportURL = "./cache/AirportList/AirportList_Admin.json";
		interviewerURL = "./cache/InterviewerList/InterviewerList_Admin.json";
	}
	
    airportURL = "./cache/AirportList/AirportList_" + sessionStorage.airportLoginId + ".json";
	$.getJSON(airportURL, function(response, status) {
		if (status == "success") {
			$('#AirportList').append('<option selected="true" value=' + response[0].AirportId + '>' + response[0].AirportName + '</option>');
		}
	});

	interviewerURL = "./cache/InterviewerList/InterviewerList_" + sessionStorage.airportLoginId + ".json";
	$.getJSON(interviewerURL, function(response, status) {
		if (status == "success") {
			var optionString = '';
			for (var i = 0; i < response.length; i++) {
				optionString += '<option value=' + response[i].InterviewerId + '>' + response[i].InterviewerName + '</option>'
			}
			$('#InterviewerList').append(optionString);
		}
	});

    initialiseAirlineStatics();
}

var languageString = '', airlineString = '', firstAirlineCode = '', firstAirlineDestinatons = '';
function initialiseAirlineStatics() {

    var airlineURL = "./cache/AirportAirlineList/Departure_AirportAirlineList_" + sessionStorage.airportLoginId + ".json";
	if(!isDeparture)
		airlineURL = "./cache/AirportAirlineList/Arrival_AirportAirlineList_" + sessionStorage.airportLoginId + ".json";
    $.getJSON(airlineURL, function(response, status) {
        if (status == "success") {
            airlineVsProps = [];
            var optionString = '';
            var curAirlineID = '';
            var airlineCode = {};
            var isAdded = true;
            for (var i = 0; i < response.length; i++) {
                isAdded = false;
                if (curAirlineID !== response[i].AirlineId) {
                    isAdded = true;
                    curAirlineID = response[i].AirlineId;

                    airlineCode = {
                        id: response[i].AirlineId,
                        code: response[i].AirlineCode,
                        name: response[i].AirlineName,
                        destinations: [{
                                destId: response[i].DestinationId,
                                destName: response[i].DestinationName
                            }]
                    };

                    airlineVsProps.push(airlineCode);
                }
                if (!isAdded)
                {
                    airlineCode.destinations.push(
						{
							destId: response[i].DestinationId,
							destName: response[i].DestinationName
						}
                    );
                }
            }

            for (var j = 0; j < airlineVsProps.length; j++) {
                airlineString += '<option value=' + airlineVsProps[j].id + '>' + airlineVsProps[j].name + '</option>';
                for (var k = 0; k < airlineVsProps[j].destinations.length; k++) {
                    firstAirlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
                    $('#destination1').append(firstAirlineDestinatons);
                }
            }
            $('#airlinesDD1').append(airlineString);
            firstAirlineCode = airlineVsProps[0].code;
            $('#code1').val(firstAirlineCode);
        }
    });


    var languageList = getLanguageOptions();
    $('#language1_1').append(languageString);
}

function callChangeInterviewer() {

    var interviewerURL = "./cache/InterviewerList/InterviewerList_" + $('#AirportList').val() + ".json";
    $.getJSON(interviewerURL, function(response, status) {
        if (status == "success") {
            var optionString = '';
            for (var i = 0; i < response.length; i++) {
                optionString += '<option value=' + response[i].InterviewerId + '>' + response[i].InterviewerName + '</option>'
            }
            $('#InterviewerList').empty().append(optionString);
        }
    });

    callAirlineChange();
}

var table = $('#smpl_tbl').DataTable({
    "bPaginate": false,
    "bFilter": false,
    "bInfo": false
});

function callOnAirlinesChange(id) {
    var airlineId = parseInt(document.getElementById("airlinesDD" + id).value);
    for (var j = 0; j < airlineVsProps.length; j++) {
        if (airlineId === airlineVsProps[j].id) {
            $('#code' + id).val(airlineVsProps[j].code);
            var airlineDestinatons = '';
            for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
                airlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
            $('#destination' + id).empty().append(airlineDestinatons);
            break;
        }
    }
}

