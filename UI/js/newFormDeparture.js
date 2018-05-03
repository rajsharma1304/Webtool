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
    fieldWorkRange: {
        START: '-14d',
        END: '+14d'
    },
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
    },
    handleFocusIn: function() {
        $("input").focusin(function() {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('.sict_error');
        });
    },
    checkValueisNumber: function(elem) {
        if (elem != "") {
            var inputValue = elem[0].value.replace(/\s+/g, ""), elemId = elem[0].id, parent = elem.parent();
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
            if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > -1) {
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
                if (inputValue === "")
                    parent.addClass('sict_error');
                else
                    parent.removeClass('sict_error');
                return false;
            }
            if (/^flight/g.test(elemId)) {
                if (inputValue.length > 5)
                    parent.addClass('sict_error');
                else
                    parent.removeClass('sict_error');
                return false;
            }
            if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > -1) {
                    parent.removeClass('sict_error');
                    return;
                }
            }
            //	Add the error class to inform it is invalid
            parent.addClass('sict_error');
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
        while (ai < a.length && bi < b.length)
        {
            if (a[ai] < b[bi]) {
                ai++;
            }
            else if (a[ai] > b[bi]) {
                bi++;
            }
            else /* they're equal */
            {
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
            me_.notify(resp.ReturnMessage, me_.msg_POSITION.top.RIGHT, me_.msg_INFO);
    },
    notify: function(msg, position, type) {
        var me_ = this;
        if (msg && position && type)
            $.sticky(msg, {autoclose: 5000, position: position, type: type});
        else
            $.sticky(msg, {autoclose: 2000, position: me_.msg_POSITION.top.RIGHT, type: me_.msg_INFO});
    },
    triggerSuccess: function(resp, callback) {
        var me_ = this;
        if (null !== callback && undefined !== callback) {
            if (resp.AirlineDetails && resp.AirlineDetails.length === 1) {
                if (resp.ReturnCode > 0 && resp.AirlineDetails[0].InvalidLanguages === null &&
                        resp.AirlineDetails[0].IsSerialNoValid && resp.AirlineDetails[0].IsSuccess) {
                    me_.notify(resp.ReturnMessage, me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
                    return callback(newformDeparture.depFormData);
                }
            }
        }
        if (resp.ReturnCode > 0) {
            if (resp.AirlineDetails && resp.AirlineDetails.length > 0) {
                var inValidAirlineLang = [], airline, langSection, langRow, airlineError = [], highlightedAirline = [];
                $.each(resp.AirlineDetails, function(i, airline) {
                    if (airline.IsSerialNoValid && airline.IsSuccess) {
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
                    }//has empty Object which has to removed for Success.
                    else {
                        /*if(i === 0){
                         $(".airlinesChildContainer:eq(" + i + ") input[type=text]:not(input[id^=code])").val("");
                         }
                         else{
                         $(".airlinesChildContainer:eq(" + i + ")").remove();
                         }*/
                        //$(".airlinesChildContainer:eq(" + i + ")").remove();
                    }
                });
                //Appending Airlines Template
                //1)Atleast One Element is there ?
                $(".airlinesChildContainer:not(.sict_row_error)").remove();
                if ($(".airlinesChildContainer").length === 0) {
                    newformDeparture.addnewAirlines();
                }
                if ($(".airlinesChildContainer:eq(0) .airlineclose").length > 0) {
                    $(".airlinesChildContainer:eq(0) .airlineclose").remove();
                }

                if (airlineError.length > 0) {
                    /*if (airlineError.length > 1)
                     me_.notify(me_.msg_LANG.serial_exists.msg + "s " + airlineError.join(" ,") + ".", me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
                     else
                     me_.notify(me_.msg_LANG.serial_exists.msg + " " + airlineError.join(" ,") + ".", me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);*/
                    $(".airlinesChildContainer.sict_row_error").each(function(i, ele) {
                        highlightedAirline.push(ele.querySelector("h4").innerHTML.trim());
                    });
                    me_.notify(me_.msg_LANG.serial_exists.msg + ((highlightedAirline.length > 1) ? "s " : ' ') + highlightedAirline.join(", ") + ".", me_.msg_POSITION.top.RIGHT, me_.msg_SUCCESS);
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
                depObj = {AirportId: "", FieldWorkDate: "", InterviewerId: "", Airlines: []};

        if (isnewfDep)
            depObj.IsDepartureForm = "true";
        else
            depObj.IsDepartureForm = "false";

        depObj.AirportId = airport, depObj.FieldWorkDate = fieldworkDate.val(), depObj.InterviewerId = interviewer;
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

            if (!me_.checkValueisNumber(flightNo))
                isFlightNoEmpty = false;
            else
                isFlightNoEmpty = true;
            if (!me_.checkValueisNumber(businessCards))
                isBusinessCardsEmpty = false;
            else
                isBusinessCardsEmpty = true;

            if (!isBusinessCardsEmpty && !isFlightNoEmpty)
                tempObj.AirlineId = airlineId, tempObj.FlightNumber = code + flightNo.val().toString(), tempObj.DestinationId = destinationId, tempObj.BCardsDistributed = businessCards.val(), tempObj.OriginId = 1, tempObj.Route = "TATL", tempObj.Direction = "EAST"; //, me_.isFormSubmittable = true;//, tempObj.Code = code;

            //if (tempObj.Code === "")//Deleting the Code key temporarily
            delete tempObj["Code"];

            //Processing Language
            var totalLang = 0;
            $.each(languageRow, function(j, val) {
                var row = $(this), lang, fs, ls, tempLangObj = {}, langLabel, airlineLang = [];
                lang = row.find("option:selected").val(),
                        fs = row.find("input[id^=fserial]"),
                        ls = row.find("input[id^=lserial]"),
                        langLabel = row.find("select").prev("label");
                if (!me_.checkValueisNumber(fs)) {
                    isFSEmpty = false;
                }
                if (!me_.checkValueisNumber(ls)) {
                    isLSEmpty = false;
                }
                if (!isFSEmpty && !isLSEmpty) {
                    airlineLang = me_.fillNumbers(fs, ls);
                    totalLang = totalLang + airlineLang.length;
                    langValidation.push(airlineLang);
                    tempLangObj.LanguageId = lang, tempLangObj.FirstSerialNo = fs.val(), tempLangObj.LastSerialNo = ls.val(), me_.isFormSubmittable = true;
                    tempLang.push(tempLangObj);
                }
            });
            if (totalLang < parseInt(businessCards.val(), 10))
                bCardsAirlineMatch.push(i + 1);
            airlineLang.push(langValidation);
            tempObj.Languages = tempLang;
            depObj.Airlines.push(tempObj);
        });

        //Validating Business Cards ranges
        if (bCardsAirlineMatch.length > 0) {
            me_.notify("Business Cards Mismatch for Airlines" + bCardsAirlineMatch.join(", "), me_.msg_POSITION.top.RIGHT, me_.msg_ERROR);
            me_.isFormSubmittable = false;
        }

        //Validating Languages
        if (me_.isFormSubmittable && !isBusinessCardsEmpty && !isFlightNoEmpty && !isFSEmpty && !isLSEmpty)
            me_.validateLanguages(airlineLang);

        if ($(".sict_error").length > 0 || !me_.isFormSubmittable) {
            me_.isFormSubmittable = false;
        }
        else {
            me_.isFormSubmittable = true;
            me_.depFormData = depObj;
        }

        if (airport === "-1") {
            $("#AirportList").parent().addClass("sict_error");
            me_.isFormSubmittable = false;
        }
        else {
            $("#AirportList").parent().removeClass("sict_error");
            me_.isFormSubmittable = true;
        }
        if (interviewer === "-1") {
            $("#InterviewerList").parent().addClass("sict_error");
            me_.isFormSubmittable = false;
        } else {
            $("#InterviewerList").parent().removeClass("sict_error");
            me_.isFormSubmittable = true;
        }
        if (fieldworkDate.val() === "") {
            fieldworkDate.parent().addClass("sict_error");
            me_.isFormSubmittable = false;
        } else {
            fieldworkDate.parent().removeClass("sict_error");
            me_.isFormSubmittable = true;
        }

        if (me_.isFormSubmittable === false)
            me_.notify("Please re-check Airlines before submit.", me_.msg_POSITION.top.RIGHT, me_.msg_ERROR);

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
    submitEditDeparture: function(origin, sEle, callback) {
        var origin = origin, me_ = this, isnewfDep = me_.checkInNewfDep(), parentEle = $("#editAirlineBody"), prevEle = parentEle.prev();
        if (prevEle.hasClass("alert"))
            prevEle.remove();
        $(".sict_error").removeClass('sict_error');
        $(".sict_row_error").removeClass('sict_row_error');

        var depformp = $("#editAirlineBody"),
                depformc = depformp.children(),
                depformf = depformc.first(),
                airport = sessionStorage.airportLoginId,
                interviewerLabel = sEle.Interviewer,
                interviewer = sEle.InterviewerId,
                fieldworkDate = moment(sEle.DistributionDate).format("MM/DD/YYYY"),
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
                depObj = {AirportId: "", FieldWorkDate: "", InterviewerId: "", FormId: sEle.FormId, Airlines: []};

        if (isnewfDep)
            depObj.IsDepartureForm = "true";
        else
            depObj.IsDepartureForm = "false";

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

            if (!me_.checkValueisNumber(flightNo))
                isFlightNoEmpty = false;
            else
                isFlightNoEmpty = true;
            if (!me_.checkValueisNumber(businessCards))
                isBusinessCardsEmpty = false;
            else
                isBusinessCardsEmpty = true;

            if (!isBusinessCardsEmpty && !isFlightNoEmpty)
                tempObj.AirlineId = airlineId, tempObj.FlightNumber = code + flightNo.val().toString(), tempObj.DestinationId = destinationId, tempObj.BCardsDistributed = businessCards.val(), tempObj.OriginId = 1, tempObj.Route = "TATL", tempObj.Direction = "EAST"; //, me_.isFormSubmittable = true;//, tempObj.Code = code;

            //if (tempObj.Code === "")//Deleting the Code key temporarily
            delete tempObj["Code"];

            //Processing Language
            var totalLang = 0;
            $.each(languageRow, function(j, val) {
                var row = $(this), lang, fs, ls, tempLangObj = {}, langLabel, airlineLang = [];
                lang = row.find("option:selected").val(),
                        fs = row.find("input[id^=fserial]"),
                        ls = row.find("input[id^=lserial]"),
                        langLabel = row.find("select").prev("label");
                if (!me_.checkValueisNumber(fs)) {
                    isFSEmpty = false;
                }
                if (!me_.checkValueisNumber(ls)) {
                    isLSEmpty = false;
                }
                if (!isFSEmpty && !isLSEmpty) {
                    airlineLang = me_.fillNumbers(fs, ls);
                    totalLang = totalLang + airlineLang.length;
                    langValidation.push(airlineLang);
                    tempLangObj.LanguageId = lang, tempLangObj.FirstSerialNo = fs.val(), tempLangObj.LastSerialNo = ls.val(), me_.isFormSubmittable = true;
                    tempLang.push(tempLangObj);
                }
            });
            if (totalLang < parseInt(businessCards.val(), 10))
                bCardsAirlineMatch.push(i + 1);
            airlineLang.push(langValidation);
            tempObj.Languages = tempLang;
            depObj.Airlines.push(tempObj);
        });

        //Validating Business Cards ranges
        if (bCardsAirlineMatch.length > 0) {
            me_.notify("Business Cards Mismatch for Airlines" + bCardsAirlineMatch.join(", "), me_.msg_POSITION.top.RIGHT, me_.msg_ERROR);
            me_.isFormSubmittable = false;
        }

        //Validating Languages
        if (me_.isFormSubmittable && !isBusinessCardsEmpty && !isFlightNoEmpty && !isFSEmpty && !isLSEmpty)
            me_.validateLanguages(airlineLang);

        if ($(".sict_error").length > 0 || !me_.isFormSubmittable) {
            me_.isFormSubmittable = false;
        }
        else {
            me_.isFormSubmittable = true;
            me_.depFormData = depObj;
        }

        if (me_.isFormSubmittable) {
            if ($(".sict_error").length > 0)
                return false;
            else
                sendDepartureForm(callback, true);
        }
    },
    getNumberofCards: function() {

    },
    validateLanguages: function(lArray) {
        console.log("I am Validating Languages");
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
                    for (j = 0; j < arrVal.length; ++j)
                    {
                        for (k = j + 1; k < arrVal.length; ++k)
                        {
                            if ((me.intersectSafe(arrVal[j], arrVal[k])).length > 0) {
                                airline = $(".airlinesChildContainer:eq(" + 0 + ")");
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
                                airline = $(".airlinesChildContainer:eq(" + j + ")");
                                langRow = airline.find("div[id^=languageAirline] .row-fluid:eq(" + sk + ")");
                                langRow.find('input[id^=fserial]').parent('div').addClass('sict_error');
                                langRow.find('input[id^=lserial]').parent('div').addClass('sict_error');
                            }
                        }
                    }
                }
            }
        }
    },
    newAirline: function(scBt) {
        var scBtStyle = scBt === undefined ? '' : 'style="display:none"';
        var scBtFStyle = scBt === undefined ? 'formSep' : '';
        var dynamicType = isDeparture ? 'Destination' : 'Origin';
        var nAir = '<div class="airlinesChildContainer row-fluid">' +
                '<div class="span12 ' + scBtFStyle + '">' +
                '<i class="close airlineclose ttip_l" title="Discard Airline" onclick="newformDeparture.removeAirline(this);"' + scBtStyle + ' >&times;</i>' +
                '<div class="row-fluid">' +
                '<div class="span6">' +
                '<h4 id="airlineHeader{0}" class="sepH_c"> ' + fALName + ' </h4>' +
                '<div class="span6">' +
                '<label>Airline </label>' +
                '<select id="airlinesDD{0}" name="airlinesDD{0}" class="span10 airlinesdrop" onchange="callOnAirlinesChange({0})">' +
                airlineString +
                '</select>' +
                '</div>' +
                '<div class="span2" style="margin-left:23px;">' +
                '<label>Code</label>' +
                '<input id="code{0}" name="code{0}" disabled="disabled" type="text" class="span8" value=' + firstAirlineCode + '>' +
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
                '<input type="text" class="span5" id="businesscard{0}" name="businesscard{0}">' +
                '</div>' +
                '</div>' +
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
            me.siblings().append(languageRowString);
            //Validation for First Serial and Last Serial.
            //$('input[id^=fserial' + parentId + '_' + (langcounterval + 1) + ']').rules("add", {required: true, number: true, messages: {required: ""}});
            var lserial = $('input[id^=lserial' + parentId + '_' + (langcounterval + 1) + ']');
            if (count > 0) {
                lserial.after('<i class="close langclose ttip_t" title="Discard Language" onclick="newformDeparture.removeLangauage(this);">&times;</i>');
                //lserial.rules("add", {required: true, number: true, messages: {required: ""}});
            }
            // else {
            // lserial.rules("add", {required: true, number: true, messages: {required: ""}});
            // }
            if (count === 4) {
                me_.notify(me_.msg_LANG.error_notify, me_.msg_POSITION.top.RIGHT, me_.msg_ERROR);
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
        ;
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
        var me = this;
        $(e).parents(".airlinesChildContainer").remove();
        if ($(".airlinesChildContainer").length <= 10)
            $("#addAirlines").removeAttr("disabled");
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
    }

    if (iNeeded === undefined || iNeeded === null) {
        optionString = airportUtil.interviewerListOptions();
        $('#InterviewerList').append(optionString);
    }

    initialiseAirlineStatics();

}

var languageString = '', airlineString = '', firstAirlineCode = '', firstAirlineDestinatons = '', fALName = '', airlineVsProps = [];
function initialiseAirlineStatics() {
    var selairport = $("#AirportList option:selected").val();
    if (selairport === "-1")
        return false;

    var isFT = false, airlineVsProps = cacheMgr.airlineList(selairport, isDeparture);
    for (var j = 0; j < airlineVsProps.length; j++, isFT = true) {
        airlineString += '<option value=' + airlineVsProps[j].id + '>' + airlineVsProps[j].name + '</option>';
        if (!isFT) {
            for (var k = 0; k < airlineVsProps[j].destinations.length; k++) {
                firstAirlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
            }
        }
    }
    //$('#destination1').append(firstAirlineDestinatons);
    $.each($("select[id^=destination]"), function(i, elem) {
        $(elem).empty().append(firstAirlineDestinatons);
    });
    //$('#airlinesDD1').append(airlineString);
    $.each($('select[id^=airlinesDD]'), function(i, elem) {
        $(elem).empty().append(airlineString);
    });
    firstAirlineCode = airlineVsProps.length === 0 ? '' : airlineVsProps[0].code;
    fALName = airlineVsProps.length === 0 ? '' : airlineVsProps[0].name;
    //$('#code1').val(firstAirlineCode);
    $.each($("input[id^=code]"), function(i, elem) {
        $(elem).val(firstAirlineCode);
    });
    //$('#airlineHeader1').text(fALName);
    $.each($("h4[id^=airlineHeader]"), function(i, elem) {
        $(elem).text(fALName);
    });

    var languageList = airportUtil.languageOptions();
    $('#language1_1').append(languageString);
}

function callChangeInterviewer() {

    var interviewerURL = "./cache/InterviewerList/InterviewerList_" + $('#AirportList').val() + ".json";
    $.getJSON(interviewerURL, function(response, status) {
        if (response && response.length > 0 && status == "success") {
            var optionString = '';
            for (var i = 0; i < response.length; i++) {
                optionString += '<option value=' + response[i].InterviewerId + '>' + response[i].InterviewerName + '</option>'
            }
            $('#InterviewerList').empty().append(optionString);
        }
    });
    initialiseAirlineStatics();
}

var table = $('#smpl_tbl').DataTable({
    "bPaginate": false,
    "bFilter": false,
    "bInfo": false
});

function callOnAirlinesChange(id) {
    var selairport = $("#AirportList option:selected").val();
    airlineVsProps = cacheMgr.airlineList(selairport, isDeparture);
    var airlineId = parseInt(document.getElementById("airlinesDD" + id).value);
    for (var j = 0; j < airlineVsProps.length; j++) {
        if (airlineId === airlineVsProps[j].id) {
            $('#code' + id).val(airlineVsProps[j].code);
            $('#airlineHeader' + id).text(airlineVsProps[j].name);
            var airlineDestinatons = '';
            for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
                airlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
            $('#destination' + id).empty().append(airlineDestinatons);
            break;
        }
    }
}

