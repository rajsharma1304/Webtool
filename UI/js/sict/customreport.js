/*<Copyright> Cross-Tab </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> reports.js </FileName>
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
        me.initializeFormType();
        me.initializeRoute();
        me.initializeReportType();
        me.initializeAircraftType();
        me.initializeFlightType();

        airportUtil.initialiseDates('startDate', 'endDate');
        airportUtil.initialiseDateSingle('ResponseDate');

        gebo_tips.init();
    },
    initializeDestinationList: function () {
        var me = this, optionString = "", airportList = $('#Destination');
        if (airportList.length > 0) {
            optionString = airportUtil.airportListOptions('reports');
            airportList.append(optionString);
        }
        $("#Destination").chosen({ 'width': '100%' }).trigger("chosen:updated");
    },
    initializeAirportList: function () {
        var me = this, optionString = "", airportList = $('#airportList');
        if (airportList.length > 0) {
            optionString = airportUtil.airportListOptions('reports');
            airportList.append(optionString);
        }
        $("#airportList").chosen({ 'width': '100%' }).trigger("chosen:updated");
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
        $("#InterviewerList").chosen({ 'width': '100%' }).trigger("chosen:updated");

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
        $("#Direction").chosen({ 'width': '100%' }).trigger("chosen:updated");

    },
    initializeRoute: function () {
        var optionString = "", route = $('#Route');
        if (route.length > 0) {
            optionString = airportUtil.routeListOptions();
            route.append(optionString);
        }
        $("#Route").chosen({ 'width': '100%' }).trigger("chosen:updated");

    },
    initializeFlightType: function () {
        var optionString = "", flightType = $('#flighttype');
        if (flightType.length > 0) {
            optionString = airportUtil.flightTypeListOptions();
            flightType.append(optionString);
        }
        $("#flighttype").chosen({ 'width': '100%' }).trigger("chosen:updated");
    },
    initializeAircraftType: function () {
        var optionString = "", aircraftType = $('#aircrafttype');
        if (aircraftType.length > 0) {
            optionString = airportUtil.aircraftTypeListOptions();
            aircraftType.append(optionString);
        }
        $("#aircrafttype").chosen({ 'width': '100%' }).trigger("chosen:updated");
    },
    initializeReportType: function () {
        var optionString = "", report_type = $('#report_type');
        if (report_type.length > 0) {
            optionString = airportUtil.reportTypeListOptions();
            report_type.append(optionString);
        }
        $("#report_type").chosen({ 'width': '100%' }).trigger("chosen:updated");
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

        fAirportval = "-1";

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

        if ($('#airlineList').length > 0) {
            firstText = $('#airlineList').siblings("label").text();
            //if (firstText && /^Airline/g.test(firstText))
            $('#airlineList')[0].options[0].text = "-- Please select an " + firstText + " --";
        }
        $("#airlineList").chosen({ 'width': '100%' }).trigger("chosen:updated");

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
            $("#Destination").chosen({ 'width': '100%' }).trigger("chosen:updated");

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
            default:
                nodeName = "";
                break;
        }
        return nodeName;
    },
    formReportURL: function (url, name, airportID, route, aircraftType, flightType) {
        var me = this, originID,
                      destinationID = me.defaultVal,
                      formType = $("#form_type"),
                      interviewerID = me.defaultVal,
                      airline = me.defaultVal,
                      direction = me.defaultVal,
                      startDate = $("#startDate"),
                      endDate = $("#endDate"),
                      responseDate = $("#ResponseDate"),
 
        originID = airportID;
     
        if (formType && formType.length === 0 || (formType.val() === "select" || formType.val() === "-1"))
            formType = me.defaultVal;
        else
            formType = formType.val();

        if (startDate.data('date') !== undefined)
            startDate = pageUtils.returnServerDate(startDate.data('date'));
        else
            startDate = me.defaultVal;

        if (endDate.data('date') !== undefined)
            endDate = pageUtils.returnServerDate(endDate.data('date'));
        else
            endDate = me.defaultVal;

        if (responseDate.data('date') !== undefined)
            responseDate = pageUtils.returnServerDate(responseDate.data('date'));
        else
            responseDate = me.defaultVal;

        switch (name) {
            case "_1"://Interviewers Report
                url += "/" + airportID + "/" + originID + "/" + destinationID +  "/" + interviewerID + "/" + airline + "/" + formType + "/" + route + "/" + direction + "/" + flightType + "/" + aircraftType + "/" + startDate + "/" + endDate + "/" + responseDate;
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
    getColumns: function (JsonData, name,j) {
        var me = this, columns = [], data = communion, host = data.hostInstance, columnname, responseDate = $("#ResponseDate");
        if (responseDate.data('date') !== undefined)
            responseDate = pageUtils.returnServerDate(responseDate.data('date'));
        else
            responseDate = me.defaultVal;

        switch (name) {
            case "_1"://Interviewer
                if (j == -1) {
                    columns[0] = "Interviewer Name";
                    columns[1] = "Business Class Completes";
                    columns[2] = "Total Completes";
                    columns[3] = "Incompletes";
                    columns[4] = "Distributed";
                    columns[5] = "Response Rate %";
                    columns[6] = "Business Class %";
                }
                else if (j == -2) {
                    var tBCompletes = 0, tTotalCompletes = 0, tIncompletes = 0, tDistributed = 0, tResponseRate = 0, tBResponseRate = 0, tRCardsDistributed = 0, tRTotalCompletes=0;

                    $.each(JsonData, function () {
                        tBCompletes += this.BCompletes;
                        tTotalCompletes += this.TotalCompletes;
                        tIncompletes += this.Incompletes;
                        tDistributed += this.Distributed;
                        tRCardsDistributed += this.RCardsDistributed;
                        tRTotalCompletes += this.RTotalCompletes;
                    });

                    if (responseDate == '-1') 
                        tResponseRate = me.getValue(tTotalCompletes, tDistributed);
                    else
                        tResponseRate = me.getValue(tRTotalCompletes, tRCardsDistributed);

                    tBResponseRate = me.getValue(tBCompletes, tTotalCompletes);

                    columns[0] = "Grand Total:String";
                    columns[1] = me.roundValue(tBCompletes) + ":Number";
                    columns[2] = me.roundValue(tTotalCompletes) + ":Number";
                    columns[3] = me.roundValue(tIncompletes) + ":Number";
                    columns[4] = me.roundValue(tDistributed) + ":Number";
                    columns[5] = tResponseRate + ":String";
                    columns[6] = tBResponseRate + ":String";
                }
                else
                {
                    columns[0] = JsonData.InterviewerName+":String";
                    columns[1] = me.roundValue(JsonData.BCompletes) + ":Number";
                    columns[2] = me.roundValue(JsonData.TotalCompletes) + ":Number";
                    columns[3] = me.roundValue(JsonData.Incompletes) + ":Number";
                    columns[4] = me.roundValue(JsonData.Distributed) + ":Number";
                    columns[5] = me.roundValueWithPerc(JsonData.ResponseRate) + ":String";
                    columns[6] = me.roundValueWithPerc(JsonData.BResponseRate) + ":String";
                }
                break;
            case "_2"://DOD [#72752] [Customer]:DOD report – The sequence of the columns should be as per the old web tool. Old web tool sequence
  
                if (j == -1) {
                    columns[0] = "Date of Distribution";
                    columns[1] = "Business Class Completes";
                    columns[2] = "Total Completes";
                    columns[3] = "Incompletes";
                    columns[4] = "Distributed";
                    columns[5] = "Response Rate %";
                    columns[6] = "Business Class %";
                }
                else if (j == -2) {
                    var tBCompletes = 0, tTotalCompletes = 0, tIncompletes = 0, tDistributed = 0, tResponseRate = 0, tBResponseRate = 0, tRCardsDistributed = 0, tRTotalCompletes = 0;

                    $.each(JsonData, function () {
                        tBCompletes += this.BCompletes;
                        tTotalCompletes += this.TotalCompletes;
                        tIncompletes += this.Incompletes;
                        tDistributed += this.Distributed;
                        tRCardsDistributed += this.RCardsDistributed;
                        tRTotalCompletes += this.RTotalCompletes;
                    });

                    if (responseDate == '-1')
                        tResponseRate = me.getValue(tTotalCompletes, tDistributed);
                    else
                        tResponseRate = me.getValue(tRTotalCompletes, tRCardsDistributed);

                    tBResponseRate = me.getValue(tBCompletes, tTotalCompletes);

                    columns[0] = "Grand Total:String";
                    columns[1] = me.roundValue(tBCompletes) + ":Number";
                    columns[2] = me.roundValue(tTotalCompletes) + ":Number";
                    columns[3] = me.roundValue(tIncompletes) + ":Number";
                    columns[4] = me.roundValue(tDistributed) + ":Number";
                    columns[5] = tResponseRate + ":String"
                    columns[6] = tBResponseRate + ":String"
                }
                else {
                    columns[0] = JsonData.DOD + ":String"
                    columns[1] = me.roundValue(JsonData.BCompletes) + ":Number";
                    columns[2] = me.roundValue(JsonData.TotalCompletes) + ":Number";
                    columns[3] = me.roundValue(JsonData.Incompletes) + ":Number";
                    columns[4] = me.roundValue(JsonData.Distributed) + ":Number";
                    columns[5] = me.roundValueWithPerc(JsonData.ResponseRate) + ":String"
                    columns[6] = me.roundValueWithPerc(JsonData.BResponseRate) + ":String"
                }

                break;
            case "_3"://Airline
                var k = 1;
                if (j == -1) {
                    columns[0] = "AirlineName";
                    if ((host === "EUR") || (host === "AIR")) {
                        columns[k++] = "Type";
                    }
                    columns[k++] = "Target";
                    columns[k++] = "Total Completes";
                    columns[k++] = "Business Class Completes";
                    columns[k++] = "Economy Class Completes";
                    columns[k++] = "Premium Economy Class Completes";
                    if (host != "EUR") {
                        columns[k++] = "First Class Completes";
                    }
                    columns[k++] = "Incompletes";
                    columns[k++] = "Distributed";
                    columns[k++] = "Response Rate %";
                    columns[k++] = "Business Class %";
                    columns[k++] = "Target Achieved";
                    columns[k++] = "Business Target Achieved";
                    columns[k++] = "Missing Target";
                    columns[k++] = "Missing Business";
                }
                else if (j == -2) {
                    var tTarget = 0, tTotalCompletes = 0, tBCompletes = 0, tECompletes = 0, tPECompletes = 0, tFCCompletes = 0, tIncompletes = 0, tDistributed = 0, tResponseRate = 0,
                tBResponseRate = 0, tTargetAchieved = 0, tBTargetAchieved = 0, tMissingTarget = 0, tMissingBTarget = 0, tRCardsDistributed = 0, tRTotalCompletes = 0;

                    $.each(JsonData, function () {
                        tTarget += this.Target;
                        tTotalCompletes += this.TotalCompletes;
                        tBCompletes += this.BCompletes;
                        tECompletes += this.ECompletes;
                        tPECompletes += this.PECompletes;
                        tFCCompletes += this.FCCompletes;
                        tIncompletes += this.Incompletes;
                        tDistributed += this.Distributed;
                        tMissingTarget += this.MissingTarget;
                        tMissingBTarget += this.MissingBTarget;
                        tRCardsDistributed += this.RCardsDistributed;
                        tRTotalCompletes += this.RTotalCompletes;
                    });

                    if (responseDate == '-1')
                        tResponseRate = me.getValue(tTotalCompletes, tDistributed);
                    else
                        tResponseRate = me.getValue(tRTotalCompletes, tRCardsDistributed);

                    tBResponseRate = me.getValue(tBCompletes, tTotalCompletes);
                    tTargetAchieved = me.getValue(tTotalCompletes, tTarget);
                    var businessTargets = 0;

                    if (host === 'EUR')
                        businessTargets = (1 / 4 * tTarget);
                    else
                        businessTargets = (1 / 3 * tTarget);

                    tBTargetAchieved = me.getValue(tBCompletes, businessTargets);

                    columns[0] = "Grand Total:String";
                    if ((host === "EUR") || (host === "AIR")) {
                        columns[k++] =  " :String";
                    }
                    columns[k++] = tTarget + ":String";
                    columns[k++] = me.roundValue(tTotalCompletes) + ":Number";
                    columns[k++] = me.roundValue(tBCompletes) + ":Number";
                    columns[k++] = me.roundValue(tECompletes) + ":Number";
                    columns[k++] = me.roundValue(tPECompletes) + ":Number";
                    if (host != "EUR") {
                        columns[k++] = me.roundValue(tFCCompletes) + ":Number";
                    }
                    columns[k++] = me.roundValue(tIncompletes) + ":Number";
                    columns[k++] = me.roundValue(tDistributed) + ":Number";
                    columns[k++] = tResponseRate + ":String";
                    columns[k++] = tBResponseRate + ":String";
                    columns[k++] = tTargetAchieved + ":String";
                    columns[k++] = tBTargetAchieved + ":String";
                    columns[k++] = me.roundValue(tMissingTarget) + ":String";
                    columns[k++] = me.roundValue(tMissingBTarget) + ":String";

                }
                else {
                    columns[0] = JsonData.AirlineName + ":String";
                    if ((host === "EUR") || (host === "AIR")) {
                        columns[k++] = JsonData.Type + ":String";
                    }
                    columns[k++] = JsonData.Target + ":Number";
                    columns[k++] = me.roundValue(JsonData.TotalCompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.BCompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.ECompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.PECompletes) + ":Number";
                    if (host != "EUR") {
                        columns[k++] = me.roundValue(JsonData.FCCompletes) + ":Number";
                    }
                    columns[k++] = me.roundValue(JsonData.Incompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.Distributed) + ":Number";
                    columns[k++] = me.roundValueWithPerc(JsonData.ResponseRate) + ":String";
                    columns[k++] = me.roundValueWithPerc(JsonData.BResponseRate) + ":String";
                    columns[k++] = me.roundValueWithPerc(JsonData.TargetAchieved) + ":String";
                    columns[k++] = me.roundValueWithPerc(JsonData.BTargetAchieved) + ":String";
                    columns[k++] = me.roundValue(JsonData.MissingTarget) + ":Number";
                    columns[k++] = me.roundValue(JsonData.MissingBTarget) + ":Number";
                }

                break;
            case "_4"://Flight
                var k = 1;
                if (j == -1) {
                    columns[0] = "AirlineName";
                    if (host === "EUR")
                        columns[k++] = "FlightType";
                    if (host != "AIR")
                        columns[k++] = "Origin";

                    columns[k++] = "Destination";
                    columns[k++] = "Flight Number";
                    columns[k++] = "Total Completes";
                    columns[k++] = "Business Class Completes";
                    columns[k++] = "Incompletes";
                    columns[k++] = "Distributed";
                    columns[k++] = "Response Rate %";
                    columns[k++] = "Business Class %";
                }
                else if (j == -2) {
                    var tBCompletes = 0, tTotalCompletes = 0, tIncompletes = 0, tDistributed = 0, tResponseRate = 0, tBResponseRate = 0, tRCardsDistributed = 0, tRTotalCompletes = 0;

                    $.each(JsonData, function () {
                        tBCompletes += this.BCompletes;
                        tTotalCompletes += this.TotalCompletes;
                        tIncompletes += this.Incompletes;
                        tDistributed += this.Distributed;
                        tRCardsDistributed += this.RCardsDistributed;
                        tRTotalCompletes += this.RTotalCompletes;
                    });

                    if (responseDate == '-1')
                        tResponseRate = me.getValue(tTotalCompletes, tDistributed);
                    else
                        tResponseRate = me.getValue(tRTotalCompletes, tRCardsDistributed);

                    tBResponseRate = me.getValue(tBCompletes, tTotalCompletes);

                    columns[0] = "Grand Total:String";
                    if (host === "EUR")
                        columns[k++] = " :String";
                    if (host != "AIR")
                        columns[k++] = " :String";

                    columns[k++] =  " :String";
                    columns[k++] =  " :String";
                    columns[k++] = me.roundValue(tTotalCompletes) + ":Number";
                    columns[k++] = me.roundValue(tBCompletes) + ":Number";
                    columns[k++] = me.roundValue(tIncompletes) + ":Number";
                    columns[k++] = me.roundValue(tDistributed) + ":Number";
                    columns[k++] = tResponseRate + ":String";
                    columns[k++] = tBResponseRate + ":String";
                }
                else {
                    columns[0] = JsonData.AirlineName + ":String";
                    if (host === "EUR")
                        columns[k++] = JsonData.FlightType + " :String";
                    if (host != "AIR")
                        columns[k++] = JsonData.OriginName + " :String";

                    columns[k++] = JsonData.DestinationName + ":String";
                    columns[k++] = JsonData.FlightNumber + ":String";
                    columns[k++] = me.roundValue(JsonData.TotalCompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.BCompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.Incompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.Distributed) + ":Number";
                    columns[k++] = me.roundValueWithPerc(JsonData.ResponseRate) + ":String";
                    columns[k++] = me.roundValueWithPerc(JsonData.BResponseRate) + ":String";

                }
                break;
            case "_5"://Airport
                var k = 1;
                if (j == -1) {
                    columns[0] = "Fieldwork Airport";
                    if ((host === "EUR") || (host === "AIR")) {
                        columns[k++] = "Type";
                    }
                    columns[k++] = "Target";
                    columns[k++] = "Total Completes";
                    columns[k++] = "Business Class Completes";
                    columns[k++] = "Distributed";
                    columns[k++] = "Response Rate %";
                    columns[k++] = "Business Class %";
                    columns[k++] = "Target Achieved";
                    columns[k++] = "Business Target Achieved";
                }
                else if (j == -2) {
                    var tTarget = 0, tBCompletes = 0, tTotalCompletes = 0, tDistributed = 0, tResponseRate = 0, tBResponseRate = 0, tRCardsDistributed = 0, tRTotalCompletes = 0, tTargetAchieved = 0, tBTargetAchieved = 0;

                    $.each(JsonData, function () {
                        tTarget += this.Target;
                        tBCompletes += this.BCompletes;
                        tTotalCompletes += this.TotalCompletes;
                        tDistributed += this.Distributed;
                        tRCardsDistributed += this.RCardsDistributed;
                        tRTotalCompletes += this.RTotalCompletes;
                    });

                    if (responseDate == '-1')
                        tResponseRate = me.getValue(tTotalCompletes, tDistributed);
                    else
                        tResponseRate = me.getValue(tRTotalCompletes, tRCardsDistributed);

                    tBResponseRate = me.getValue(tBCompletes, tTotalCompletes);

                    tTargetAchieved = me.getValue(tTotalCompletes, tTarget);
                    var businessTargets = 0;

                    if (host === 'EUR')
                        businessTargets = (1 / 4 * tTarget);
                    else
                        businessTargets = (1 / 3 * tTarget);

                    tBTargetAchieved = me.getValue(tBCompletes, businessTargets);

                    columns[0] = "Grand Total:String";
                    if ((host === "EUR") || (host === "AIR")) {
                        columns[k++] = " :String";
                    }
                    columns[k++] = tTarget + ":Number";
                    columns[k++] = me.roundValue(tTotalCompletes) + ":Number";
                    columns[k++] = me.roundValue(tBCompletes) + ":Number";
                    columns[k++] = me.roundValue(tDistributed) + ":Number";
                    columns[k++] = tResponseRate + ":String";
                    columns[k++] = tBResponseRate + ":String";
                    columns[k++] = tTargetAchieved + ":String";
                    columns[k++] = tBTargetAchieved + ":String";
                }
                else {
                    columns[0] = JsonData.OriginName + ":String";
                    if ((host === "EUR") || (host === "AIR")) {
                        columns[k++] = JsonData.Type + ":String";
                    }

                    columns[k++] = JsonData.Target + ":Number";
                    columns[k++] = me.roundValue(JsonData.TotalCompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.BCompletes) + ":Number";
                    columns[k++] = me.roundValue(JsonData.Distributed) + ":Number";
                    columns[k++] = me.roundValueWithPerc(JsonData.ResponseRate) + ":String";
                    columns[k++] = me.roundValueWithPerc(JsonData.BResponseRate) + ":String";
                    columns[k++] = me.roundValueWithPerc(JsonData.TargetAchieved) + ":String";
                    columns[k++] = me.roundValueWithPerc(JsonData.BTargetAchieved) + ":String";
                }
                break;
            case "_6"://Aircraft
                if (j == -1) {
                    columns[0] = "AircraftType";
                    columns[1] = "Target";
                    columns[2] = "Total Completes";
                    columns[3] = "Business Class Completes";                   
                    columns[4] = "Incompletes";
                    columns[5] = "Distributed";
                    columns[6] = "Response Rate %";
                    columns[7] = "Business Class %";
                    columns[8] = "Target Achieved";
                    columns[9] = "Business Target Achieved";
                    columns[10] = "Missing Target";
                    columns[11] = "Missing Business";
                }
                else if (j == -2) {
                    var tTarget = 0, tTotalCompletes = 0, tBCompletes = 0, tIncompletes = 0, tDistributed = 0, tResponseRate = 0,
           tBResponseRate = 0, tTargetAchieved = 0, tBTargetAchieved = 0, tMissingTarget = 0, tMissingBTarget = 0, tRCardsDistributed = 0, tRTotalCompletes = 0;

                    $.each(JsonData, function () {
                        tTarget += this.Target;
                        tTotalCompletes += this.TotalCompletes;
                        tBCompletes += this.BCompletes;
                        tIncompletes += this.Incompletes;
                        tDistributed += this.Distributed;
                        tMissingTarget += this.MissingTarget;
                        tMissingBTarget += this.MissingBTarget;
                        tRCardsDistributed += this.RCardsDistributed;
                        tRTotalCompletes += this.RTotalCompletes;
                    });

                    if (responseDate == '-1')
                        tResponseRate = me.getValue(tTotalCompletes, tDistributed);
                    else
                        tResponseRate = me.getValue(tRTotalCompletes, tRCardsDistributed);

                    tBResponseRate = me.getValue(tBCompletes, tTotalCompletes);
                    tTargetAchieved = me.getValue(tTotalCompletes, tTarget);
                    var businessTargets = 0;

                    if (host === 'EUR')
                        businessTargets = (1 / 4 * tTarget);
                    else
                        businessTargets = (1 / 3 * tTarget);

                    tBTargetAchieved = me.getValue(tBCompletes, businessTargets);

                    columns[0] = "Grand Total:String";
                    columns[1] = tTarget + ":Number";
                    columns[2] = me.roundValue(tTotalCompletes) + ":Number";
                    columns[3] = me.roundValue(tBCompletes) + ":Number";
                    columns[4] = me.roundValue(tIncompletes) + ":Number";
                    columns[5] = me.roundValue(tDistributed) + ":Number";
                    columns[6] = tResponseRate + ":String";
                    columns[7] = tBResponseRate + ":String";
                    columns[8] = tTargetAchieved + ":String";
                    columns[9] = tBTargetAchieved + ":String";
                    columns[10] = me.roundValue(tMissingTarget) + ":Number";
                    columns[11] = me.roundValue(tMissingBTarget) + ":Number";
                }
                else {
                    columns[0] = JsonData.AircraftType + ":String";
                    columns[1] = JsonData.Target + ":Number";
                    columns[2] = me.roundValue(JsonData.TotalCompletes) + ":Number";
                    columns[3] = me.roundValue(JsonData.BCompletes) + ":Number";
                    columns[4] = me.roundValue(JsonData.Incompletes) + ":Number";
                    columns[5] = me.roundValue(JsonData.Distributed) + ":Number";
                    columns[6] = me.roundValueWithPerc(JsonData.ResponseRate) + ":String";
                    columns[7] = me.roundValueWithPerc(JsonData.BResponseRate) + ":String";
                    columns[8] = me.roundValueWithPerc(JsonData.TargetAchieved) + ":String";
                    columns[9] = me.roundValueWithPerc(JsonData.BTargetAchieved) + ":String";
                    columns[10] = me.roundValue(JsonData.MissingTarget) + ":Number";
                    columns[11] = me.roundValue(JsonData.MissingBTarget) + ":Number";
                }
                break;
            default:
                columnname = '';
                break;
        }
        return columns;
    },
    roundValue: function (data) {
        return Math.round(data);
    },
    roundValueWithPerc: function (data) {
        return reports.roundValue(data) + '%';
    },
    getName: function (e) {
        return e.attr('name');
    },
    exportcustomReport: function () {
        var me = this, data = communion, host = data.hostInstance, airportID = $("#airportList"), airportName = $("#airportList"), route = me.defaultVal,
            aircrafttype = me.defaultVal, flighttype = me.defaultVal,
        responseDate = $("#ResponseDate"), report_type = $('#report_type'), report_typename = $('#report_type');;
          
        if (airportID && airportID.length === 0 || airportID.val() === "select" || airportID.val() === null) {
            airportID = me.defaultVal;
            airportName = me.defaultVal;
        }
        else {
            airportID = ('#airportList :selected').length > 0 ? '' + $('#airportList').val().join(',') + '' : "";
     
            var selMulti = $.map($("#airportList option:selected"), function (el, i) {
                return $(el).text();
            });
            airportName = ('#airportList :selected').length > 0 ? selMulti.join(",") : "";

        }
        if (host === 'AIR') {
            aircrafttype = $("#aircrafttype");
            if (aircrafttype && aircrafttype.length === 0 || (aircrafttype.val() === "select" || aircrafttype.val() === null))
                aircrafttype = me.defaultVal;
            else
                aircrafttype = ('#aircrafttype :selected').length > 0 ? '' + $('#aircrafttype').val().join(',') + '' : "";
        }
        else if (host === 'EUR') {
            flighttype = $("#flighttype");
            if (flighttype && flighttype.length === 0 || (flighttype.val() === "select" || flighttype.val() === null))
                flighttype = me.defaultVal;
            else
                flighttype = ('#flighttype :selected').length > 0 ? '' + $('#flighttype').val().join(',') + '' : "";
        }
        else {
            route = $("#Route");
            if (route && route.length === 0 || (route.val() === "select" || route.val() === null))
                route = me.defaultVal;
            else
                route = ('#Route :selected').length > 0 ? '' + $('#Route').val().join(',') + '' : "";
        }

        if (responseDate.data('date') !== undefined)
            responseDate = pageUtils.returnServerDate(responseDate.data('date'));
        else
            responseDate = me.defaultVal;

        if (report_type && report_type.length === 0 || (report_type.val() === "select" || report_type.val() === null)) {
            report_type = me.defaultVal;
            report_typename = me.defaultVal;
        }
        else {
            report_type = ('#report_type :selected').length > 0 ? '' + $('#report_type').val().join(',') + '' : "";

            var selMulti = $.map($("#report_type option:selected"), function (el, i) {
                return $(el).text();
            });
            report_typename = ('#report_type :selected').length > 0 ? selMulti.join(",") : "";

        }
        if (sessionStorage.roleId === '0')
            originID = sessionStorage.selectedAirportId;

        var airportIDArr = airportID.split(",");
        var airportNameArr = airportName.split(",");

        
        var report_typeArr = report_type.split(",");
        var report_typenameArr = report_typename.split(",");
       
        for (var i = 0; i < airportIDArr.length; i++) {
            var TotalResult = [];
            var ReportType = [];
            var SheetName = [];
            var a = 0;

            for (var j = 0; j < report_typeArr.length; j++) {
                if (host === 'AIR') {
                    var aircrafttypeArr = aircrafttype.split(",");
                    for (var k = 0; k < aircrafttypeArr.length; k++) {
                        var url = me.formReportURL(me.getReportURL(report_typeArr[j]), report_typeArr[j], airportIDArr[i], route, aircrafttypeArr[k], flighttype);
                        $.ajax({
                            dataType: 'json',
                            url: url,
                            async: false,
                            success: function (result) {
                                var ReportTitle = airportNameArr[i] + '_' + report_typenameArr[j] + '_' + aircrafttypeArr[k];

                                TotalResult[a] = result;
                                ReportType[a] = report_typeArr[j];
                                SheetName[a] = ReportTitle;
                                a++;
                            }
                        });
                    }
                }
                else if (host === 'EUR') {
                    var flighttypeArr = flighttype.split(",");
                    for (var k = 0; k < flighttypeArr.length; k++) {
                        var url = me.formReportURL(me.getReportURL(report_typeArr[j]), report_typeArr[j], airportIDArr[i], route, aircrafttype, flighttypeArr[k]);
                        $.ajax({
                            dataType: 'json',
                            url: url,
                            async: false,
                            success: function (result) {
                                var ReportTitle = airportNameArr[i] + '_' + report_typenameArr[j] + '_' + flighttypeArr[k];

                                TotalResult[a] = result;
                                ReportType[a] = report_typeArr[j];
                                SheetName[a] = ReportTitle;
                                a++;
                            }
                        });
                    }
                }
                else {
                    var routeArr = route.split(",");
                    for (var k = 0; k < routeArr.length; k++) {
                        var url = me.formReportURL(me.getReportURL(report_typeArr[j]), report_typeArr[j], airportIDArr[i], routeArr[k], aircrafttype, flighttype);
                        $.ajax({
                            dataType: 'json',
                            url: url,
                            async: false,
                            success: function (result) {
                                var ReportTitle = airportNameArr[i] + '_' + report_typenameArr[j] + '_' + routeArr[k];

                                TotalResult[a] = result;
                                ReportType[a] = report_typeArr[j];
                                SheetName[a] = ReportTitle;
                                a++;
                            }
                        });
                    }
                }
            }
            me.DownloadJSONToExcel(TotalResult,ReportType, SheetName, airportNameArr[i], 'Excel');
        }
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
                columnCount = 7;
                break;
            case "_6"://Aircraft
                columnCount = 12;
                break;
            default:
                break;
        }
        return columnCount;
    },
    getValue: function (param1, param2) {
        var value = '';
        if (param1 != '0' && param2!='0')
            value = Math.round((param1 / param2) * 100);
        else
            value=0;

        value = isNaN(value) ? 0 + '%' : value + '%';
        return value;
    },
    DownloadJSONToExcel: function (tables, ReportType, wsnames, ReportTitle, appname) {
        var uri = 'data:application/vnd.ms-excel;base64,'
        , tmplWorkbookXML = '<?xml version="1.0"?><?mso-application progid="Excel.Sheet"?><Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">'
          + '<DocumentProperties xmlns="urn:schemas-microsoft-com:office:office"><Author>Axel Richter</Author><Created>{created}</Created></DocumentProperties>'
          + '{worksheets}</Workbook>'
        , tmplWorksheetXML = '<Worksheet ss:Name="{nameWS}"><Table>{rows}</Table></Worksheet>'
        , tmplCellXML = '<Cell><Data ss:Type="{nameType}">{data}</Data></Cell>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }

        var ctx = "";
        var workbookXML = "";
        var worksheetsXML = "";
        var rowsXML = "";
        var me = this;
        //Generate a file name

        var fileName = "";

        //this will remove the blank-spaces from the title and replace it with an underscore
        fileName += ReportTitle.replace(/ /g, "_");

        var dataType = "";
        var dataStyle = "";
        var dataFormula = null;

        for (var i = 0; i < tables.length; i++) {
            var JSONData = me.getParentNode(tables[i], ReportType[i]);

            var RowHeader = me.getColumns(JSONData, ReportType[i], '-1');
            if (RowHeader != null) {
                rowsXML += '<Row>'
                for (var k = 0; k < RowHeader.length; k++) {
                    var dataValue = RowHeader[k];
                    //dataFormula = (dataFormula) ? dataFormula : (appname == 'Calc' && dataType == 'DateTime') ? dataValue : null;
                    ctx = { nameType: (dataType == 'Number' || dataType == 'DateTime' || dataType == 'Boolean' || dataType == 'Error') ? dataType : 'String', data: dataValue };
                    rowsXML += format(tmplCellXML, ctx);
                }
                rowsXML += '</Row>'
            }
            if (JSONData != null) {
                for (var j = 0; j < JSONData.length; j++) {
                    var RowArray = me.getColumns(JSONData[j], ReportType[i], j);
                    rowsXML += '<Row>'
                    for (var k = 0; k < RowArray.length; k++) {

                        var Cellvalue = RowArray[k].split(":");
                        var dataValue = Cellvalue[0];
                        dataType = Cellvalue[1];
                        if (dataType == 'Number' && dataValue == '')
                            dataValue = 0;

                        ctx = { nameType: (dataType == 'Number' || dataType == 'DateTime' || dataType == 'Boolean' || dataType == 'Error') ? dataType : 'String', data: dataValue };
                        rowsXML += format(tmplCellXML, ctx);
                    }
                    rowsXML += '</Row>'
                }
                var RowTotal = me.getColumns(JSONData, ReportType[i], '-2');
                if (RowTotal != null) {
                    rowsXML += '<Row>'
                    for (var k = 0; k < RowTotal.length; k++) {
                        var Cellvalue = RowTotal[k].split(":");
                        var dataValue = Cellvalue[0];
                        dataType = Cellvalue[1];
                        if (dataType == 'Number' && dataValue == '')
                            dataValue = 0;

                        ctx = { nameType: (dataType == 'Number' || dataType == 'DateTime' || dataType == 'Boolean' || dataType == 'Error') ? dataType : 'String', data: dataValue };
                        rowsXML += format(tmplCellXML, ctx);
                    }
                    rowsXML += '</Row>'
                }
            }
            ctx = { rows: rowsXML, nameWS: wsnames[i] || 'Sheet' + i };
            worksheetsXML += format(tmplWorksheetXML, ctx);
            rowsXML = "";
        }
        ctx = { created: (new Date()).getTime(), worksheets: worksheetsXML };
        workbookXML = format(tmplWorkbookXML, ctx);

        var link = document.createElement("A");
        link.href = uri + base64(workbookXML);
        link.download = fileName + ".xls";
        link.target = '_blank';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

    }
};