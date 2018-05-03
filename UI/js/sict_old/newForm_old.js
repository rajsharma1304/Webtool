/*<Copyright> Cross-Tab </Copyright>
<ProjectName>SICT</ProjectName>
<FileName> newform.js </FileName>
<CreatedOn>15 Jan 2015</CreatedOn>*/
//var newFormMessages =
//{
//    ERROR_NO_AIRPORT_SELECT: "Please select an airport to continue",
//    ERROR_NO_INTRVWR_SELECT: "Please select an interviewer to continue",
//    ERROR_NO_FLDWRK_SELECT: "Please select date of form distribution",
//    ERROR_BUSINESS_CARDS_MISMATCH: 'No. of business cards for below airline exceed total card range',
//    ERROR_AIRLINES_OVERLAPPING: "Found conflicting serial numbers in following airlines",
//    ERROR_EXCEED_CARDLIMIT: "Cards distribution has exceeded the maximum configured limit",
//    SUCCESS_AIRLINE_UPDATE: "Successfully updated the airline",
//    SUCCESS_AIRLINE_SUBMIT: "Successfully added following airlines",
//    ERROR_GENERAL_MESSAGE: "Please re-check airlines before continuing",
//    ERROR_GENERAL_CONTROLS: "Please re-check airlines for highlighted controls",
//    GENERAL_PROCSING_REQUEST: 'Processing your request. Please wait..',
//    INFO_MAX_LANGUAGE_SUPPORT: "Maximum allowed languages per airline is 5"
//};

//[#73200] [Customer]:Change error message texts on New form entries
var newFormMessages =
{
    ERROR_NO_AIRPORT_SELECT: "Please select an airport to continue",
    ERROR_NO_INTRVWR_SELECT: "Please select an interviewer to continue",
    ERROR_NO_FLDWRK_SELECT: "Please select date of form distribution",
    //[#73676] [Customer]:All Instances – New entry forms – The error message for Bus. Cards exceeded should have a dot after ‘No’, which is short for number.
    ERROR_BUSINESS_CARDS_MISMATCH: 'No. of business cards exceed total card range',
    ERROR_AIRLINES_OVERLAPPING: "The serial numbers entered already exist",
    ERROR_EXCEED_CARDLIMIT: "Cards distribution has exceeded the maximum configured limit",
    SUCCESS_AIRLINE_UPDATE: "Successfully updated the airline",
    SUCCESS_AIRLINE_SUBMIT: "Successfully added following airlines",
    ERROR_GENERAL_MESSAGE: "The following airline entries have errors:",
    ERROR_GENERAL_CONTROLS: "Please re-check airlines for highlighted controls",
    GENERAL_PROCSING_REQUEST: 'Processing your request. Please wait..',
    INFO_MAX_LANGUAGE_SUPPORT: "Maximum allowed languages per airline is 5",

    ERROR_EMPTY_AIRLINE: 'Airline value is not selected',
    ERROR_EMPTY_FLIGHT: 'Flight no. is empty',
    ERROR_INVALID_FLIGHT: 'Flight no. is invalid',  //[#73343] [Customer]: EU + International instances � Entry forms- The Flight no. field is accepting more than 5 digits.
    ERROR_EMPTY_CARD: 'Card no. is empty',
    ERROR_EMPTY_BCARDS: 'Invalid number of business cards',
    ERROR_EMPTY_LANGUAGE: 'Language not selected',
    ERROR_EMPTY_SERIAL: 'Card serial numbers not provided',
    ERROR_EMPTY_DESTINATION: 'Please select the destination',
    ERROR_EMPTY_AIRCRAFT: 'Please select an aircraft type',
    ERROR_GREATER_SERIAL: 'The value of first serial no. is greater than last serial no.'
};

//[#72790] [Customer]:Editing entries through View form Entries :
var newFormUtils = {
    initialize: function (isAirport, isInterviewers, isDateTime) {
        if (isAirport) {
            var optionString = airportUtil.airportListOptions(), apSelect = $('#AirportList');
            apSelect.empty().append(optionString);
            //	Select if only one airport is present and make it readonly
            if (apSelect[0].length === 2) {
                apSelect[0].disabled = true;
                apSelect[0].selectedIndex = 1;
                cacheMgr.selectedAirport(apSelect.val());
            }
        }

        if (isInterviewers) {
            optionString = airportUtil.interviewerListOptions();
            $('#InterviewerList').append(optionString);
        }

        if (isDateTime) {
            var fdDate = $('#fieldworkDate');
            fdDate.datepicker({
                format: "mm/dd/yyyy",
                autoclose: true,
                orientation: "top left",
                todayHighlight: true,
                startDate: newformDeparture.fieldWorkRange.START,
                endDate: newformDeparture.fieldWorkRange.END
            });
        }
    },
    initializeAirports: function (ctrlName, id, shouldDisable) {
        var optionString = airportUtil.airportListOptions(), apSelect = $('#' + ctrlName);
        apSelect.empty().append(optionString);
        //	Select if only one airport is present and make it readonly
        if (apSelect[0].length === 2) {
            apSelect[0].selectedIndex = 1;
            cacheMgr.selectedAirport(apSelect.val());
        }
        if (undefined !== id) {
            var sOpt = apSelect[0].options;
            for (var i = 0; i < sOpt.length; i++) {
                if (sOpt[i].value === id.toString()) {
                    sOpt[i].selected = true;
                    cacheMgr.selectedAirport(apSelect.val());
                    break;
                }
            }
        }
        apSelect[0].disabled = shouldDisable;
    },
    initializeDateControls: function (ctrlName, newDate) {
        var fdDate = $('#' + ctrlName);
        fdDate.datepicker({
            format: "mm/dd/yyyy",
            autoclose: true,
            orientation: "top left",
            todayHighlight: true,
            startDate: newformDeparture.fieldWorkRange.START,
            endDate: newformDeparture.fieldWorkRange.END
        });
        newDate = (undefined === newDate) ? new Date() : newDate;
        fdDate.datepicker().data('date', moment(newDate).format('MM/DD/YYYY'));
        fdDate.val(moment(newDate).format('MM/DD/YYYY'));
        fdDate.datepicker('update');
        fdDate.focusout(function () {
            var input = $(this).val();
            var currentDate = new Date(input);
            var todaysDate = new Date();
            var validformat = /^\d{2}\/\d{2}\/\d{4}$/ //Basic check for format validity
            var isValid = validformat.test(input);
            //[#73529] General - Fieldwork date is accepting the future dates
            if (currentDate > todaysDate || '' === input || isNaN(currentDate.getTime()) || !isValid) {
                $(this).val((todaysDate.getMonth() + 1) + '/' + todaysDate.getDate() + '/' + todaysDate.getFullYear());
                fdDate.datepicker('update');
            }
        });
    },
    initializeIntervieweControls: function (ctrlName, id) {
        var optionString = airportUtil.interviewerListOptions();
        $('#' + ctrlName).append(optionString);

        var sOpt = $('#' + ctrlName)[0].options;
        for (var i = 0; i < sOpt.length; i++) {
            if (sOpt[i].value === id.toString()) {
                sOpt[i].selected = true;
                break;
            }
        }
    }
};

var newformDeparture = {
    maxCardCountPerAirline: 100,
    airlineCounter: 1,
    scrolled: 0,
    lang_data: [],
    submitted_airlines: [],
    errored_airlines: [],
    isFormSubmittable: false,
    fieldWorkRange: {
        START: '-3650d', //	Allow for past 10 years
        END: '+0d'			// 	Dont allow for future dates
    },
    depFormData: {},
    postSUBMISSION: [],
    initializeDepartureForm: function () {
        var me = this;
        //[#73529] General - Fieldwork date is accepting the future dates
        newFormUtils.initializeDateControls('fieldworkDate');
        //Binding event for adding new Airlines.
        $('body').on("click", "#addAirlines", function () {
            newformDeparture.addnewAirlines();
        });

        $("input").focusin(function () {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('.sict_error');
        });
        initialiseStatics();
        me.checkValueisNumber("");
        var isAIR = (getInstance() === "AIR") ? true : false;
        if (isAIR)
            me.addAircraftTypeControl();
    },
    addAircraftTypeControl: function () {
            aircraftTypeString = airportUtil.aircraftTypeListOptions();
            var aircrafttype = '<div class="span12">' +
                    '<div class="span6">' +
                    '<label>Aircraft Type</label>' +
                    '<select class="span10" id="aircrafttype{0}" name="aircrafttype{0}">' +
                    aircraftTypeString +
                    '</select>' +
                    '</div>' +
                    '</div>';
            $(".airlinesChildContainer .span6:eq(0)").append(aircrafttype);
    },
    handleFocusIn: function () {
        $("input").focusin(function () {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('.sict_error');
        });
        $("select").focusin(function () {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('sict_error');
        });
    },
    checkValueisNumber: function (elem) {
        if (elem != "") {
            var inputValue = elem[0].value.replace(/\s+/g, ""), elemId = elem[0].id, parent = elem.parent();
            if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > -1) {
                    if (/^flight/g.test(elemId)) {
                        if (inputValue.length > 5) {
                            parent.addClass('sict_error');
                            return true;
                        }
                        else {
                            parent.removeClass('sict_error');
                            return false;
                        }
                    }
                    elem.parent('div').removeClass('sict_error');
                    return false;
                }
            }
            //	Add the error class to inform it is invalid
            elem.parent('div').addClass('sict_error');
            return true;
        }

        $("input[type=text]").focusout(function (e) {
            var me = this, elemId = e.target.id, inputValue = me.value.replace(/\s+/g, ""),
                    parent = $(me).parent();
            if (elemId === "fieldworkDate") {
                return false;
            }
            if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > -1) {
                    //[#73623] [Customer]:One additional remark regarding the Flight numbers.
                    if (/^flight/g.test(elemId)) {
                        var inputIntValue = parseInt(inputValue);
                        if (e.target.value.length <= 2 && e.target.value.length !== 0 && inputIntValue !== 0) {
                            me.value = (inputIntValue < 100) ? ((inputIntValue >= 10) ? ("0" + inputIntValue) : ("00" + inputIntValue)) : inputIntValue;
                            inputValue = me.value;
                        }
                        else if (e.target.value.length === 4 && inputIntValue !== 0 && e.target.value[0] === '0') {
                            me.value = (inputIntValue < 1000) ?
                                ((inputIntValue < 100) ?
                                ((inputIntValue >= 10) ? ("0" + inputIntValue) : ("00" + inputIntValue))
                                : inputIntValue)
                                : inputIntValue;
                            inputValue = me.value;
                        }
                        else if (inputIntValue === 0) {
                            me.value = inputValue = "000";
                        }
                        if (inputValue.length > 5)
                            parent.addClass('sict_error');
                        else
                            parent.removeClass('sict_error');
                        return false;
                    }
                    parent.removeClass('sict_error');
                    return;
                }
                //[#72446] Departure/Arrival form->Flight no. is accepting alphabets and special characters also.
                var target = $(e.target);
                var targetString = $(e.target).val();
                if (targetString.length > 0) {
                    for (var i = 0; i < targetString.length; i++) {
                        var charCode = targetString.charCodeAt(i);
                        //To check if the string has alphabets.
                        if ((charCode > 32 && charCode < 47) || (charCode > 58 && charCode < 122)) {
                            target.parent().addClass('sict_error');
                            $(e.target).val('');
                            return;
                        } else {
                            target.parent().removeClass('sict_error');
                        }
                    }
                } else {
                    target.parent().addClass('sict_error');
                }
            }
            
            var val = parseInt(e.target.value)
                if (val >= 9223372036854776000)
                   target.parent().addClass('sict_error');
            
            //	Add the error class to inform it is invalid
            parent.addClass('sict_error');
        });

        $('input[type=text]').keydown(function (e) {
            //[#73150] Departure form:-accepting alphabets
            var keyCodes = [46, 8, 9, 27, 13];
            if (e.target.id === "fieldworkDate")
                keyCodes.push(191);	// Support the '/' separator
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, keyCodes) !== -1 ||
                //[#72764] [Customer]:Departure/Arrival forms- Cannot use keyboard shortcuts like Ctrl V and Ctrl C in any of the text boxes present on the entry forms.
                // Allow: Ctrl+A, Ctrl+C, Ctrl+V
                    ((e.keyCode == 65 || e.keyCode == 67 || e.keyCode == 86 ||
                //Ctrl+a, Ctrl+c, Ctrl+v
                    e.keyCode == 97 || e.keyCode == 99 || e.keyCode == 118 ||
                //Ctrl + Z, Ctrl + z
                    e.keyCode == 90 || e.keyCode == 122) &&
                    e.ctrlKey === true) ||
                // Allow: home, end, left, right
                    (e.keyCode >= 35 && e.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
            else {
                if (e.target.id.indexOf('flight') !== -1) {
                    if (e.target.value.length > 3) {
                        e.preventDefault();
                    }
                }
            }
        });

        $("select").focusout(function (e) {
            var target = $(e.target);
            if ("-1" === target.val())
                target.parent().addClass('sict_error');
            else
                target.parent().removeClass('sict_error');
        });
    },
    fillNumbers: function (start, end) {
        //it must return an array.
        var sValue = parseInt(start.val(), 10), lValue = parseInt(end.val(), 10), temp;
        if (start && end) {
            var finalValues = [];
            if (sValue === lValue) {
                finalValues.push(sValue);
            }
            else if (sValue > lValue) {
                for (var i = lValue; i <= sValue; i++) {
                    finalValues.push(i);
                }
            }
            else if (sValue < lValue) {
                for (var i = sValue; i <= lValue; i++) {
                    finalValues.push(i);
                }
            }
        }
        return finalValues;
    },
    intersectSafe: function (a, b) {
        var ai = 0, bi = 0;
        var result = new Array();
        while (ai < a.length && bi < b.length) {
            if (a[ai] < b[bi]) {
                ai++;
            }
            else if (a[ai] > b[bi]) {
                bi++;
            }
            else /* they're equal */ {
                result.push(a[ai]);
                ai++;
                bi++;
            }
        }
        return result;
    },
    triggerError: function (resp) {
        var me_ = this;
        if (resp.ReturnCode > 0) {
        }
        else
            pageHelper.notify(resp.ReturnMessage, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);
    },
    triggerSuccess: function (resp, callback) {
        var me_ = this;

        pageHelper.removeSmokeSignal();

        if (null !== callback && undefined !== callback) {
            if (resp.AirlineDetails && resp.AirlineDetails.length === 1) {
                if (resp.ReturnCode > 0 && resp.AirlineDetails[0].InvalidLanguages === null &&
                        resp.AirlineDetails[0].IsSerialNoValid && resp.AirlineDetails[0].IsSuccess) {
                    return callback(newformDeparture.depFormData);
                }
            }
        }

        if (resp.ReturnCode > 0) {
            if (resp.AirlineDetails && resp.AirlineDetails.length > 0) {
                var inValidAirlineLang = [], airline, langSection, langRow, airlineError = [], highlightedAirline = [], iAL = [], sALNames = [];
                $.each(resp.AirlineDetails, function (i, airline) {
                    if (airline.IsSerialNoValid && airline.IsSuccess) {
                        inValidAirlineLang.push({});
                        iAL.push(i);
                    }
                    else {
                        if (airline.InvalidLanguages === null)
                            return false;
                        inValidAirlineLang.push({ airline: airline.AirlineId, invalidLang: airline.InvalidLanguages })
                        airlineError.push(i + 1);
                    }
                });
                $.each(iAL, function (i, el) {
                    var sALCntr = $(".airlinesChildContainer:eq(" + me_.submitted_airlines[el] + ")");
                    sALNames.push(sALCntr.find("h4").text());
                    delete me_.submitted_airlines[el];
                });

                $.each(inValidAirlineLang, function (i, obj) {
                    if (!$.isEmptyObject(obj)) {
                        airline = $(".airlinesChildContainer:eq(" + me_.submitted_airlines[i] + ")");
                        airline.addClass('sict_row_error');
                        langSection = airline.find("div[id^=languageAirline]");
                        $.each(obj.invalidLang, function (i, val) {
                            langRow = langSection.find(".row-fluid:eq(" + (parseInt(val, 10) - 1) + ")");
                            langRow.find("input[id^=fserial]").parent().addClass("sict_error");
                            langRow.find("input[id^=lserial]").parent().addClass("sict_error");
                        });
                    }//has empty Object which has to removed for Success.
                });
                //Appending Airlines Template
                //1)Atleast One Element is there ?
                $(".airlinesChildContainer:not(.sict_row_error)").hide('slow', function () {
                    $(".airlinesChildContainer:not(.sict_row_error)").remove();
                    if ($(".airlinesChildContainer").length === 0) {
                        newformDeparture.addnewAirlines();
                    }
                    if ($(".airlinesChildContainer:eq(0) .airlineclose").length > 0) {
                        $(".airlinesChildContainer:eq(0) .airlineclose").remove();
                    }
                });
                if (airlineError.length > 0) {
                    $(".airlinesChildContainer.sict_row_error").each(function (i, ele) {
                        highlightedAirline.push(ele.querySelector("h4").innerHTML.trim());
                    });
                    pageHelper.notify(newFormMessages.ERROR_AIRLINES_OVERLAPPING + '<br>' + highlightedAirline.join("<br>"), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
                }

                if (sALNames.length > 0)
                    pageHelper.notify(newFormMessages.SUCCESS_AIRLINE_SUBMIT + '<br>' + sALNames.join("<br>"), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
            }
            else {
                me_.postSUBMISSION = [];
                pageHelper.notify(resp.ReturnMessage, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            }
        }//For Processing Invalid Session or Other DB Errors.
        else
            pageHelper.notify(resp.ReturnMessage, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);
    },
    checkInNewfDep: function () {
        return isDeparture;
    },
    submitFormDeparture: function (origin) {
        var origin = origin, me_ = this, isnewfDep = me_.checkInNewfDep(), parentEle = $("#airlinesContainer"), prevEle = parentEle.prev();
        if (prevEle.hasClass("alert"))
            prevEle.remove();
        pageHelper.clearStickies();
        me_.submitted_airlines = [], me_.errored_airlines = [];
        $(".sict_error").removeClass('sict_error');
        $(".sict_row_error").removeClass('sict_row_error');
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
                bCardsAirlineMatch = [],
                maxCardDistribution = [],
                depObj = { AirportId: "", FieldWorkDate: "", InterviewerId: "", Airlines: [] };

        var msgArray = [], canContinue = true;
        if (airport === "-1") {
            msgArray.push(newFormMessages.ERROR_NO_AIRPORT_SELECT);
            $("#AirportList").parent().addClass("sict_error");
            canContinue = false;
        }
        else {
            $("#AirportList").parent().removeClass("sict_error");
            canContinue = !canContinue ? canContinue : true;
        }
        if (interviewer === "-1") {
            msgArray.push(newFormMessages.ERROR_NO_INTRVWR_SELECT);
            $("#InterviewerList").parent().addClass("sict_error");
            canContinue = false;
        } else {
            $("#InterviewerList").parent().removeClass("sict_error");
            canContinue = !canContinue ? canContinue : true;
        }
        if (fieldworkDate.val() === "") {
            msgArray.push(newFormMessages.ERROR_NO_FLDWRK_SELECT);
            fieldworkDate.parent().addClass("sict_error");
            canContinue = false;
        } else {
            fieldworkDate.parent().removeClass("sict_error");
            canContinue = !canContinue ? canContinue : true;
        }
        //	No need of further validation
        if (!canContinue) {
            pageHelper.notify(msgArray.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            return;
        }

        if (isnewfDep)
            depObj.IsDepartureForm = "true";
        else
            depObj.IsDepartureForm = "false";

        depObj.AirportId = airport, depObj.FieldWorkDate = fieldworkDate.val(), depObj.InterviewerId = interviewer;

        airlineChild.each(function (i, ele) {
            //Processing each Row
            var row = $(this), tempObj = {}, tempLang = [], airlineId, flightNo, destinationId, businessCards, code = "", aircrafttype = '-1',
                    languageRow = row.find("div[id^=languageAirline] .row-fluid"), isFSEmpty = false, isLSEmpty = false, langValidation = [];
            airlineId = row.find(".airlinesdrop option:selected").val();
            airlineIdLabel = row.find(".airlinesdrop").prev("label");
            flightNo = row.find("input[id^=flight]");
            flightNoLabel = row.find("input[id^=flight]").prev("label");
            destinationId = row.find("select[id^=destination] option:selected").val();
            destinationIdLabel = row.find("select[id^=destination]").prev("label");
            businessCards = row.find("input[id^=businesscard]");
            businessCardsLabel = row.find("input[id^=businesscard]").prev("label");
            code = row.find("input[id^=code]").val();
            codeLabel = row.find("input[id^=code]").prev("label");
            aircrafttype = row.find("select[id^=aircrafttype] option:selected").val() === undefined ? '-1' : row.find("select[id^=aircrafttype] option:selected").val();
            langODetails = [];
            //	Ignore the airlines which doesnt have the airline selected
            var canContinue = true;
            if ("-1" === airlineId) {
                canContinue = false;
            }
            if ("-1" === destinationId) {
                canContinue = false;
            }
            if (row.find("select[id^=aircrafttype]").length > 0) {
                if ("-1" === aircrafttype) {
                    canContinue = false;
                }
            }
            if (!canContinue) {
                me_.errored_airlines.push(i);
                return true;
            }

            if (!me_.checkValueisNumber(flightNo))
                isFlightNoEmpty = false;
            else
                isFlightNoEmpty = true;
            if (!me_.checkValueisNumber(businessCards))
                isBusinessCardsEmpty = false;
            else
                isBusinessCardsEmpty = true;

            if (!isBusinessCardsEmpty && !isFlightNoEmpty)
                tempObj.AirlineId = airlineId, tempObj.FlightNumber = code + flightNo.val().toString(),
                        tempObj.DestinationId = destinationId, tempObj.BCardsDistributed = businessCards.val(),
                        tempObj.OriginId = row.find(".airlinesdrop option:selected").attr('originId'),
                        tempObj.Route = row.find(".airlinesdrop option:selected").attr('route'),
                        tempObj.FlightType = row.find(".airlinesdrop option:selected").attr('FlightType'),
                        tempObj.Direction = row.find(".airlinesdrop option:selected").attr('direction');
            tempObj.AircraftType = aircrafttype;

            delete tempObj["Code"];

            //Processing Language
            var totalLang = 0, isOK = true;
            $.each(languageRow, function (j, val) {
                var row = $(this), lang, fs, ls, tempLangObj = {}, langLabel, airlineLang = [];
                lang = row.find("option:selected").val(),
                        fs = row.find("input[id^=fserial]"),
                        ls = row.find("input[id^=lserial]"),
                        langLabel = row.find("select").prev("label");
                if ('-1' === lang)
                    row.find('select').parent().addClass("sict_error");
                else
                    row.find('select').parent().removeClass("sict_error");

                if (!me_.checkValueisNumber(fs))
                    isFSEmpty = false;

                if (!me_.checkValueisNumber(ls))
                    isLSEmpty = false;

                if (!isFSEmpty && !isLSEmpty) {
                    var fsVal = parseInt(fs.val(), 10), lsVal = parseInt(ls.val(), 10);
                    if (fsVal > lsVal) {
                        fs.parent().addClass('sict_error');
                        ls.parent().addClass('sict_error');
                    }

                    airlineLang = me_.fillNumbers(fs, ls);
                    totalLang = totalLang + airlineLang.length;
                    langValidation.push(airlineLang);

                    if (fsVal > lsVal) {
                        var t = fsVal;
                        fs = lsVal;
                        lsVal = t;
                    }
                    tempLangObj.LanguageId = lang, tempLangObj.FirstSerialNo = fsVal, tempLangObj.LastSerialNo = lsVal, me_.isFormSubmittable = true;
                    tempLang.push(tempLangObj);
                }
            });
            if (totalLang < parseInt(businessCards.val(), 10)) {
                bCardsAirlineMatch.push($('#airlineHeader' + (i + 1)).text());
                isOK = false;
            }
            if (me_.maxCardCountPerAirline < totalLang) {
                maxCardDistribution.push($('#airlineHeader' + (i + 1)).text());
                isOK = false;
            }
            if (isOK) {
                airlineLang.push(langValidation);
                tempObj.Languages = tempLang;
                depObj.Airlines.push(tempObj);
                me_.submitted_airlines.push(i);
            }
        });
       
        //Validating Business Cards ranges
        if (bCardsAirlineMatch.length > 0) {
            pageHelper.notify(newFormMessages.ERROR_BUSINESS_CARDS_MISMATCH + '<br>' + bCardsAirlineMatch.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            //me_.isFormSubmittable = false;
        }
        //Validating for maximum cards distributed per airline
        if (maxCardDistribution.length > 0) {
            pageHelper.notify(newFormMessages.ERROR_EXCEED_CARDLIMIT + '<br>' + maxCardDistribution.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            //me_.isFormSubmittable = false;
        }
        //Validating Languages
        var olAirlines = [];
        if (me_.isFormSubmittable && !isBusinessCardsEmpty && !isFlightNoEmpty && !isFSEmpty && !isLSEmpty) {
            olAirlines = me_.validateLanguages(airlineLang);
            if (olAirlines.length > 0)
                pageHelper.notify(newFormMessages.ERROR_AIRLINES_OVERLAPPING + '<br>' + olAirlines.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }

        if ($(".sict_error").length > 0 || 0 === depObj.Airlines.length) {
            me_.isFormSubmittable = false;
        }
        else {
            me_.isFormSubmittable = true;
            me_.depFormData = depObj;
        }

        if (!me_.isFormSubmittable) {
            var airlineNames = [];
            $.each(me_.errored_airlines, function (i, obj) {
                airline = $(".airlinesChildContainer:eq(" + obj + ")");
                airline.addClass('sict_row_error');
                airlineNames.push(airline.find("h4").text());
            });
            if (airlineNames.length > 0)
                pageHelper.notify(newFormMessages.ERROR_GENERAL_MESSAGE + '<br>' + airlineNames.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            //  Get All airlines which are erroring out
            airlineNames = [];
            $.each($(".sict_error"), function (i, obj) {
                var eleText = $(obj).closest('.formSep').find('h4').text();
                if (-1 === airlineNames.indexOf(eleText))
                    airlineNames.push(eleText);
            });
            //Ignore the business, overlapping and exceeding limit airlines
            var index = -1;
            $.each(bCardsAirlineMatch, function (i, obj) {
                airline = $(".airlinesChildContainer:eq(" + obj + ")");
                airline.addClass('sict_row_error');
                index = airlineNames.indexOf(obj);
                airlineNames = -1 !== index ? airlineNames.splice(index + 1, 1) : airlineNames;
            });
            $.each(maxCardDistribution, function (i, obj) {
                airline = $(".airlinesChildContainer:eq(" + obj + ")");
                airline.addClass('sict_row_error');
                index = airlineNames.indexOf(obj);
                airlineNames = -1 !== index ? airlineNames.splice(index + 1, 1) : airlineNames;
            });
            $.each(olAirlines, function (i, obj) {
                index = airlineNames.indexOf(obj);
                airlineNames = -1 !== index ? airlineNames.splice(index + 1, 1) : airlineNames;
            });
            if (airlineNames.length > 0)
                pageHelper.notify(newFormMessages.ERROR_GENERAL_CONTROLS + '<br>' + airlineNames.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }
        if (me_.isFormSubmittable) {
            //  Highlight the business and max limit card airlines
            $.each(bCardsAirlineMatch, function (i, obj) {
                airline = $(".airlinesChildContainer:eq(" + obj + ")");
                airline.addClass('sict_row_error');
            });
            $.each(maxCardDistribution, function (i, obj) {
                airline = $(".airlinesChildContainer:eq(" + obj + ")");
                airline.addClass('sict_row_error');
            });
            if ($(".sict_error").length > 0)
                return false;
            else {
                $("#Submit").button('loading');
                pageHelper.addSmokeSignal(newFormMessages.GENERAL_PROCSING_REQUEST);
                sendDepartureForm();
            }
        }
    },
    submitEditDeparture: function (origin, sEle, callback) {
        var origin = origin, me_ = this, isnewfDep = me_.checkInNewfDep(), parentEle = $("#editAirlineBody"), prevEle = parentEle.prev();
        if (prevEle.hasClass("alert"))
            prevEle.remove();
        pageHelper.clearStickies();
        me_.submitted_airlines = [];
        $(".sict_error").removeClass('sict_error');
        $(".sict_row_error").removeClass('sict_row_error');

        var depformp = $("#editAirlineBody"),
                depformc = depformp.children(),
                depformf = depformc.first(),
                airport = sessionStorage.selectedAirportId,
                interviewerLabel = sEle.Interviewer,
                interviewer = sEle.InterviewerId,
                fieldworkDate = $('#fieldworkDate input'),
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
                bCardsAirlineMatch = [],
                maxCardDistribution = [],
                depObj = { AirportId: "", FieldWorkDate: "", InterviewerId: "", FormId: sEle.FormId, Airlines: [] };

        var msgArray = [], canContinue = true;

        if (isnewfDep)
            depObj.IsDepartureForm = "true";
        else
            depObj.IsDepartureForm = "false";

        var canContinue = true;

        if (fieldworkDate.val() === "") {
            pageHelper.notify(newFormMessages.ERROR_NO_FLDWRK_SELECT, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            fieldworkDate.parent().addClass("sict_error");
            return;
        }

        depObj.AirportId = airport, depObj.FieldWorkDate = fieldworkDate.val(), depObj.InterviewerId = interviewer;
        airlineChild.each(function (i, ele) {
            //Processing each Row
            var row = $(this), tempObj = {}, tempLang = [], airlineId, flightNo, destinationId, businessCards, code = "", aircrafttype = '-1',
                    languageRow = row.find("div[id^=languageAirline] .row-fluid"), isFSEmpty = false, isLSEmpty = false, langValidation = [];

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
                    aircrafttype = row.find("select[id^=aircrafttype] option:selected").val() === undefined ? '-1' : row.find("select[id^=aircrafttype] option:selected").val();
            langODetails = [];
            //	Ignore the airlines which doesnt have the airline selected
            row.find(".airlinesdrop").parent().removeClass("sict_error");
            row.find("select[id^=destination]").parent().removeClass("sict_error");
            var canContinue = true;
            if ("-1" === airlineId) {
                row.find(".airlinesdrop").parent().addClass("sict_error");
                canContinue = false;
            }

            if ("-1" === destinationId) {
                row.find("select[id^=destination]").parent().addClass("sict_error");
                canContinue = false;
            }
            if (row.find("select[id^=aircrafttype]").length > 0) {
                if ("-1" === aircrafttype) {
                    row.find("select[id^=aircrafttype]").parent().addClass("sict_error");
                    canContinue = false;
                }
            }
            if (!canContinue)
                return false;

            if (!me_.checkValueisNumber(flightNo))
                isFlightNoEmpty = false;
            else
                isFlightNoEmpty = true;

            if (!me_.checkValueisNumber(businessCards))
                isBusinessCardsEmpty = false;
            else
                isBusinessCardsEmpty = true;

            if (!isBusinessCardsEmpty && !isFlightNoEmpty)
                tempObj.AirlineId = airlineId,
                        tempObj.FlightNumber = code + flightNo.val().toString(),
                        tempObj.DestinationId = destinationId,
                        tempObj.BCardsDistributed = businessCards.val(),
                        tempObj.OriginId = row.find(".airlinesdrop option:selected").attr('OriginId'),
                        tempObj.Route = row.find(".airlinesdrop option:selected").attr('Route'),
                        tempObj.FlightType = row.find(".airlinesdrop option:selected").attr('FlightType'),
                        tempObj.Direction = row.find(".airlinesdrop option:selected").attr('Direction'); //, me_.isFormSubmittable = true;//, tempObj.Code = code;
            tempObj.AircraftType = aircrafttype;

            //if (tempObj.Code === "")//Deleting the Code key temporarily
            delete tempObj["Code"];

            //Processing Language
            var totalLang = 0, isOK = true;
            $.each(languageRow, function (j, val) {
                var row = $(this), lang, fs, ls, tempLangObj = {}, langLabel, airlineLang = [];
                lang = row.find("option:selected").val(),
                        fs = row.find("input[id^=fserial]"),
                        ls = row.find("input[id^=lserial]"),
                        langLabel = row.find("select").prev("label");
                if ('-1' === lang)
                    row.find('select').parent().addClass("sict_error");
                else
                    row.find('select').parent().removeClass("sict_error");

                if (!me_.checkValueisNumber(fs)) {
                    isFSEmpty = false;
                }
                if (!me_.checkValueisNumber(ls)) {
                    isLSEmpty = false;
                }

                if (!isFSEmpty && !isLSEmpty) {
                    var fsVal = parseInt(fs.val(), 10), lsVal = parseInt(ls.val(), 10);
                    if (fsVal > lsVal) {
                        fs.parent().addClass('sict_error');
                        ls.parent().addClass('sict_error');
                    }
                    airlineLang = me_.fillNumbers(fs, ls);
                    totalLang = totalLang + airlineLang.length;
                    langValidation.push(airlineLang);
                    if (fsVal > lsVal) {
                        var t = fsVal;
                        fs = lsVal;
                        lsVal = t;
                    }
                    tempLangObj.LanguageId = lang, tempLangObj.FirstSerialNo = fsVal, tempLangObj.LastSerialNo = lsVal, me_.isFormSubmittable = true;
                    tempLang.push(tempLangObj);
                }
            });
            if (totalLang < parseInt(businessCards.val(), 10)) {
                isOK = false;
                bCardsAirlineMatch.push($('#airlineHeader' + (i + 1)).text());
            }
            if (me_.maxCardCountPerAirline < totalLang) {
                isOK = false;
                maxCardDistribution.push($('#airlineHeader' + (i + 1)).text());
            }
            if (isOK) {
                airlineLang.push(langValidation);
                tempObj.Languages = tempLang;
                depObj.Airlines.push(tempObj);
                me_.submitted_airlines.push(i);
            }
        });

        //Validating Business Cards ranges
        if (bCardsAirlineMatch.length > 0) {
            pageHelper.notify(newFormMessages.ERROR_BUSINESS_CARDS_MISMATCH + '<br>' + bCardsAirlineMatch.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            row.find("input[id^=businesscard]").addClass(".sict_error");
            me_.isFormSubmittable = false;
        }
        //Validating for maximum cards distributed per airline
        if (maxCardDistribution.length > 0) {
            pageHelper.notify(newFormMessages.ERROR_EXCEED_CARDLIMIT + '<br>' + maxCardDistribution.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            row.find("input[id^=businesscard]").addClass(".sict_error");
            me_.isFormSubmittable = false;
        }

        //Validating Languages
        var olAirlines = [];
        if (me_.isFormSubmittable && !isBusinessCardsEmpty && !isFlightNoEmpty && !isFSEmpty && !isLSEmpty) {
            olAirlines = me_.validateLanguages(airlineLang);
            if (olAirlines.length > 0) {
                pageHelper.notify(newFormMessages.ERROR_AIRLINES_OVERLAPPING + '<br>' + olAirlines.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
                return false;
            }
        }

        if ($(".sict_error").length > 0 || 0 === depObj.Airlines.length) {
            me_.isFormSubmittable = false;
        }
        else {
            me_.isFormSubmittable = true;
            me_.depFormData = depObj;
        }
        if (!me_.isFormSubmittable)
            pageHelper.notify(newFormMessages.ERROR_GENERAL_CONTROLS, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);

        if (me_.isFormSubmittable) {
            if ($(".sict_error").length > 0)
                return false;
            else {
                pageHelper.addSmokeSignal(newFormMessages.GENERAL_PROCSING_REQUEST);
                sendDepartureForm(callback, true);
            }
        }
    },
    validateLanguages: function (lArray) {
        var olAirlines = [];
        var me = this,
                airline,
                langRow;
        //intersectSafe;
        if (lArray && lArray.length > 0) {
            var arrVal, scndArrVal;
            //  Check with in the airline
            if (lArray.length > 0) {
                arrVal = lArray[0];
                //Intersect needs atleast 2 arrays to compare.
                if (arrVal && arrVal.length > 1) {
                    for (j = 0; j < arrVal.length; ++j) {
                        for (k = j + 1; k < arrVal.length; ++k) {
                            if ((me.intersectSafe(arrVal[j], arrVal[k])).length > 0) {
                                airline = $(".airlinesChildContainer:eq(" + 0 + ")");
                                var hdText = airline.find('div h4').text();
                                if (-1 === olAirlines.indexOf(hdText))
                                    olAirlines.push(hdText);
                                langRow = airline.find("div[id^=languageAirline] .row-fluid:eq(" + k + ")");
                                langRow.find('input[id^=fserial]').parent('div').addClass('sict_error');
                                langRow.find('input[id^=lserial]').parent('div').addClass('sict_error');
                            }
                        }
                    }
                }
            }
            //  Check across airlines
            for (var i = 0; i < lArray.length; i++) {
                arrVal = lArray[i];
                for (j = i + 1; j < lArray.length; ++j) {
                    scndArrVal = lArray[j];
                    for (fk = 0; fk < arrVal.length; ++fk) {
                        for (sk = 0; sk < scndArrVal.length; ++sk) {
                            if ((me.intersectSafe(arrVal[fk], scndArrVal[sk])).length > 0) {
                                airline = $(".airlinesChildContainer:eq(" + this.submitted_airlines[j] + ")");
                                var hdText = airline.find('div h4').text();
                                if (-1 === olAirlines.indexOf(hdText))
                                    olAirlines.push(hdText);
                                langRow = airline.find("div[id^=languageAirline] .row-fluid:eq(" + sk + ")");
                                langRow.find('input[id^=fserial]').parent('div').addClass('sict_error');
                                langRow.find('input[id^=lserial]').parent('div').addClass('sict_error');
                            }
                        }
                    }
                }
            }
        }
        return olAirlines;
    },
    newAirline: function (scBt) {
        var scBtStyle = scBt === undefined ? '' : 'style="display:none"';
        var scBtFStyle = scBt === undefined ? 'formSep' : '';
        var dynamicType = isDeparture ? 'Destination' : 'Origin',
                aircrafttype = '<div class="span12">' +
                '<div class="span6">' +
                '<label>Aircraft Type</label>' +
                '<select class="span10" id="aircrafttype{0}" name="aircrafttype{0}">' +
                aircraftTypeString +
                '</select>' +
                '</div>' +
                '</div>',
                aircraft = (getInstance() === "AIR") ? aircrafttype : '';

        var nAir = '<div class="airlinesChildContainer row-fluid">' +
                '<div class="span12 ' + scBtFStyle + '">' +
                '<i class="close airlineclose ttip_l" title="Discard Flight" onclick="newformDeparture.removeAirline(this);"' + scBtStyle + ' >&times;</i>' +
                '<div class="row-fluid">' +
                '<div class="span6">' +
                '<h4 id="airlineHeader{0}" class="sepH_c"> Flight {0} </h4>' +
                '<div class="span6">' +
                '<label>Airline </label>' +
                '<select id="airlinesDD{0}" name="airlinesDD{0}" class="span10 airlinesdrop" onchange="callOnAirlinesChange({0})">' +
                airlineString +
                '</select>' +
                '</div>' +
                '<div class="span2" style="margin-left:23px;">' +
                '<label>Code</label>' +
                '<input id="code{0}" name="code{0}" disabled="disabled" type="text" class="span8" value="">' +
                '</div>' +
                '<div class="span3">' +
                '<label>Flight No. </label>' +
                '<input id="flight{0}" name="flight{0}" type="text" class="span10">' +
                '</div>' +
                '<div class="span12">' +
                '<div class="span6">' +
                '<label>' + dynamicType + '</label>' +
                '<select class="span10 destdrop" id="destination{0}" name="destination{0}" >' +
                firstAirlineDestinatons +
                '</select>' +
                '</div>' +
                '<div class="span3" style="margin-left:23px;">' +
                '<label>Business Cards</label>' +
                '<input type="text" class="span5" id="businesscard{0}" name="businesscard{0}" value="0">' +
                '</div>' +
                '</div>' +
                aircraft +
                '</div>' +
                '<div id="{0}" class="span6">' +
                '<button type="button" class="btn sepH_b btn-pink addLanguage ttip_l"  class="ttip_l" title="Add New Language"data-langcounter="1" onclick="newformDeparture.addLanguage(this);"> Add Language </button>' +
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
    newAirport: function () {
        return '<div class="row-fluid">' +
                                '<div class="span12 formSep">' +
                                    '<div id="airportContainer" class="span3" style="margin-left: 15px;">' +
                                        '<label>Airport </label>' +
                                        '<select class="span11" id="EditedAirportList" name="EditedAirportList" onchange="callChangeInterviewer()">' +
                                        '</select>' +
                                    '</div>' +
                                    '<div id="interviewerContainer" class="span3">' +
                                        '<label>Interviewer Name </label>' +
                                        '<select class="span11" id="InterviewerList" name="InterviewerList"></select>' +
                                    '</div>' +
                                    '<div class="span2" style="margin-left: 16px;" id="fieldworkDateDiv" data-date-format="MM/DD/YYYY">' +
                                        '<label>Fieldwork Date</label>' +
                                        '<input type="text" id="fieldworkDate" name="fieldworkdate" class="span12" />' +
                                    '</div>' +
                                '</div>' +
                            '</div>';
    },
    addLanguage: function (e) {
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
            me.siblings().append(languageRowString).slideDown('slow');
            var lserial = $('input[id^=lserial' + parentId + '_' + (langcounterval + 1) + ']');
            if (count > 0)
                lserial.after('<i class="close langclose ttip_t" title="Discard Language" onclick="newformDeparture.removeLangauage(this);">&times;</i>');

            if (count === 4) {
                pageHelper.notify(newFormMessages.INFO_MAX_LANGUAGE_SUPPORT, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);
                me.prop("disabled", "disabled");
            }
            this.checkValueisNumber("");
        } else {
            me.prop("disabled", "disabled");
        }
        gebo_tips.init();
    },
    addnewAirlines: function () {
        var me = this;
        if ($(".airlinesChildContainer").length >= 9) {
            $("#addAirlines").attr("disabled", "disabled");
        }
        var template = jQuery.validator.format($.trim((me.newAirline())));
        $('#airlinesContainer').append($(template(++me.airlineCounter))).show('slow');
        setTimeout(function () {
            me.scrollDown();
        }, 500);
        me.checkValueisNumber("");
        gebo_tips.init();
    },
    addvalidationRules: function (ct) {
        $('input[id^=flight' + ct + ']').rules("add", { required: true, number: true, min: 0, messages: { required: "" } });
        $('input[id^=businesscard' + ct + ']').rules("add", { required: true, number: true, min: 0, messages: { required: "" } });
        $('input[id^=fserial' + ct + '_' + 1 + '' + ']').rules("add", { required: true, number: true, min: 0, messages: { required: "" } });
        $('input[id^=lserial' + ct + '_' + 1 + '' + ']').rules("add", { required: true, number: true, min: 0, messages: { required: "" } });
    },
    removeLangauage: function (e) {
        var rows = $(e).parents("div[id^=languageAirline]").find(".row-fluid").length;
        if (rows <= 5)
            $(e).parents("div[id^=languageAirline]").prev('button').removeAttr('disabled');
        $(e).parent().parent().remove();
    },
    removeAirline: function (e) {
        $(e).parents(".airlinesChildContainer").hide('slow', function () {
            $(e).parents(".airlinesChildContainer").remove();
            if ($(".airlinesChildContainer").length <= 10)
                $("#addAirlines").removeAttr("disabled");

            if (0 === $(".airlinesChildContainer").length)
                newformDeparture.addnewAirlines();
            if (1 === $(".airlinesChildContainer").length)
                $(".airlinesChildContainer:eq(0) .airlineclose").hide();
        });
    },
    scrollDown: function () {
        $('html, body').animate({
            scrollTop: $("#" + 'addAirlines').offset().top
        }, 400);
        return false;
    }
};

var validator = {
    maxCardCountPerAirline: 100,
    invalidIntValue: -1,
    invalidValue: '-1',
    emptyValue: '',
    zeroValue: '0',
    isEditing : false,
    formData: {},
    isAircraftInstance: function () {
        if ((getInstance() === "AIR"))
            return true;
        return false;
    },
    airlineData: new Map(),
    addErrorClass: function (ctrl) {
        if (null !== ctrl && undefined !== ctrl)
            ctrl.parent().addClass("sict_error");
    },
    removeErrorClass: function (ctrl) {
        if (null !== ctrl && undefined !== ctrl)
            ctrl.parent().removeClass("sict_error");
    },
    getAirlineHeader: function (airlineChild, flag) {
        var name = $(airlineChild).find("h4[id^=airlineHeader]").text();
        if (-1 === name.indexOf('-') && flag)
            name += '- Airline value not selected';
        return name;
    },
    getAirlineHeaderFromContainer: function (index) {
        var cnt = $(".airlinesChildContainer:eq(" + index + ")");
        return cnt.find("h4").text();
    },
    getIntValue: function (ctrl) {
        var value = 0;
        try {
            var ctrlVal = ctrl.val();
            value = parseInt(this.emptyValue === ctrlVal ? this.zeroValue : ctrlVal);
        } catch (e) { }
        return value;
    },
    validateAirportData: function (depObj, isNew) {
        var airportCtrl = isNew ? $("#AirportList") : $("#EditedAirportList"),
            interviewerCtrl = $("#InterviewerList"),
            fieldworkDateCtrl = $('#fieldworkDate'),
            airport = airportCtrl.val(),
            interviewer = interviewerCtrl.val(),
            fieldworkDate = fieldworkDateCtrl.val();

        var msgArray = [], canContinue = true;
        //  Check for Airport validity
        if (airport === this.invalidValue) {
            msgArray.push(newFormMessages.ERROR_NO_AIRPORT_SELECT);
            this.addErrorClass(airportCtrl);
            canContinue = false;
        } else {
            depObj.AirportId = airport;
            this.removeErrorClass(airportCtrl);
            canContinue = !canContinue ? canContinue : true;
        }
        //  Check for interviewer validty
        if (interviewer === this.invalidValue) {
            msgArray.push(newFormMessages.ERROR_NO_INTRVWR_SELECT);
            this.addErrorClass(interviewerCtrl);
            canContinue = false;
        } else {
            depObj.InterviewerId = interviewer;
            this.removeErrorClass(interviewerCtrl);
            canContinue = !canContinue ? canContinue : true;
        }
        //  Check for field work date
        if (fieldworkDate === this.emptyValue) {
            msgArray.push(newFormMessages.ERROR_NO_FLDWRK_SELECT);
            this.addErrorClass(fieldworkDateCtrl);
            canContinue = false;
        } else {
            depObj.FieldWorkDate = fieldworkDate;
            this.removeErrorClass(fieldworkDateCtrl);
            canContinue = !canContinue ? canContinue : true;
        }
        //	Display the error message
        if (!canContinue)
            pageHelper.notify(msgArray.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);

        return canContinue;
    },
    checkAirlineLangInputs: function (index, languageRow, bCtrl, messages, ctrls, cards) {
        var isinvLangAddedd = false, isinvFSAddedd = false, isinvLSAddedd = false,
            isAnySelected = false, me = this, tCards = 0,
            bCards = me.getIntValue(bCtrl),
            aServerObject = me.airlineData.get(index);

        var fsCtrls = [], lsCtrls = [];
        $.each(languageRow, function (j, val) {
            var row = $(this),
                langCtrl = row.find("select"),
                langId = row.find("option:selected").val(),
                fsCtrl = row.find("input[id^=fserial]"),
                fsValue = row.find("input[id^=fserial]").val(),
                lsCtrl = row.find("input[id^=lserial]"),
                lsValue = row.find("input[id^=lserial]").val(),
                fsIntValue = me.getIntValue(fsCtrl),
                lsIntValue = me.getIntValue(lsCtrl),
                langServerObject = me.getLanguageObject();
            //  Check for language selection
            if (me.invalidValue === langId) {
                if (!isinvLangAddedd)
                    messages.push(newFormMessages.ERROR_EMPTY_LANGUAGE);
                isinvLangAddedd = true;
                ctrls.push(langCtrl);
            }
            else {
                isAnySelected = true;
                langServerObject.LanguageId = langId;
            }
            //  Check for first serial number
            if (me.invalidValue === fsValue || me.emptyValue === fsValue || me.zeroValue === fsValue || 0 > fsIntValue || isNaN(fsIntValue)) {
                if (!isinvFSAddedd)
                    messages.push(newFormMessages.ERROR_EMPTY_SERIAL);
                isinvLangAddedd = true;
                ctrls.push(fsCtrl);
            }
            else {
                isAnySelected = true;
                langServerObject.FirstSerialNo = fsIntValue;
            }
            //  Check for last serial number
            if (me.invalidValue === lsValue || me.emptyValue === lsValue || me.zeroValue === lsValue || 0 > lsIntValue || isNaN(lsIntValue)) {
                if (!isinvLSAddedd)
                    messages.push(newFormMessages.ERROR_EMPTY_SERIAL);
                ctrls.push(lsCtrl);
            }
            else {
                isAnySelected = true;
                langServerObject.LastSerialNo = lsIntValue;
            }
            //  Check for first and last serial number values
            //[#73201] [Customer]:New form entries are allowed even if start card number is more than end card number
            if (fsIntValue > lsIntValue) {
                messages.push(newFormMessages.ERROR_GREATER_SERIAL);
                ctrls.push(fsCtrl);
                ctrls.push(lsCtrl);
                //  exchange
                var t = fsIntValue;
                fsIntValue = lsIntValue;
                lsIntValue = t;
            }
            //  push individual cards to array
            var langCards = [];
            for (var i = fsIntValue; i <= lsIntValue && 0 !== fsIntValue && 0 !== lsIntValue ; i++)
                langCards.push(i);
            tCards += langCards.length;

            fsCtrls.push(fsCtrl);
            lsCtrls.push(lsCtrl);

            cards.push(langCards);
            aServerObject.Languages.push(langServerObject);
        });
        //  Check if business has exceeded the total distribution
        if (bCards > tCards) {
            messages.push(newFormMessages.ERROR_BUSINESS_CARDS_MISMATCH);
            ctrls.push(bCtrl);
        }
        //  Check if total has exceeded the limit
        if (tCards > me.maxCardCountPerAirline) {
            messages.push(newFormMessages.ERROR_EXCEED_CARDLIMIT);
            fsCtrls.forEach(function (fCtrl) { ctrls.push(fCtrl) });
            lsCtrls.forEach(function (lCtrl) { ctrls.push(lCtrl) });
        }
        return isAnySelected;
    },
    checkAirlineInputs: function (index, airlineChild, messages, ctrls, cards) {
        var isAnySelected = false, me = this, aServerObject = me.airlineData.get(index);
        //Processing each airline
        var row = $(airlineChild),
            airlineCtrl = row.find(".airlinesdrop"),
            airlineId = row.find(".airlinesdrop option:selected").val(),
            airlineOption = row.find(".airlinesdrop option:selected"),
            destinationCtrl = row.find("select[id^=destination]"),
            destinationId = row.find("select[id^=destination] option:selected").val(),
            aircraftCtrl = row.find("select[id^=aircrafttype]"),
            aircraftId = row.find("select[id^=aircrafttype] option:selected").val(),
            codeCtrl = row.find("input[id^=code]"),
            code = row.find("input[id^=code]").val(),
            flightCtrl = row.find("input[id^=flight]"),
            flightNo = row.find("input[id^=flight]").val(),
            flightIntNo = parseInt(flightNo),
            businessCtrl = row.find("input[id^=businesscard]"),
            businessCards = row.find("input[id^=businesscard]").val(),
            businessIntCards = me.getIntValue(businessCtrl),
            languageRow = row.find("div[id^=languageAirline] .row-fluid");
        //  Check for Airline selection
        if (me.invalidValue === airlineId) {
            messages.push(newFormMessages.ERROR_EMPTY_AIRLINE);
            ctrls.push(airlineCtrl);
        }
        else {
			var airlineInfo = { isInit:false};
			airportUtil.initAirlineInfo(isDeparture, airlineId, destinationId, airlineInfo);
			isAnySelected = true;
			aServerObject.AirlineId = airlineId;
			aServerObject.OriginId = isDeparture ? airlineOption.attr('originId') : destinationId;
			if(airlineInfo.isInit){
				aServerObject.Route = airlineInfo.route;
				aServerObject.FlightType = airlineInfo.flightType === null ? airlineOption.attr('FlightType') : airlineInfo.flightType,
				aServerObject.Direction = airlineInfo.direction;
			}
			else{
				var newFlightType = communion.hostInstance === 'EUR' ? airportUtil.getFlightType(isDeparture, airlineId, destinationId): null;
				aServerObject.Route = airlineOption.attr('route');
				aServerObject.FlightType = newFlightType === null ? airlineOption.attr('FlightType') : newFlightType,
				aServerObject.Direction = airlineOption.attr('direction');
			}
        }
        //  Check for Destination selection
        if (me.invalidValue === destinationId) {
            messages.push(newFormMessages.ERROR_EMPTY_DESTINATION);
            ctrls.push(destinationCtrl);
        }
        else {
            isAnySelected = true;         
            aServerObject.DestinationId = isDeparture ? destinationId :  (me.isEditing ? $('#EditedAirportList').val() : $('#AirportList').val());
        }

        if (me.isAircraftInstance()) {
            //  Check for Destination selection
            if (me.invalidValue === aircraftId) {
                messages.push(newFormMessages.ERROR_EMPTY_AIRCRAFT);
                ctrls.push(aircraftCtrl);
            }
            else {
                isAnySelected = true;
                aServerObject.AircraftType = aircraftId;
            }
        }
        //  Check for Code of Airline
        if (me.emptyValue === code) {
            messages.push(newFormMessages.ERROR_EMPTY_CARD);
            ctrls.push(codeCtrl);
        }
        else // No need to add code for sending to server
            isAnySelected = true;

        //  Check for Flight number of Airline
        if (me.emptyValue === flightNo || isNaN(flightIntNo)) {
            messages.push(newFormMessages.ERROR_EMPTY_FLIGHT);
            ctrls.push(flightCtrl);
        }
        else {
            isAnySelected = true;
            if (flightNo.length > 5) {
                messages.push(newFormMessages.ERROR_INVALID_FLIGHT);
                ctrls.push(flightCtrl);
            }
            else {
                aServerObject.FlightNumber = code + flightNo;
            }
        }
        //  Check for Business card distribution
        if (me.invalidValue === businessCards || 0 > businessIntCards || isNaN(businessIntCards)) {
            messages.push(newFormMessages.ERROR_EMPTY_BCARDS);
            ctrls.push(businessCtrl);
        }
        else {
            aServerObject.BCardsDistributed = businessIntCards;
        }

        // Processing languages
        var isAnyLangSel = this.checkAirlineLangInputs(index, languageRow, businessCtrl, messages, ctrls, cards);
        isAnySelected = !isAnySelected ? isAnyLangSel : isAnySelected;
        return isAnySelected;
    },
    checkAirlines: function () {
        var me = this;
        me.airlineData = new Map();
        var airlineChildren = $(".airlinesChildContainer"),
            totalLength = airlineChildren.length;

        var airlineErrors = [], airlineCardsMap = new Map(), notAirlines = [];
        airlineChildren.each(function (i, ele) {
            var messages = [], ctrls = [], cards = [], iAirlineData = me.getAirlineObject();
            me.airlineData.put(i, iAirlineData);
            var isAnySelected = me.checkAirlineInputs(i, ele, messages, ctrls, cards);
            //if (1 === totalLength && !isAnySelected) {
            //    pageHelper.notify(newFormMessages.ERROR_GENERAL_MESSAGE + '<br>' + me.getAirlineHeader(ele, true),
            //        pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            //    $(ele).addClass('sict_row_error');
            //    canContinue = false;
            //}
            if (!isAnySelected) {   //  nothing is selected
                notAirlines.push(me.getAirlineHeader(ele, true));
                $(ele).addClass('sict_row_error');
            }
            else if (isAnySelected && messages.length > 0) {    //  some thing is selected
                airlineErrors.push(
                    {
                        id: i,
                        sMsg: me.getAirlineHeader(ele) + ' ' + messages[0],
                        messages: messages,
                        ctrls: ctrls
                    });

                //  Special case to show the both controls highlighting in case of 50 or more cards
                if (newFormMessages.ERROR_EXCEED_CARDLIMIT !== messages[0])
                    me.addErrorClass(ctrls[0]);
                else
                    ctrls.forEach(function (eCtrl) { me.addErrorClass(eCtrl); });
            }
            else if (isAnySelected && messages.length === 0) {  //  All seems to be fine
                // store for overlapping of cards
                if (cards.length > 0) {
                    for (var k = 0; k < cards.length && cards[k].length > 0; k++) {
                        airlineCardsMap.put(i, cards);
                        break;
                    }
                }
            }
        });
        //  Indicate error for invalid selection
        if (notAirlines.length > 0) {
            pageHelper.notify(newFormMessages.ERROR_GENERAL_MESSAGE + '<br>' + notAirlines.join('<br>'),
                 pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }
        //  Indicate for control invalid selection
        if (airlineErrors.length > 0) {
            var userMsg = newFormMessages.ERROR_GENERAL_MESSAGE;
            airlineErrors.forEach(function (v) {
                userMsg += '<br>' + v.sMsg;
                me.airlineData.remove(v.id);
            });
            //  This is for all airlines
            pageHelper.notify(userMsg, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }
        // check for overlapping of cards
        if (me.airlineData.size() > 0) {
            var olAirlines = me.isOverlappingCards(airlineCardsMap);
            if (olAirlines.length > 0)
                me.showOverppingError(olAirlines);
        }
        //  Check for invalid entry of server objects
        if (me.airlineData.size() > 0) {
            this.updateServerObjects();
        }
        //  if we still have data then this should be sent to server
        return (me.airlineData.size() > 0);
    },
    addOverlappingErrorClass: function (id, rowNo) {
        var airline = $(".airlinesChildContainer:eq(" + id + ")");
        var aText = airline.find('div h4').text();
        langRow = airline.find("div[id^=languageAirline] .row-fluid:eq(" + rowNo + ")");
        this.addErrorClass(langRow.find('input[id^=fserial]'));
        this.addErrorClass(langRow.find('input[id^=lserial]'));
        return aText;
    },
    findArrayIntersection: function (a, b) {
        var ai = 0, bi = 0;
        var result = new Array();
        while (ai < a.length && bi < b.length) {
            if (a[ai] < b[bi]) {
                ai++;
            }
            else if (a[ai] > b[bi]) {
                bi++;
            }
            else /* they're equal */ {
                result.push(a[ai]);
                ai++;
                bi++;
            }
        }
        return result;
    },
    isArrayCardsOverlapping: function (a1, a2, aId, olAirlines, isWithin) {
        //  Check within the airline languages
        for (var aj = 0; aj < a1.length; ++aj) {
            for (var ak = isWithin ? (aj + 1) : 0; ak < a2.length; ++ak) {
                if ((me.findArrayIntersection(a1[aj], a2[ak])).length > 0) {
                    var aText = this.addOverlappingErrorClass(aId, ak);
                    if (-1 === olAirlines.indexOf(aText))
                        olAirlines.push(aText);
                    me.airlineData.remove(aId);
                }
            }
        }
    },
    isOverlappingCards: function (airlineCardsMap) {
        var airlinesIds = [], lArray = [];
        airlineCardsMap.forAll(function (k, v) {
            airlinesIds.push(k);
            lArray.push(v);
        });
        var olAirlines = [];
        var me = this,
                airline,
                langRow;
        if (lArray && lArray.length > 0) {
            var arrVal, scndArrVal;
            //  Check across airlines
            for (var i = 0; i < lArray.length; i++) {
                arrVal = lArray[i];
                //  Check within the airline languages
                me.isArrayCardsOverlapping(arrVal, arrVal, airlinesIds[i], olAirlines, true);
                //  Check against other airlines
                for (var j = i + 1; j < lArray.length; ++j) {
                    scndArrVal = lArray[j];
                    me.isArrayCardsOverlapping(arrVal, scndArrVal, airlinesIds[j], olAirlines, false);
                }
            }
        }
        return olAirlines;
    },
    showOverppingError: function (olAirlines) {
        if (olAirlines.length > 0) {
            var msg = '';
            for (var olI = 0; olI < olAirlines.length; olI++)
                msg += olAirlines[olI] + ' ' + newFormMessages.ERROR_AIRLINES_OVERLAPPING + '<br>';

            pageHelper.notify(msg, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }
    },
    getAirlineObject: function () {
        var airlineObject =
        {
            AirlineId: 0,
            FlightNumber: '',
            DestinationId: 0,
            BCardsDistributed: 0,
            OriginId: 0,
            Route: '',
            FlightType: '',
            Direction: '',
            Languages: [],
            AircraftType: 0
        };
        return airlineObject;
    },
    getLanguageObject: function () {
        var languageObject =
        {
            LanguageId: -1,
            FirstSerialNo: 0,
            LastSerialNo: 0
        };
        return languageObject;
    },
    updateServerObjects: function () {
        var me = this, isValid = true;
        var isDep = newformDeparture.checkInNewfDep();
        if (me.airlineData.size() > 0) {
            var invValidIds = [];
            me.airlineData.forAll(function (k, v) {
                isValid = false;
                var airlineid = parseInt(v.AirlineId),
                    flightNumber = v.FlightNumber,
                    destinationId = parseInt(v.DestinationId),
                    originId = parseInt(v.OriginId),
                    aircraftId = parseInt(v.AircraftType);

                if (NaN !== airlineid && me.invalidIntValue !== airlineid &&
                    //[#73343] [Customer]: EU + International instances � Entry forms- The Flight no. field is accepting more than 5 digits.
                    (flightNumber.length > 2 && flightNumber.length <= 7)) {
                    if (isDep) {
                        if (NaN !== destinationId && me.invalidIntValue !== destinationId)
                            isValid = true;
                    }
                    else {
                        if (NaN !== originId && me.invalidIntValue !== originId)
                            isValid = true;
                    }
                }

                if (isValid) {
                    if (NaN !== aircraftId && me.invalidIntValue !== aircraftId)
                        isValid = true;
                }

                if (isValid) {
                    if (v.Languages.length > 0) {
                        v.Languages.forEach(function (item) {
                            var lId = parseInt(item.LanguageId),
                                fNo = parseInt(item.FirstSerialNo),
                                lNo = parseInt(item.LastSerialNo);

                            if (NaN === lId || me.invalidIntValue === lId ||
                                NaN === fNo || me.invalidIntValue === fNo ||
                                NaN === fNo || me.invalidIntValue === fNo)
                                isValid = false;
                        }
                        );
                    }
                }
                if (!isValid)
                    invValidIds.push(k);
            });
            invValidIds.forEach(function (key) {
                me.airlineData.remove(key);
            });
        }
    },
    submitForm: function (origin) {
        me = this;
        me.formData = { AirportId: '', FieldWorkDate: '', InterviewerId: '', Airlines: [], IsDepartureForm: isDeparture.toString() };

        $(".sict_error").removeClass('sict_error');
        $(".sict_row_error").removeClass('sict_row_error');
        pageHelper.clearStickies();

        if (validator.validateAirportData(this.formData, true)) {
            if (validator.checkAirlines()) {
                me.airlineData.forEach(function (value) { me.formData.Airlines.push(value) });

                pageHelper.addSmokeSignal(newFormMessages.GENERAL_PROCSING_REQUEST);
                sendNewForm(me.formData);
            }
        }
    },
    updateForm: function (origin, sEle, callback) {
        me = this;
        me.isEditing = true;
        me.formData = {
            AirportId: sessionStorage.selectedAirportId,
            FieldWorkDate: '',
            InterviewerId: sEle.InterviewerId,
            Airlines: [],
            IsDepartureForm: isDeparture.toString(),
            FormId: sEle.FormId
        };
        $(".sict_error").removeClass('sict_error');
        $(".sict_row_error").removeClass('sict_row_error');
        pageHelper.clearStickies();

        if (validator.validateAirportData(this.formData, false)) {
            if (validator.checkAirlines()) {
                me.airlineData.forEach(function (value) { me.formData.Airlines.push(value) });
                console.log(me.formData);
                pageHelper.addSmokeSignal(newFormMessages.GENERAL_PROCSING_REQUEST);
                sendNewForm(me.formData, callback, true);
            }
        }
        me.isEditing = false;
    },
    handleResponse: function (resp, callback) {
        var me = this;
        pageHelper.removeSmokeSignal();

        if (null !== callback && undefined !== callback) {
            if (resp.AirlineDetails && resp.AirlineDetails.length === 1) {
                if (resp.ReturnCode > 0 && resp.AirlineDetails[0].InvalidLanguages === null &&
                        resp.AirlineDetails[0].IsSerialNoValid && resp.AirlineDetails[0].IsSuccess) {
                    return callback(me.formData);
                }
            }
        }

        if (resp.ReturnCode > 0) {
            if (resp.AirlineDetails && resp.AirlineDetails.length > 0) {
                var index = 0, name = '', successAirlines = [], failedAirlines = [];
                me.airlineData.forAll(function (k, v) {
                    name = me.getAirlineHeaderFromContainer(k);
                    if (resp.AirlineDetails[index].IsSerialNoValid && resp.AirlineDetails[index].IsSuccess) {
                        successAirlines.push(name);
                        $(".airlinesChildContainer:eq(" + k + ") .airlineclose").click();
                    }
                    else {
                        if (null !== resp.AirlineDetails[index].InvalidLanguages) {
                            failedAirlines.push(name);
                            var airline = $(".airlinesChildContainer:eq(" + k + ")");
                            var langSection = airline.find("div[id^=languageAirline]");
                            $.each(resp.AirlineDetails[index].InvalidLanguages, function (i, val) {
                                langRow = langSection.find(".row-fluid:eq(" + (parseInt(val, 10) - 1) + ")");
                                me.addErrorClass(langRow.find("input[id^=fserial]"));
                                me.addErrorClass(langRow.find("input[id^=lserial]"));
                            });
                        }
                    }
                    index++
                });
                if (successAirlines.length > 0)
                    pageHelper.notify(newFormMessages.SUCCESS_AIRLINE_SUBMIT + '<br>' + successAirlines.join("<br>"),
                        pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
                if (failedAirlines.length > 0)
                    me.showOverppingError(failedAirlines);
            }
        }
        me.formData = {};
    },
};

function initialiseStatics(iNeeded) {
    var optionString = airportUtil.airportListOptions(), apSelect = $('#AirportList');
    apSelect.append(optionString);
    //	Select if only one airport is present and make it readonly
    if (apSelect[0].length === 2) {
        apSelect[0].disabled = true;
        apSelect[0].selectedIndex = 1;
        cacheMgr.selectedAirport(apSelect.val());
    }

    if (iNeeded === undefined || iNeeded === null) {
        optionString = airportUtil.interviewerListOptions();
        $('#InterviewerList').append(optionString);
    }

    initialiseAirlineStatics();
}

var languageString = '', airlineString = '', airlineVsProps = [], firstAirlineDestinatons = '', aircraftTypeString = '';
function initialiseAirlineStatics() {
    var selairport = $("#AirportList").val();
    if (selairport === "-1")
        return false;
    else
        cacheMgr.selectedAirport(selairport);

    airlineString = airportUtil.airlineListOptions(isDeparture);
    
    //[#73705] US International:Viewl forms:Origin is displaying as ‘Please select Destination’.
    firstAirlineDestinatons = '<option value="-1">-- Please select an Airport --</option>';
    $.each($("select[id^=destination]"), function (i, elem) {
        $(elem).empty().append(firstAirlineDestinatons);
    });

    $.each($('select[id^=airlinesDD]'), function (i, elem) {
        $(elem).empty().append(airlineString);
    });

    $.each($("input[id^=code]"), function (i, elem) {
        $(elem).val('');
    });
    languageString = airportUtil.languageOptions();
    $.each($("select[id^=language]"), function (i, elem) {
        $(elem).empty().append(languageString);
    });
    aircraftTypeString = airportUtil.aircraftTypeListOptions();
    $.each($("select[id^=aircrafttype]"), function (i, elem) {
        $(elem).empty().append(aircraftTypeString);
    });
}

function callChangeInterviewer() {
    cacheMgr.selectedAirport($('#AirportList').val());
    var optionString = airportUtil.interviewerListOptions();
    $('#InterviewerList').empty().append(optionString);

    initialiseAirlineStatics();
}

function callOnAirlinesChange(id) {
    airlineVsProps = cacheMgr.airlineList(isDeparture);

    var airlineId = parseInt(document.getElementById("airlinesDD" + id).value);
    var header = 'Flight ' + id;
    var airlineDestinatons = firstAirlineDestinatons;
    //[#73122] [Customer]:New airport login – Entry forms
    $('#code' + id).val('');
    if (-1 !== airlineId) {
        for (var j = 0; j < airlineVsProps.length; j++) {
            if (airlineId === airlineVsProps[j].id) {
                $('#code' + id).val(airlineVsProps[j].code);
                header += ' - ' + airlineVsProps[j].name;
                //[#72743] [Customer]:Departure Forms- Need to show Destinations in alphabetical order under Destination dropdown.
                airlineVsProps[j].destinations.sort(function (a, b) { return (a).destName.localeCompare((b).destName) });
                for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
                    airlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
                break;
            }
        }
    }
    $('#destination' + id).empty().append(airlineDestinatons);
    $('#airlineHeader' + id).text(header);
}