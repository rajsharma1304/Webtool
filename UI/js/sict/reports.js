/*<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> reports.js </FileName>
 <Author> Raghavendra G.N, Akhilesh M.S, Vivek.A </Author>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
reports = {
    reportsMessage: {
        validSearch: 'Please enter a valid value for search'
    },
    tableReport: undefined,
    name: "",
    defaultVal: "-1",
    totalColumn: '',
    businessColumn: '',
    targetsColumn: '',
    distributedColumn: '',
    msg: {
        EMPTY_RECORDS: "Empty records cannot be exported.",
        RECORDS_EXPORTED: "Successfully Exported."
    },
    init: function (u) {
        var me = this;

        if (!u)
            me.initializeAirportList();
        me.initializeInterviewer(u);
        me.initializeFormType();
        me.initlializeDirection();
        me.initializeRoute();
        me.initializeFlightType();
        me.initializeAircraftType();
        me.initializeAirlines(u);
        me.changeDestination(u);

        airportUtil.initialiseDates('startDate', 'endDate');
        gebo_tips.init();
    },
    initializeAirportList: function () {
        var me = this, optionString = airportUtil.airportListOptions('reports') /*airportListOptions()*/, apParent = $("#airportlp"), apSelect = "", firstOption = "", firstVal = "", selectedId;
        //[#73728] Aircraft-Aircraft Quota Report: Date Calendar is not displaying.

        if (apParent.length > 0) {
            if (apParent.length > 0 && optionString && $.parseHTML(optionString).length === 2) {
                //[#73233] USDomestic:ORD(Chicago):Origin is displaying as Chicago O.
                firstOption = $.parseHTML(optionString)[1].innerHTML;
                if (firstOption)
                    firstOption = pageUtils.parseValue(firstOption);
                selectedId = firstVal = $.parseHTML(optionString)[1].value;
                apParent[0].innerHTML = "<input class='span12' type='text' disabled='disabled' style='display:inline-block;' value='" + firstOption + "' data-id='" + selectedId + "'/>";
                apParent.siblings("label")[0].value = selectedId;
            }
            else {
                apParent.empty().append($("<select class='span12' id='AirportList' onchange='reports.onOriginChange(this);'>")[0]);
                apSelect = $('#AirportList');
                apSelect.append(optionString);
                if (apSelect.length > 0 && apSelect[0].length === 2) {
                    apSelect[0].disabled = true;
                }
                if (apSelect.length > 0) {
                    apSelect[0].selectedIndex = 0;
                    selectedId = apSelect.val();
                }
                //if (apSelect.parent().siblings("label").text() === "Origin") {
                apSelect[0].options[0].text = "-- Please select an Airport --";
                //}
            }
            if (selectedId && selectedId !== undefined)
                cacheMgr.selectedAirport(selectedId);
        }
    },
    initializeInterviewer: function (u) {
        var me = this, optionString = "", interviewer = $('#InterviewerList');
        if (!u && interviewer.length > 0) {
            optionString = airportUtil.interviewerListOptions();
            interviewer.append(optionString);
        }
        else if (interviewer.length > 0) {
            optionString = airportUtil.buildairportInterviewerOptions();
            interviewer.empty().append(optionString);
        }
    },
    initializeFormType: function () {
        var optionString = "", form_type = $('#form_type'), host = getInstance();
        if (form_type.length > 0) {
            optionString = airportUtil.formTypeListOptions();
            form_type.append(optionString);
            if (host !== "US" && host !== "AIR") {
                form_type.val("D");
                $("#form_type").attr("disabled", "disabled");
            }
        }
    },
    initlializeDirection: function () {
        var optionString = "", direction = $('#Direction');
        if (direction.length > 0) {
            optionString = airportUtil.directionListOptions();
            direction.append(optionString);
        }
    },
    initializeRoute: function () {
        var optionString = "", route = $('#Route');
        if (route.length > 0) {
            optionString = airportUtil.routeListOptions();
            route.append(optionString);
        }
    },
    initializeFlightType: function () {
        var optionString = "", flightType = $('#flighttype');
        if (flightType.length > 0) {
            optionString = airportUtil.flightTypeListOptions();
            flightType.append(optionString);
        }
    },
    initializeAircraftType: function () {
        var optionString = "", aircraftType = $('#aircrafttype');
        if (aircraftType.length > 0) {
            optionString = airportUtil.aircraftTypeListOptions();
            aircraftType.append(optionString);
        }
    },
    initializeAirlines: function (u) {
        var data = communion, host = data.hostInstance;
        var optionsString = "",
                airline = $('#airlineList'),
                firstText = "",
                fieldworkAirport = $('#AirportList'),
                fAirportval = "", optionString = "",
                formType = $("#form_type :selected").val(),
                host = getInstance();

        if (fieldworkAirport.length > 0)
            fAirportval = fieldworkAirport.val();
        else
            fAirportval = $("#airportlp input").data("id");

        if (host !== "US" && host !== "AIR") {//Other than USI all form Types are "Departure."
            formType = "D";
            //Show airlines list to only admin users
            if (sessionStorage.roleId === '1')
                $('#eurAirlines').removeClass('no-show');
        }

        if (formType === "-1" && (fAirportval === "-1" || fAirportval !== "-1")) { // -1 -1 Combination
            if (fAirportval === "-1" && formType === "-1") {//Both F.Type && Origin as "-1" // F.Type -1 Origin -1
                optionString = airportUtil.buildairportairlineOptions();
                airline.empty().append(optionString);
            }
            else {//Orgin is any && F.Type is "-1" 
                if (!u && airline.length > 0) {
                    optionString = airportUtil.airlineOriginDestOptions();
                    airline.empty().append(optionString);
                }
                else if (airline.length > 0) {//Airport Report
                    optionString = airportUtil.buildairportairlineOptions();
                    airline.empty().append(optionString);
                }
            }
        }
        else {//form type is D/A && Origin is Airport/-1
            if (fAirportval === "-1") {//F.Type && Origin as "-1" // F.Type !-1 Origin -1
                optionString = airportUtil.buildairportairlineOptions();
                airline.empty().append(optionString);
            }
            else {
                formType = formType === "D" ? true : false;
                if (!u && airline.length > 0) {
                    optionString = airportUtil.airlineDestinationOptions(formType);
                    airline.empty().append(optionString);
                }
                else if (airline.length > 0) {//Airport Report
                    optionString = airportUtil.buildairportairlineOptions();
                    airline.empty().append(optionString);
                }
            }
        }
		//Changes for Special Airport.
		if(sessionStorage["IsSpecialUser"] && sessionStorage.IsSpecialUser === "true"){
			if(fAirportval === "-1")
			$("#airlineList option:not(:eq(0))").remove();
			else{
				optionString = airportUtil.airlineOriginDestOptions();
				airline.empty().append(optionString);
			}
			/*if(fAirportval !== "-1"){
			 airline.val().prop('disabled', true);	
			}*/
		}
        if ($('#airlineList').length > 0) {
            firstText = $('#airlineList').siblings("label").text();
            //if (firstText && /^Airline/g.test(firstText))
			//if(sessionStorage["IsSpecialUser"] && sessionStorage.IsSpecialUser === "true")
			//$('#airlineList')[0].options[0].text = '<option value="31" direction="EAST" flightId="3350" route="EURASIA" flightType="null" originId="45">Vietnam //Airlines (VN)</option>';
			//else
            $('#airlineList')[0].options[0].text = "-- Please select an " + firstText + " --";
        }

    },
    onOriginChange: function (e) {
        var me = this,
                fieldworkAirport = $('#AirportList'),
                fAirportval = "",
                reportName = "";
        if (fieldworkAirport.length > 0)
            fAirportval = fieldworkAirport.val();
        else
            fAirportval = $("#airportlp").find("input").data("id");

        cacheMgr.selectedAirport(fAirportval);
        if ($('#InterviewerList').length !== 0) {
            var optionString = airportUtil.interviewerListOptions();
            $('#InterviewerList').empty().append(optionString);
        }
        reportName = location.href.substring(location.href.lastIndexOf("/") + 1).replace(".html", "");
        if (reportName === "airportreport")
            reportName = "_5";
        else
            reportName = undefined;
        me.changeDestination(reportName);
        me.initializeAirlines(reportName);
    },
    changeDestination: function (u) {
        var optionString, destination = $("#Destination"),
                fieldworkAirport = $('#AirportList'),
                fAirportval = "",
                formType = $("#form_type :selected").val(),
                host = getInstance();
        if (fieldworkAirport.length > 0)
            fAirportval = fieldworkAirport.val();
        else
            fAirportval = $("#airportlp input").data("id");

        if (host !== "US" && host !== "AIR")//Other than USI all form Types are "Departure."
            formType = "D";
        if (destination.length > 0) {
            if (formType === "-1" && (fAirportval === "-1" || fAirportval !== "-1")) { // -1 -1 Combination
                if (fAirportval === "-1" && formType === "-1") {//Both F.Type && Origin as "-1" // F.Type -1 Origin -1
                    optionString = airportUtil.buildairportDestinationOriginOptions(true);
                    destination.empty().append(optionString);
                }
                else {//Orgin is any && F.Type is "-1" 
                    if (!u && destination.length > 0) {
                        optionString = airportUtil.airportDestOriginOptions();
                        destination.empty().append(optionString);
                    }
                    else if (destination.length > 0) {//Airport Report
                        if (sessionStorage.roleId === "1" || sessionStorage.roleId === "2" || sessionStorage.roleId === "3") {//For Admins
                            optionString = airportUtil.buildairportDestinationOriginOptions(true);
                            destination.empty().append(optionString);
                        }
                        else {
                            optionString = airportUtil.buildairportDestinationOriginOptions();
                            destination.empty().append(optionString);
                        }
                    }
                }
            }
            else {//form type is D/A && Origin is Airport/-1
                if (fAirportval === "-1") {// F.Type !-1 Origin -1
                    optionString = airportUtil.buildairportDestinationOriginOptions(true);
                    destination.empty().append(optionString);
                }
                else {
                    formType = formType === "D" ? true : false;
                    if (!u && destination.length > 0) {
                        optionString = airportUtil.airportDestinationOptions(formType);
                        destination.empty().append(optionString);
                    }
                    else if (destination.length > 0) {//Airport Report
                        if (sessionStorage.roleId === "1" || sessionStorage.roleId === "2" || sessionStorage.roleId === "3") {//For Admins
                            optionString = airportUtil.buildairportDestinationOriginOptions(true);
                            destination.empty().append(optionString);
                        }
                        else {
                            optionString = airportUtil.buildairportDestinationOriginOptions();
                            destination.empty().append(optionString);
                        }
                    }
                }
            }
            destination[0].options[0].text = "-- Please select a Destination/Origin --";
        }
    },
    onFormTypeChange: function (e) {
        var formTypeval = e.options[e.selectedIndex].value,
                me = this,
                fieldworkAirport = $('#AirportList'),
                fAirportval = "",
                reportName = "";
        if (fieldworkAirport.length > 0)
            fAirportval = fieldworkAirport.val();
        else
            fAirportval = $("#airportlp").find("input").data("id");

        cacheMgr.selectedAirport(fAirportval);
        if ($('#InterviewerList').length !== 0) {
            var optionString = airportUtil.interviewerListOptions();
            $('#InterviewerList').empty().append(optionString);
        }
        reportName = location.href.substring(location.href.lastIndexOf("/") + 1).replace(".html", "");
        if (reportName === "airportreport")
            reportName = "_5";
        else
            reportName = undefined;
        me.changeDestination(reportName);
        me.initializeAirlines(reportName);
    },
    getReportURL: function (n) {
        var url = "";
        switch (n) {
            case "_1"://Interviewers Report
                url = getJsonInfoAction(INTERVIEWERREPORT);
                break;
            case "_2"://DOD Report
                url = getJsonInfoAction(DODREPORT);
                break;
            case "_3"://Airline Report
                url = getJsonInfoAction(AIRLINEREPORT);
                break;
            case "_4"://Flight Report
                url = getJsonInfoAction(FLIGHTREPORT);
                break;
            case "_5"://Airport Report
                url = getJsonInfoAction(AIRPORTREPORT);
                break;
            case "_6"://Aircraft Report
                url = getJsonInfoAction(AIRCRAFTREPORT);
                break;
            case "_7"://Aircraft Economy/Business Qt. Report
            case "_8":
                url = getJsonInfoAction(AIRCRAFTQUOTAREPORT);
                break;
            case "_9"://AllAirline Report
                url = getJsonInfoAction(ALLAIRLINEREPORT);
                break;
            default:
                url = "";
                break;
        }
        return url;
    },
    getParentNode: function (msg, node) {
        var nodeName = '';
        switch (node) {
            case "_1"://Interviewers Report
                nodeName = msg.InterviewerReportDetails;
                break;
            case "_2"://DOD Report
                nodeName = msg.DODReportDetails;
                break;
            case "_3"://Airline Report
                nodeName = msg.AirlineReportDetails;
                break;
            case "_4"://Flight Report
                nodeName = msg.FlightReportDetails;
                break;
            case "_5"://Airport Report
                nodeName = msg.AirportReportDetails;
                break;
            case "_6"://Aircraft Report
                nodeName = msg.AircraftReportDetail;
                break;
            case "_7"://Aircraft Economy/Business Qt Report
            case "_8":
                nodeName = msg.Airlines;
                break;
            case "_9"://Airline Report
                nodeName = msg.AirlineReportDetails;
                break;
            default:
                nodeName = "";
                break;
        }
        return nodeName;
    },
    formReportURL: function (url, name) {
        var me = this, airportID, originID,
                destinationID = $("#Destination"),
                formType = $("#form_type"),
                interviewerID = $('#InterviewerList'),
                airline = $('#airlineList'),
                route = $("#Route"),
                direction = $('#Direction'),
                startDate = $("#startDate"),
                endDate = $("#endDate"),
                responseDate = me.defaultVal,
                flightType = $('#flighttype'),
                aircraftType = $('#aircrafttype');

        if ($("#airportlp").find("input").length === 0) {
            airportID = $("#airportlp").find("select").val();
            originID = $("#airportlp").find("select").val();
            if (airportID === "select" || airportID === "-1") {
                airportID = me.defaultVal;
                originID = me.defaultVal;
            }
        }
        else {
            airportID = $("#airportlp").siblings("label")[0].value;
            originID = $("#airportlp").siblings("label")[0].value;
        }
        if (destinationID && destinationID.length === 0 || destinationID.val() === "select" || destinationID.val() === "-1")
            destinationID = me.defaultVal;
        else
            destinationID = destinationID.val();
        if (formType && formType.length === 0 || (formType.val() === "select" || formType.val() === "-1"))
            formType = me.defaultVal;
        else
            formType = formType.val();
        if (interviewerID && interviewerID.length === 0 || interviewerID.val() === "-1" || interviewerID.val() === "select")
            interviewerID = me.defaultVal;
        else
            interviewerID = $('#InterviewerList').val();
        if (airline && airline.length === 0 || airline.val() === "-1" || airline.val() === "select")
            airline = me.defaultVal;
        else
            airline = $('#airlineList').val();
        if (direction && direction.length === 0 || (direction.val() === "select" || direction.val() === "-1"))
            direction = me.defaultVal;
        else
            direction = direction.val();
        if (route && route.length === 0 || (route.val() === "select" || route.val() === "-1"))
            route = me.defaultVal;
        else
            route = route.val();
        if (flightType && flightType.length === 0 || (flightType.val() === "select" || flightType.val() === "-1"))
            flightType = me.defaultVal;
        else
            flightType = flightType.val();
        if (aircraftType && aircraftType.length === 0 || (aircraftType.val() === "select" || aircraftType.val() === "-1"))
            aircraftType = me.defaultVal;
        else
            aircraftType = aircraftType.val();
        if (startDate.data('date') !== undefined)
            startDate = pageUtils.returnServerDate(startDate.data('date'));
        else
            startDate = me.defaultVal;

        if (endDate.data('date') !== undefined)
            endDate = pageUtils.returnServerDate(endDate.data('date'));
        else
            endDate = me.defaultVal;

        if (sessionStorage.roleId === '0')
            originID = sessionStorage.selectedAirportId;

        switch (name) {
            case "_1"://Interviewers Report
                url += "/" + airportID + "/" + originID + "/" + destinationID + "/" + interviewerID + "/"+ airline + "/" + formType + "/" + route + "/" + direction + "/" + flightType + "/" + aircraftType + "/" + startDate + "/" + endDate + "/" + responseDate;
                break;
            case "_2"://DOD Report
                url += "/" + airportID + "/" + originID + "/" + destinationID + "/" + airline + "/" + formType + "/" + interviewerID + "/" + route + "/" + direction + "/" + flightType + "/" + aircraftType + "/" + startDate + "/" + endDate + "/" + responseDate;
                break;
            case "_3"://Airline Report
                url += "/" + airportID + "/" + originID + "/" + destinationID + "/" + formType + "/" + interviewerID + "/" + route + "/" + direction + "/" + flightType + "/" + aircraftType + "/" + startDate + "/" + endDate + "/" + airline + "/" + responseDate;
                break;
            case "_4"://Flight
                url += "/" + airportID + "/" + originID + "/" + destinationID + "/" + airline + "/" + formType + "/" + interviewerID + "/" + route + "/" + direction + "/" + flightType + "/" + aircraftType + "/" + startDate + "/" + endDate + "/" + responseDate;
                break;
            case "_5"://Airport Report
                url += "/" + airportID + "/" + originID + "/" + destinationID + "/" + airline + "/" + formType + "/" + interviewerID + "/" + route + "/" + direction + "/" + flightType + "/" + aircraftType + "/" + startDate + "/" + endDate + "/" + responseDate;
                break;
            case "_6"://Aircraft Report
                url += "/" + airportID + "/" + originID + "/" + destinationID + "/" + formType + "/" + interviewerID + "/" + startDate + "/" + endDate + "/" + responseDate;
                break;
            case "_7"://Aircraft Economy Qt Report
                url += "/" + me.getAirportID() + "/" + startDate + "/" + endDate + "/" + false;
                break;
            case "_8"://Aircraft Business Qt Report
                url += "/" + me.getAirportID() + "/" + startDate + "/" + endDate + "/" + true;
                break;
            case "_9"://All Airline Report
                url += "/" + airportID + "/" + originID + "/" + destinationID + "/" + formType + "/" + interviewerID + "/" + route + "/" + direction + "/" + flightType + "/" + aircraftType + "/" + startDate + "/" + endDate + "/" + airline + "/" + responseDate;
                break;
            default:
                break;
        }
        return url;
    },
    getAirportID: function () {
        var me = this;
        if (sessionStorage.roleId === "1") //Cross-tab Or Mindset Users
            return me.defaultVal;
        else {
            return cacheMgr.airportList()[0].AirportId;
        }
    },
    getColumns: function (name) {
        var me = this, columns = [], data = communion, host = data.hostInstance;
        switch (name) {
            case "_1"://Interviewer
                columns = [
                    { "data": "InterviewerName", "sDefaultContent": '' },
                    { "data": "BCompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "TotalCompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Incompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Distributed", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ResponseRate", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BResponseRate", "sDefaultContent": '', "render": me.roundValueWithPerc }
                ];
                break;
            case "_2"://DOD [#72752] [Customer]:DOD report – The sequence of the columns should be as per the old web tool. Old web tool sequence
                columns = [
                    { "data": "DOD", "sDefaultContent": '' },
                    { "data": "BCompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "TotalCompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Incompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Distributed", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ResponseRate", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BResponseRate", "sDefaultContent": '', "render": me.roundValueWithPerc }
                ];
                break;
            case "_3"://Airline
                columns = [
                    { "data": "AirlineName", "width": "" },
                    { "data": "Type", "width": "", "visible": ((host === "EUR") || (host === "AIR") ? true : false), "sDefaultContent": '' },
                    { "data": "Target", "width": "", "sDefaultContent": '' },
                    { "data": "TotalCompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "BCompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ECompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "PECompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "FCCompletes", "width": "", "visible": ((host === "EUR") ? false : true), "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Incompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Distributed", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ResponseRate", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BResponseRate", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc }, //[#72776] [Customer]: Airline Report – The values in the Target Achieved and Business Class Target Achieved columns should not contain decimal values.
                    { "data": "TargetAchieved", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BTargetAchieved", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "MissingTarget", "width": "", "sDefaultContent": '' },
                    { "data": "MissingBTarget", "width": "", "sDefaultContent": '', "render": me.roundValue }
                ];
                break;
            case "_4"://Flight
                columns = [
                    { "data": "AirlineName", "sDefaultContent": '' },
                    { "data": "FlightType", "sDefaultContent": '', "visible": ((host === "EUR") ? true : false), "sDefaultContent": '' },
                    { "data": "OriginName", "sDefaultContent": '', "visible": ((host === "AIR") ? false : true), "sDefaultContent": '' },
                    { "data": "DestinationName", "sDefaultContent": '', "sDefaultContent": '' },
                    { "data": "FlightNumber", "sDefaultContent": '' },
                    { "data": "TotalCompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "BCompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Incompletes", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Distributed", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ResponseRate", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BResponseRate", "sDefaultContent": '', "render": me.roundValueWithPerc }
                ];
                break;
            case "_5"://Airport
                columns = [
                    { "data": "OriginName", "width": "10%", "sDefaultContent": '' },
                    { "data": "Type", "width": "5%", "visible": ((host === "EUR") || (host === "AIR") ? true : false), "sDefaultContent": '' },
                    { "data": "Target", "width": "5%", "sDefaultContent": '' },
                    { "data": "TotalCompletes", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "BCompletes", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Distributed", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ResponseRate", "width": "5%", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BResponseRate", "width": "5%", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "TargetAchieved", "width": "5%", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BTargetAchieved", "width": "5%", "sDefaultContent": '', "render": me.roundValueWithPerc }
                ];
                break;
            case "_6"://Aircraft
                columns = [
                    { "data": "AircraftType", "width": "10%", "sDefaultContent": '' },
                    { "data": "Target", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "BCompletes", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "TotalCompletes", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Incompletes", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Distributed", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ResponseRate", "width": "5%", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BResponseRate", "width": "5%", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "TargetAchieved", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "BTargetAchieved", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "MissingTarget", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "MissingBTarget", "width": "5%", "sDefaultContent": '', "render": me.roundValue }
                    //{ "data": "ToDistribute", "width": "5%", "sDefaultContent": '', "render": me.roundValue },
                    //{ "data": "ToBDistribute", "width": "5%", "sDefaultContent": '', "render": me.roundValue }
                ];
                break;
            case "_9"://All Airline
                columns = [
                    { "data": "AirlineName", "width": "" },
                    { "data": "Type", "width": "", "visible": ((host === "EUR") || (host === "AIR") ? true : false), "sDefaultContent": '' },
                    { "data": "Target", "width": "", "sDefaultContent": '' },
                    { "data": "TotalCompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "BCompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ECompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "PECompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "FCCompletes", "width": "", "visible": ((host === "EUR") ? false : true), "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Incompletes", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "Distributed", "width": "", "sDefaultContent": '', "render": me.roundValue },
                    { "data": "ResponseRate", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BResponseRate", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc }, //[#72776] [Customer]: Airline Report – The values in the Target Achieved and Business Class Target Achieved columns should not contain decimal values.
                    { "data": "TargetAchieved", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "BTargetAchieved", "width": "", "sDefaultContent": '', "render": me.roundValueWithPerc },
                    { "data": "MissingTarget", "width": "", "sDefaultContent": '' },
                    { "data": "MissingBTarget", "width": "", "sDefaultContent": '', "render": me.roundValue }
                ];
                break;
            default:
                columns = [];
                break;
        }
        return columns;
    },
    roundValue: function (data, type, full, meta) {
        return Math.round(data);
    },
    roundValueWithPerc: function (data, type, full, meta) {
        return reports.roundValue(data, type, full, meta) + '%';
    },
    getName: function (e) {
        return e.attr('name');
    },
    generateReport: function (name) {
        var me = this, columns, rowCount, search = true, info = true;
        //  Show the table even if we dont have the data
        $("table.display").parents('div.row-fluid').removeClass('no-show');
        columns = me.getColumns(name);
        if (columns && columns.length === 0) {//For Aircraft Quota Report
            search = false, info = false;
            columns = [{ "data": "AirlineName", "sDefaultContent": "" }];
            $.ajax({
                dataType: 'json',
                url: "./cache/AircraftTypes.json",
                async: false,
                success: function (e) {
                    if (e && e.length > 0) {
                        $("#aircrafteconomyquotaR thead th").attr("colspan", e.length);
                        $("#aircraftbusinessquotaR thead th").attr("colspan", e.length);
                    }
                }
            });
        }
        //[#73662] Aircraft:CT:Generate Quota report:Tool tip is not same as for other reports.
        try {
            me.tableReport = $("table.display").DataTable({
                "bJQueryUI": true,
                "bProcessing": true,
                "paging": false,
                "bDeferRender": false,
                "bDestroy": true,
                "searching": search,
                "info": info,
                "initComplete": function () {
                    $("input[type=search]").off().on('keyup', function (e) {
                        if (e.keyCode == 13) {
                            me.tableReport.search(this.value).draw();
                            this.value = this.value.trim();
                            if (this.value.trim() === '') {
                                pageHelper.clearStickies();
                                pageHelper.notify(reports.reportsMessage.validSearch, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);
                            }
                        }
                    });
                    if (undefined === name)
                        name = me.getName(this);
                    var rowCount = this.find("tbody tr").length;
                    //me.updateExtraDetails(this, name, rowCount);
                    gebo_tips.init();
                },
                "fnCreatedRow": function (nRow, aData, iDataIndex) {
                    var repName = name;
                    if (undefined === name)
                        repName = me.getName(this);
                    if (repName === "_7" || repName === "_8") {
                        var row = me.tableReport.row($(nRow)), data = row.data();
                        if (data !== undefined)
                            row.child(me.formatTable(data)).show();
                    }
                },
                "drawCallback": function () {
                    var repName = name, rowCount = this.find("tbody tr").length;
                    if (undefined === name)
                        repName = me.getName(this);
                    me.updateExtraDetails(this, repName, rowCount);
                },
                "ajax": function (data, callback, settings) {
                    var repName = name;
                    if (undefined === name) {
                        repName = me.getName(this);
                    }
                    $.get(me.formReportURL(me.getReportURL(repName), repName))
                            .done(function (result) {
                                var nodeName = me.getParentNode(result, repName);
                                rowCount = nodeName.length;
                                if (nodeName !== null) {
                                    if (rowCount > 0) {
                                        $('#Export').removeAttr('disabled');
                                    } else {
                                        $('#Export').attr("disabled", true);
                                    }
                                    callback({ data: nodeName });
                                } else {
                                    $('#Export').attr("disabled", true);
                                    callback({ data: [] });
                                }
                            });
                },
                "columns": columns
            });
        }
        catch (err) {
            console.log(err);
        }
        finally {
            gebo_tips.init();
        }
    },
    updateExtraDetails: function (e, name, rowCount) {
        var me = this, api = e.api(), data, cols,
                recordText = (e.find("tbody tr").text() === "No matching records found") || "No data available in table" === (e.find("tbody tr").text());
        // Remove the formatting to get integer data for summation
        var intVal = function (i) {
            return typeof i === 'string' ?
                    i.replace(/[\$,%]/g, '') * 1 :
                    typeof i === 'number' ?
                i : 0;
        };
        cols = me.getColumnCount(name);
        for (var j = 1; j < cols; j++) {
            var pageTotal = 0, arr = [];
            // Total over this page
            pageTotal = api
                    .column(j, { page: 'current' })
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0);
            /*arr = api
             .column(j, {page: 'current'})
             .data();*/
            // Update footer
            //if (arr.length > 0)
            $(api.column(j).footer()).html(
                    recordText === true ? "" : me.callFooterValue(pageTotal, j, name, rowCount)
                    );
        }
    },
    formatTable: function (data) {
        var tableHeader = "", tableData = "", table = "";
        for (var i = 0; i < data.Aircrafts.length; i++) {
            tableHeader += "<td>" + data.Aircrafts[i].Key + "</td>";
            tableData += "<td>" + data.Aircrafts[i].Value + "</td>";
        }
        table = '<table width="100%" cellspacing="0" cellpadding="0" border="0">' +
                '<tr>' + tableHeader + '</tr>' +
                '<tr>' + tableData + '</tr>' +
                '</table>';
        return table;
    },
    exportReport: function (n, isQtMode) {
        var me = this;
        if (me.tableReport && me.tableReport.rows().data().length !== 0) {
            $("#" + n).table2excel({
                exclude: ".noExl",
                name: "Report",
                dtMode: true,
                qtMode: isQtMode === undefined ? false : true,
                expandHeader: 'airlineR' === n ? true : false
            });
            pageUtils.notify(me.msg.RECORDS_EXPORTED, 'top-right', pageUtils.msg_SUCCESS);
        }
        else
            pageUtils.notify(me.msg.EMPTY_RECORDS, 'top-right', pageUtils.msg_INFO);
    },
    getColumnCount: function (name) {
        var columnCount;
        switch (name) {
            case "_1"://Interviewer
                columnCount = 7;
                break;
            case "_2"://DOD
                columnCount = 7;
                break;
            case "_3"://Airline Report
                columnCount = 16;
                break;
            case "_4"://Flight
                columnCount = 11;
                break;
            case "_5"://Airport
                columnCount = 10;
                break;
            case "_6"://Aircraft
                columnCount = 12;
                break;
            case "_9"://All Airline Report
                columnCount = 16;
                break;
            default:
                break;
        }
        return columnCount;
    },
    callFooterValue: function (total, i, name, rowCount) {
        var todtalValue = '', host = getInstance();
        if (rowCount > 0 && !isNaN(total) && total >= 0) {
            switch (name) {//DataTable Index starts with 0.
                //[#72741] [Customer]: (PVG login) – All reports : Do not show decimal values in the Grand Total row, under the Response rate% column and under the Business class % column
                case '_1'://Interviewer
                    if (i === 1)
                        reports.businessColumn = total;
                    if (i === 2)
                        reports.totalColumn = total;
                    if (i === 4)
                        reports.distributedColumn = total;
                    if (i === 5) {//Response Rate
                        todtalValue = reports.getValue(reports.totalColumn, reports.distributedColumn);
                    } else if (i === 6) {//Business class completes.
                        todtalValue = reports.getValue(reports.businessColumn, reports.totalColumn);
                    } else
                        todtalValue = Math.round(total.toFixed(2));
                    break;
                case '_2'://DOD
                    if (i === 1)
                        reports.businessColumn = total;
                    if (i === 2)
                        reports.totalColumn = total;
                    if (i === 4)
                        reports.distributedColumn = total;
                    if (i === 5) {//Response Rate
                        todtalValue = reports.getValue(reports.totalColumn, reports.distributedColumn);
                    } else if (i === 6) {//Business class completes.
                        todtalValue = reports.getValue(reports.businessColumn, reports.totalColumn);
                    } else
                        todtalValue = Math.round(total.toFixed(2));
                    break;;
                case '_3'://Airline Report
                    if (i === 2)
                        reports.targetsColumn = total;
                    if (i === 3)
                        reports.totalColumn = total;
                    if (i === 4)
                        reports.businessColumn = total;
                    if (i === 9)
                        reports.distributedColumn = total;
                    if (i === 10) {//Response Rate
                        todtalValue = reports.getValue(reports.totalColumn, reports.distributedColumn);
                    } else if (i === 11) {//Business Response Rate
                        todtalValue = reports.getValue(reports.businessColumn, reports.totalColumn);
                    } else if (i === 12) {//Target Achved
                        todtalValue = reports.getValue(reports.totalColumn, reports.targetsColumn);
                    } else if (i === 13) {//Business Target Achved
                        if (host === 'EUR')
                            var businessTargets = (1 / 4 * reports.targetsColumn);
                        else
                            var businessTargets = (1 / 3 * reports.targetsColumn);
                        todtalValue = reports.getValue(reports.businessColumn, businessTargets);
                    }
                        //                    else if (i === 14){//Missing Target
                        //                        todtalValue = reports.targetsColumn - reports.totalColumn;
                        //                    }else if (i === 15){//Missing Business
                        //                        if(host === 'EUR')
                        //                           var businessTargets = (1/4 * reports.targetsColumn);
                        //                        else  
                        //                           var businessTargets = (1/3 * reports.targetsColumn);
                        //                        todtalValue = businessTargets - reports.businessColumn;
                        //                    }
                    else
                        todtalValue = Math.round(total.toFixed(2));
                    break;
                case '_4'://Flight
                    if (i === 5)
                        reports.totalColumn = total;
                    if (i === 6)
                        reports.businessColumn = total;
                    if (i === 8)
                        reports.distributedColumn = total;
                    if (i === 1 || i === 2 || i === 3 || i === 4)
                        todtalValue = '';
                    else if (i === 9) {//Response Rate
                        todtalValue = reports.getValue(reports.totalColumn, reports.distributedColumn);
                    } else if (i === 10) {//Business Class
                        todtalValue = reports.getValue(reports.businessColumn, reports.totalColumn);
                    } else
                        todtalValue = Math.round(total.toFixed(2));
                    break;
                case '_5'://Airport // [#72874] International cross tab:No grand total for airport report.
                    if (i == 1)
                        todtalValue = '';
                    if (i === 2)
                        reports.targetsColumn = total;
                    if (i === 3)
                        reports.totalColumn = total;
                    if (i === 4)
                        reports.businessColumn = total;                   
                    if (i === 5)
                        todtalValue = reports.distributedColumn = total;
                    else if (i === 6) {//Response Rate
                        todtalValue = reports.getValue(reports.totalColumn, reports.distributedColumn);
                    } else if (i === 7) {//Business class completes.
                        todtalValue = reports.getValue(reports.businessColumn, reports.totalColumn);
                    } else if (i === 8) {//Target Achved
                        todtalValue = reports.getValue(reports.totalColumn, reports.targetsColumn);
                    } 
                    else if (i === 9) {//Business Target Achved
                        if (host === 'EUR')
                            var businessTargets = (1 / 4 * reports.targetsColumn);
                        else
                            var businessTargets = (1 / 3 * reports.targetsColumn);
                        todtalValue = reports.getValue(reports.businessColumn, businessTargets);
                    }
                    else
                        todtalValue = Math.round(total.toFixed(2));
                    break;
                case '_6'://Aircraft
                    if (i === 1)
                        reports.targetsColumn = total;
                    if (i === 2)
                        reports.businessColumn = total;
                    if (i === 3)
                        reports.totalColumn = total;
                    if (i === 5)
                        reports.distributedColumn = total;
                    if (i == 0)
                        todtalValue = '';
                    else if (i === 6) {//Response Rate
                        todtalValue = reports.getValue(reports.totalColumn, reports.distributedColumn);
                    } else if (i === 7) {//Business Class
                        todtalValue = reports.getValue(reports.businessColumn, reports.totalColumn);
                    } else if (i === 8) {//Target Achved
                        todtalValue = reports.getValue(reports.totalColumn, reports.targetsColumn);
                    } else if (i === 9) {//Business Target Achved
                        if (host === 'EUR')
                            var businessTargets = (1 / 4 * reports.targetsColumn);
                        else
                            var businessTargets = (1 / 3 * reports.targetsColumn);
                        todtalValue = reports.getValue(reports.businessColumn, businessTargets);
                    }
                        //                    else if (i === 10){//Missing Target
                        //                        todtalValue = reports.targetsColumn - reports.totalColumn;
                        //                    }else if (i === 11){//Missing Business
                        //                    if(host === 'EUR')
                        //                        var businessTargets = (1/4 * reports.targetsColumn);
                        //                    else
                        //                        var businessTargets = (1/3 * reports.targetsColumn);
                        //                        todtalValue = businessTargets - reports.businessColumn;
                        //                    }
                    else
                        todtalValue = Math.round(total.toFixed(2));
                    break;
                case '_9'://All Airline Report
                    if (i === 2)
                        reports.targetsColumn = total;
                    if (i === 3)
                        reports.totalColumn = total;
                    if (i === 4)
                        reports.businessColumn = total;
                    if (i === 9)
                        reports.distributedColumn = total;
                    if (i === 10) {//Response Rate
                        todtalValue = reports.getValue(reports.totalColumn, reports.distributedColumn);
                    } else if (i === 11) {//Business Response Rate
                        todtalValue = reports.getValue(reports.businessColumn, reports.totalColumn);
                    } else if (i === 12) {//Target Achved
                        todtalValue = reports.getValue(reports.totalColumn, reports.targetsColumn);
                    } else if (i === 13) {//Business Target Achved
                        if (host === 'EUR')
                            var businessTargets = (1 / 4 * reports.targetsColumn);
                        else
                            var businessTargets = (1 / 3 * reports.targetsColumn);
                        todtalValue = reports.getValue(reports.businessColumn, businessTargets);
                    }                       
                    else
                        todtalValue = Math.round(total.toFixed(2));
                    break;
                default:
                    debugger;
                    break;
            }
        } else {
            //[#72796] [Customer]:On the airline report for the last two columns the total is 0.
            //[#72739] [Customer]:Flight report – The Grand Total values for Origin , Destination and Flight No, columns should be made blank since we are not calculating anything here.
            if (name === "_2" && (i !== 0))//DOD
                todtalValue = '';
            if (name === "_3" && total === 0 && (i === 14 || i === 15 || i === 5 || i === 6 || i === 7))//Airline
                todtalValue = '';
            else if (name === "_3" && total < 0 && (i === 14 || i === 15 || i === 5 || i === 6 || i === 7))
                todtalValue = Math.round(total);
            if (name === "_4" && (i === 1 || i === 2 || i === 3))//Flight
                todtalValue = '';
            if (name === "_5" && (i === 0))//Airport
                todtalValue = '';
            else if (name === "_5" && total < 0 && (i === 6 || i === 7 || i === 8 || i === 9))
                todtalValue = Math.round(total);
            if (name === "_6" && (i === 10 || i === 11))//Aircraft
                todtalValue = Math.round(total);
            if (name === "_9" && total === 0 && (i === 14 || i === 15 || i === 5 || i === 6 || i === 7))//All Airline
                todtalValue = '';
            else if (name === "_9" && total < 0 && (i === 14 || i === 15 || i === 5 || i === 6 || i === 7))
                todtalValue = Math.round(total);
        }
        return (todtalValue);
    },
    getValue: function (param1, param2) {
        var value = '';
        if (param1 != '0' && param2 != '0')
            value = Math.round((param1 / param2) * 100);
        else
            value = 0;

        value = isNaN(value) ? 0 + '%' : value + '%';
        return value;
    }
};