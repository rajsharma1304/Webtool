/*<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> newform.js </FileName>
 <Author> Raghavendra G.N, Akhilesh M.S, Vivek.A </Author>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var newFormMessages =
        {
            ERROR_NO_AIRPORT_SELECT: "Please select an airport to continue",
            ERROR_NO_INTRVWR_SELECT: "Please select an interviewer to continue",
            ERROR_NO_FLDWRK_SELECT: "Please select date of form distribution",
            ERROR_BUSINESS_CARDS_MISMATCH: 'No of business cards for below airline exceed total card range',
            ERROR_AIRLINES_OVERLAPPING: "Found conflicting serial numbers in following airlines",
            ERROR_EXCEED_CARDLIMIT: "Cards distribution has exceeded the maximum configured limit",
            SUCCESS_AIRLINE_UPDATE: "Successfully updated the airline",
            SUCCESS_AIRLINE_SUBMIT: "Successfully added following airlines",
            ERROR_GENERAL_MESSAGE: "Please re-check airlines before continuing",
            ERROR_GENERAL_CONTROLS: "Please re-check airlines for highlighted controls",
            GENERAL_PROCSING_REQUEST: 'Processing your request. Please wait..',
            INFO_MAX_LANGUAGE_SUPPORT: "Maximum allowed languages per airline is 5"
        };

var newformDeparture = {
    maxCardCountPerAirline: 50,
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
    initializeDepartureForm: function() {
        var me = this;
        //Intialize FieldWork with today's date.
        $('#fieldworkDate').datepicker({
            format: "mm/dd/yyyy",
            autoclose: true,
            orientation: "top left",
            todayHighlight: true,
            startDate: me.fieldWorkRange.START,
            endDate: me.fieldWorkRange.END
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
        initialiseStatics();
        me.checkValueisNumber("");
        var isAIR = (getInstance() === "AIR") ? true : false;
        if (isAIR)
            me.addAircraftTypeControl();
    },
    addAircraftTypeControl: function() {
        if (isDeparture) {
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
        }
    },
    handleFocusIn: function() {
        $("input").focusin(function() {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('.sict_error');
        });
        $("select").focusin(function() {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('sict_error');
        });
    },
    checkValueisNumber: function(elem) {
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

        $("input[type=text]").focusout(function(e) {
            var me = this, elemId = e.target.id, inputValue = me.value.replace(/\s+/g, ""),
                    parent = $(me).parent();
            if (elemId === "fieldworkDate") {
                if ((inputValue === ""))
                    me.value = moment().format("MM/DD/YYYY");

                if (moment(inputValue, "MM/DD/YYYY").diff(moment().format("MM/DD/YYYY"), 'days') > 0)
                    $(this).datepicker('setValue', moment().format("MM/DD/YYYY"));

                if (moment(me.value, "MM/DD/YYYY").year() < 2000)
                    parent.addClass('sict_error');
                else
                    parent.removeClass('sict_error');

                return false;
            }
            if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > -1) {
                    if (/^flight/g.test(elemId)) {
                        if (inputValue.length > 5)
                            parent.addClass('sict_error');
                        else
                            parent.removeClass('sict_error');
                        return false;
                    }
                    parent.removeClass('sict_error');
                    return;
                }
                
                var target = $(e.target);
                var targetString = $(e.target).val();
                if(targetString.length > 0){
                    for (var i = 0; i < targetString.length; i++) {
                        var charCode = targetString.charCodeAt(i);
                        //To check if the string has alphabets.
                        if(charCode > 65 && charCode < 122){
                            target.parent().addClass('sict_error'); 
                            $(e.target).val('');
                            return;
                        }else{
                            target.parent().removeClass('sict_error');
                        }
                    }
                }else{
                    target.parent().addClass('sict_error');
                }
                
            }
            //	Add the error class to inform it is invalid
            parent.addClass('sict_error');
        });

        $('input[type=text]').keydown(function(e) {
            var keyCodes = [46, 8, 9, 27, 13, 110, 190];
            if (e.target.id === "fieldworkDate")
                keyCodes.push(191);	// Support the '/' separator
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, keyCodes) !== -1 ||
                    //[#72764] [Customer]:Departure/Arrival forms- Cannot use keyboard shortcuts like Ctrl V and Ctrl C in any of the text boxes present on the entry forms.   
                            // Allow: Ctrl+A, Ctrl+C, Ctrl+V
                                    (e.keyCode == 65 || e.keyCode == 67 || e.keyCode == 86 ||
                                            //Ctrl+a, Ctrl+c, Ctrl+v
                                            e.keyCode == 97 || e.keyCode == 99 || e.keyCode == 118 ||
                                            //Ctrl + Z, Ctrl + z
                                            e.keyCode == 90 || e.keyCode == 122 &&
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
                        });


                $("select").focusout(function(e) {
                    var target = $(e.target);
                    if ("-1" === target.val())
                        target.parent().addClass('sict_error');
                    else
                        target.parent().removeClass('sict_error');
                });
            },
            fillNumbers: function(start, end) {
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
            intersectSafe: function(a, b) {
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
            triggerError: function(resp) {
                var me_ = this;
                if (resp.ReturnCode > 0) {
                }
                else
                    pageHelper.notify(resp.ReturnMessage, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);
            },
            triggerSuccess: function(resp, callback) {
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
                        $.each(resp.AirlineDetails, function(i, airline) {
                            if (airline.IsSerialNoValid && airline.IsSuccess) {
                                inValidAirlineLang.push({});
                                iAL.push(i);
                            }
                            else {
                                if (airline.InvalidLanguages === null)
                                    return false;
                                inValidAirlineLang.push({airline: airline.AirlineId, invalidLang: airline.InvalidLanguages})
                                airlineError.push(i + 1);
                            }
                        });
                        $.each(iAL, function(i, el) {
                            var sALCntr = $(".airlinesChildContainer:eq(" + me_.submitted_airlines[el] + ")");
                            sALNames.push(sALCntr.find("h4").text());
                            delete me_.submitted_airlines[el];
                        });

                        $.each(inValidAirlineLang, function(i, obj) {
                            if (!$.isEmptyObject(obj)) {
                                airline = $(".airlinesChildContainer:eq(" + me_.submitted_airlines[i] + ")");
                                airline.addClass('sict_row_error');
                                langSection = airline.find("div[id^=languageAirline]");
                                $.each(obj.invalidLang, function(i, val) {
                                    langRow = langSection.find(".row-fluid:eq(" + (parseInt(val, 10) - 1) + ")");
                                    langRow.find("input[id^=fserial]").parent().addClass("sict_error");
                                    langRow.find("input[id^=lserial]").parent().addClass("sict_error");
                                });
                            }//has empty Object which has to removed for Success.
                        });
                        //Appending Airlines Template
                        //1)Atleast One Element is there ?
                        $(".airlinesChildContainer:not(.sict_row_error)").hide('slow', function() {
                            $(".airlinesChildContainer:not(.sict_row_error)").remove();
                            if ($(".airlinesChildContainer").length === 0) {
                                newformDeparture.addnewAirlines();
                            }
                            if ($(".airlinesChildContainer:eq(0) .airlineclose").length > 0) {
                                $(".airlinesChildContainer:eq(0) .airlineclose").remove();
                            }
                        });
                        if (airlineError.length > 0) {
                            $(".airlinesChildContainer.sict_row_error").each(function(i, ele) {
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
            checkInNewfDep: function() {
                return isDeparture;
            },
            submitFormDeparture: function(origin) {
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
                        depObj = {AirportId: "", FieldWorkDate: "", InterviewerId: "", Airlines: []};

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
                airlineChild.each(function(i, ele) {
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
                    $.each(languageRow, function(j, val) {
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
                    $.each(me_.errored_airlines, function(i, obj) {
                        airline = $(".airlinesChildContainer:eq(" + obj + ")");
                        airline.addClass('sict_row_error');
                        airlineNames.push(airline.find("h4").text());
                    });
                    if (airlineNames.length > 0)
                        pageHelper.notify(newFormMessages.ERROR_GENERAL_MESSAGE + '<br>' + airlineNames.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
                    //  Get All airlines which are erroring out
                    airlineNames = [];
                    $.each($(".sict_error"), function(i, obj) {
                        var eleText = $(obj).closest('.formSep').find('h4').text();
                        if (-1 === airlineNames.indexOf(eleText))
                            airlineNames.push(eleText);
                    });
                    //Ignore the business, overlapping and exceeding limit airlines
                    var index = -1;
                    $.each(bCardsAirlineMatch, function(i, obj) {
                        airline = $(".airlinesChildContainer:eq(" + obj + ")");
                        airline.addClass('sict_row_error');
                        index = airlineNames.indexOf(obj);
                        airlineNames = -1 !== index ? airlineNames.splice(index + 1, 1) : airlineNames;
                    });
                    $.each(maxCardDistribution, function(i, obj) {
                        airline = $(".airlinesChildContainer:eq(" + obj + ")");
                        airline.addClass('sict_row_error');
                        index = airlineNames.indexOf(obj);
                        airlineNames = -1 !== index ? airlineNames.splice(index + 1, 1) : airlineNames;
                    });
                    $.each(olAirlines, function(i, obj) {
                        index = airlineNames.indexOf(obj);
                        airlineNames = -1 !== index ? airlineNames.splice(index + 1, 1) : airlineNames;
                    });
                    if (airlineNames.length > 0)
                        pageHelper.notify(newFormMessages.ERROR_GENERAL_CONTROLS + '<br>' + airlineNames.join('<br>'), pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
                }
                if (me_.isFormSubmittable) {
                    //  Highlight the business and max limit card airlines
                    $.each(bCardsAirlineMatch, function(i, obj) {
                        airline = $(".airlinesChildContainer:eq(" + obj + ")");
                        airline.addClass('sict_row_error');
                    });
                    $.each(maxCardDistribution, function(i, obj) {
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
            submitEditDeparture: function(origin, sEle, callback) {
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
                        depObj = {AirportId: "", FieldWorkDate: "", InterviewerId: "", FormId: sEle.FormId, Airlines: []};

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
                airlineChild.each(function(i, ele) {
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
                    $.each(languageRow, function(j, val) {
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
            getNumberofCards: function() {
            },
            validateLanguages: function(lArray) {
                console.log("I am Validating Languages");
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
            newAirline: function(scBt) {
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
                        '<i class="close airlineclose ttip_l" title="Discard Airline" onclick="newformDeparture.removeAirline(this);"' + scBtStyle + ' >&times;</i>' +
                        '<div class="row-fluid">' +
                        '<div class="span6">' +
                        '<h4 id="airlineHeader{0}" class="sepH_c"> Airline {0} </h4>' +
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
                        '<select class="span10 destdrop" id="destination{0}" name="destination{0}">' +
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
            addnewAirlines: function() {
                var me = this;
                if ($(".airlinesChildContainer").length >= 9) {
                    $("#addAirlines").attr("disabled", "disabled");
                }
                var template = jQuery.validator.format($.trim((me.newAirline())));
                $('#airlinesContainer').append($(template(++me.airlineCounter))).show('slow');
                setTimeout(function() {
                    me.scrollDown();
                }, 500);
                me.checkValueisNumber("");
                gebo_tips.init();
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
                $(e).parents(".airlinesChildContainer").hide('slow', function() {
                    $(e).parents(".airlinesChildContainer").remove();
                    if ($(".airlinesChildContainer").length <= 10)
                        $("#addAirlines").removeAttr("disabled");
                });
            },
            scrollDown: function() {
                $('html, body').animate({
                    scrollTop: $("#" + 'addAirlines').offset().top
                }, 400);
                return false;
            }
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

            firstAirlineDestinatons = '<option value="-1">-- Please select destination --</option>';
            $.each($("select[id^=destination]"), function(i, elem) {
                $(elem).empty().append(firstAirlineDestinatons);
            });

            $.each($('select[id^=airlinesDD]'), function(i, elem) {
                $(elem).empty().append(airlineString);
            });

            $.each($("input[id^=code]"), function(i, elem) {
                $(elem).val('');
            });
            languageString = airportUtil.languageOptions();
            $.each($("select[id^=language]"), function(i, elem) {
                $(elem).empty().append(languageString);
            });
            aircraftTypeString = airportUtil.aircraftTypeListOptions();
            $.each($("select[id^=aircrafttype]"), function(i, elem) {
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
            var header = 'Airline ' + id;
            var airlineDestinatons = firstAirlineDestinatons;
            if (-1 !== airlineId) {
                for (var j = 0; j < airlineVsProps.length; j++) {
                    if (airlineId === airlineVsProps[j].id) {
                        $('#code' + id).val(airlineVsProps[j].code);
                        header += ' - ' + airlineVsProps[j].name;
                        for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
                            airlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
                        break;
                    }
                }
            }
            $('#destination' + id).empty().append(airlineDestinatons);
            $('#airlineHeader' + id).text(header);
        }