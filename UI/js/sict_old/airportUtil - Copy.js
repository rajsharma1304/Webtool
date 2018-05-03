/*<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> airportutil.js </FileName>
 <Author> Raghavendra G.N, Akhilesh M.S, Vivek.A </Author>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var airportUtil = {
    languageOptions: function() {
        var languageList = cacheMgr.languagesList(), languageString = '<option value="-1">-- Please select a language --</option>';
        if (languageList && languageList.length > 0) {
            for (var a = 0; a < languageList.length; a++) {
                languageString += '<option value=' + languageList[a].LanguageId + '>' + languageList[a].LanguageName + '</option>';
            }
        }
        return languageString;
    },
    languageName: function(id) {
        var languagesList = cacheMgr.languagesList();
        if (languagesList && languagesList.length > 0) {
            for (var i = 0; i < languagesList.length; i++) {
                if (id === languagesList[i].LanguageId)
                    return languagesList[i].LanguageName;
            }
            return '';
        }
    },
    airportListOptions: function(type) {
        var apList = cacheMgr.airportList(type), optionString = '<option value="-1">-- Please select an Airport --</option>';
        if (apList && apList.length > 0) {
            for (var i = 0; i < apList.length; i++)
                //Client request to change the filedwork airport to valid airport and not all airports.
                if (sessionStorage.roleId === '1') {
                    if (type === 'reports')
                        optionString += '<option value=' + apList[i].AId + '>' + apList[i].AName + '</option>';
                    else
                        optionString += '<option value=' + apList[i].AirportId + '>' + apList[i].AirportName + '</option>';
                } else {
                    optionString += '<option value=' + apList[i].AirportId + '>' + apList[i].AirportName + '</option>';
                }
        }
        return optionString;
    },
    //[#72732] For the login CT, Edit interviewers is not saving in the manage interviewers page
    validAirportOptions: function() {
        var validairportMap = [], validCodeMap = [], validAirportString = '<option value="-1">-- Please select an Airport --</option>', len, i;
        validairportMap = cacheMgr.validAirportList();
        validairportMap.sort(function(a, b) {
            return (a).airportName.localeCompare((b).airportName)
        })

        len = validairportMap.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                //[#73154] US Domestic:Add interviewer is taking atl in small when we add interviewer name for Atlanta airport.
                validAirportString += '<option value=' + validairportMap[i].airportID + ' label="' + validairportMap[i].airportName + '" >' + validairportMap[i].airportCode.toUpperCase() + '</option>';
            }
        }
        return validAirportString;
    },
    interviewerListOptions: function() {
        var iList = cacheMgr.interviewerList(), optionString = '<option value="-1">-- Please select an Interviewer --</option>';
        if (iList && iList.length > 0) {
            for (var i = 0; i < iList.length; i++)
                optionString += '<option value=' + iList[i].InterviewerId + '>' + iList[i].InterviewerName + '</option>';
        }
        return optionString;
    },
    aircraftTypeListOptions: function() {
        var iList = cacheMgr.aircraftTypeList(), optionString = '<option value="-1">--Please select an Aircraft type--</option>';
        if (iList && iList.length > 0) {
            for (var i = 0; i < iList.length; i++)
                optionString += '<option value=' + iList[i].Name + '>' + iList[i].Name + '</option>';
        }
        return optionString;
    },
    flightTypeListOptions: function() {
        var iList = cacheMgr.flightTypeList(), optionString = '<option value="-1">--Please select Flight type--</option>';
        if (iList && iList.length > 0) {
            for (var i = 0; i < iList.length; i++)
                optionString += '<option value=' + iList[i].Name + '>' + iList[i].Name + '</option>';
        }
        return optionString;
    },
    routeListOptions: function() {
        var iList = cacheMgr.routeList(), optionString = '<option value="-1">-- Please select a Route --</option>';
        if (iList && iList.length > 0) {
            for (var i = 0; i < iList.length; i++)
                optionString += '<option value=' + iList[i].RouteName + '>' + iList[i].RouteName + '</option>';
        }
        return optionString;
    },
    directionListOptions: function() {
        var iList = cacheMgr.directionList(), optionString = '<option value="-1">-- Please select a Direction --</option>';
        if (iList && iList.length > 0) {
            for (var i = 0; i < iList.length; i++)
                optionString += '<option value=' + iList[i].DirectionName + '>' + iList[i].DirectionName + '</option>';
        }
        return optionString;
    },
    formTypeListOptions: function(manage) {
        var iList = cacheMgr.formTypeList(manage), optionString = '<option value="-1">-- Please select a form type --</option>';
        if (iList && iList.length > 0) {
            for (var i = 0; i < iList.length; i++)
                optionString += '<option value=' + iList[i].formID + '>' + iList[i].formName + '</option>';
        }
        return optionString;
    },
    airlineListOptions: function(isDeparture) {
        var airlineVsProps = cacheMgr.airlineList(isDeparture);
        var airlineString = '<option value="-1">-- Please select an Airline --</option>';
        if (airlineVsProps && airlineVsProps.length > 0) {
            for (var j = 0; j < airlineVsProps.length; j++) {
                airlineString += '<option value="' + airlineVsProps[j].id + '" direction="' + airlineVsProps[j].direction +
                        '" flightId="' + airlineVsProps[j].flightId + '" route="' + airlineVsProps[j].route + '" flightType="' + airlineVsProps[j].flightType +
                        '" originId="' + airlineVsProps[j].originId + '">' + airlineVsProps[j].name + '</option>';
            }
        }
        return airlineString;
    },
    getFlightType: function(isDeparture, airlineID, destId) {
        var airlineVsProps = cacheMgr.airlineList(isDeparture);
        var airlineId = parseInt(airlineID);
        var destID = parseInt(destId);
        var flightType = null;
        if (-1 !== airlineId) {
            for (var j = 0; j < airlineVsProps.length; j++) {
                if (airlineId === airlineVsProps[j].id) {
                    airlineVsProps[j].destinations.sort(function(a, b) {
                        return (a).destName.localeCompare((b).destName)
                    });
                    for (var k = 0; k < airlineVsProps[j].destinations.length; k++) {
                        if (destID === airlineVsProps[j].destinations[k].destId) {
                            flightType = airlineVsProps[j].destinations[k].flightType;
                        }
                    }
                }
            }
        }
        return flightType;
    },
    airportDestinationOptions: function(isDeparture) { //Get Destination based on Origin
        //[#72665] CDG International instance:Destination dropdown list is displaying as "please select airline"
        //[#72729] For the login CT, By default “Abu Dhahi” is selecting for the Origin in the Interview reports.
        var airlineVsProps = cacheMgr.airlineList(isDeparture), origin, airportOrigin, isAdded = false, originString = '<option value="-1">-- Please select a Destination --</option>', key, val, airlineMap = new Map(), item, len;
        if (airlineVsProps !== undefined) {
            for (j = 0; j < airlineVsProps.length; j++) {
                origin = airlineVsProps[j].destinations;
                for (var k = 0; k < origin.length; k++) {
                    isAdded = airlineMap.containsKey(origin[k].destId);
                    if (!isAdded) {
                        airlineMap.put(origin[k].destId, origin[k].destName);
                    }
                }
            }
        }
        var sortedAirportDest = this.sortMapOnName(airlineMap);
        len = sortedAirportDest.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                originString += '<option value=' + sortedAirportDest[i].key + '>' + sortedAirportDest[i].name + '</option>';
            }
        }
        return originString;
    },
    airportDestOriginOptions: function() {//Added for changes related to Origin->Fieldwork Airport, Destination->Destination/Origin
        var airlineVsProps = cacheMgr.airlineList(true), airlineVsProps2 = cacheMgr.airlineList(false), origin, airportOrigin, isAdded = false,
                originString = '<option value="-1">-- Please select a Destination/Origin --</option>',
                key, val, airlineMap = new Map(), item, len, dest;
        if (airlineVsProps !== undefined) {
            for (j = 0; j < airlineVsProps.length; j++) {
                origin = airlineVsProps[j].destinations;
                for (var k = 0; k < origin.length; k++) {
                    isAdded = airlineMap.containsKey(origin[k].destId);
                    if (!isAdded) {
                        airlineMap.put(origin[k].destId, origin[k].destName);
                    }
                }
            }
        }
        if (airlineVsProps2 !== undefined) {
            for (j = 0; j < airlineVsProps2.length; j++) {
                dest = airlineVsProps2[j].destinations;
                for (var k = 0; k < dest.length; k++) {
                    isAdded = airlineMap.containsKey(dest[k].destId);
                    if (!isAdded) {
                        airlineMap.put(dest[k].destId, dest[k].destName);
                    }
                }
            }
        }
        var sortedAirportDest = this.sortMapOnName(airlineMap);
        len = sortedAirportDest.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                originString += '<option value=' + sortedAirportDest[i].key + '>' + sortedAirportDest[i].name + '</option>';
            }
        }
        return originString;
    },
    airlineDestinationOptions: function(isDeparture) {//Added for changes related to Origin->Fieldwork Airport, Destination->Destination/Origin
        var airlineVsProps = cacheMgr.airlineList(isDeparture), origin,
                airportOrigin, isAdded = false, originString = '<option value="-1">-- Please select an origin --</option>',
                key, val, airlineMap = new Map(), item, len = 0;
        //[#72729] For the login CT, By default “Abu Dhahi” is selecting for the Origin in the Interview reports.
        if (airlineVsProps !== undefined) {
            for (j = 0; j < airlineVsProps.length; j++) {
                key = airlineVsProps[j].id;
                isAdded = airlineMap.containsKey(key);
                if (!isAdded) {
                    airlineMap.put(key, airlineVsProps[j].name.trim());
                }
            }
        }

        var sortedAirportOrigin = this.sortMapOnName(airlineMap);
        len = sortedAirportOrigin.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                originString += '<option value=' + sortedAirportOrigin[i].key + '>' + sortedAirportOrigin[i].name + '</option>';
            }
        }
        return originString;
    },
    airlineOriginDestOptions: function() { //Getting all Airlines for an Origin.
        var airlineVsProps = cacheMgr.airlineList(false), airlineVsProps2 = cacheMgr.airlineList(true), origin,
                airportOrigin, isAdded = false, originString = '<option value="-1">-- Please select an origin --</option>',
                key, val, airlineMap = new Map(), item, len = 0;
        //[#72729] For the login CT, By default “Abu Dhahi” is selecting for the Origin in the Interview reports.
        if (airlineVsProps !== undefined) {
            for (j = 0; j < airlineVsProps.length; j++) {
                key = airlineVsProps[j].id;
                isAdded = airlineMap.containsKey(key);
                if (!isAdded) {
                    airlineMap.put(key, airlineVsProps[j].name.trim());
                }
            }
        }
        if (airlineVsProps2 !== undefined) {
            for (j = 0; j < airlineVsProps2.length; j++) {
                key = airlineVsProps2[j].id;
                isAdded = airlineMap.containsKey(key);
                if (!isAdded) {
                    airlineMap.put(key, airlineVsProps2[j].name.trim());
                }
            }
        }


        var sortedAirportOrigin = this.sortMapOnName(airlineMap);
        len = sortedAirportOrigin.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                originString += '<option value=' + sortedAirportOrigin[i].key + '>' + sortedAirportOrigin[i].name + '</option>';
            }
        }
        return originString;
    },
    inputKeyDownHandler: function(ele) {
        var elements = ele === undefined ? $('input[type=text]') : ele;
        elements.keydown(function(e) {
            var keyCodes = [46, 8, 9, 27, 13, 110, 190];
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, keyCodes) !== -1 ||
                    // Allow: Ctrl+A
                            (e.keyCode == 65 && e.ctrlKey === true) ||
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
    },
    buildairportInterviewerOptions: function() {
        var airportInterviewerOptions = [], key, item, airportinterviewerString = '<option value="-1">-- Please select an Interviewer --</option>', len, i;
        airportInterviewerOptions = this.sortMapOnName(cacheMgr.airportInterviewerList())
        len = airportInterviewerOptions.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                airportinterviewerString += '<option value=' + airportInterviewerOptions[i].key + '>' + airportInterviewerOptions[i].name + '</option>';
            }
        }
        return airportinterviewerString;
    },
    buildairportDestinationOriginOptions: function(fromFlightCombo) {
        var airportDestOrimap = [], airportDestOriString = '<option value="-1">-- Please select an Airport --</option>', len, i;
        airportDestOrimap = this.sortMapOnName(cacheMgr.airportDestinationOriginList(fromFlightCombo));
        len = airportDestOrimap.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                airportDestOriString += '<option value=' + airportDestOrimap[i].key + '>' + airportDestOrimap[i].name + '</option>';
            }
        }
        return airportDestOriString;
    },
    buildairportairlineOptions: function() {
        var airportDestOrimap = [], airportairlineString = '<option value="-1">-- Please select an Airline --</option>', len, i;
        airportDestOrimap = this.sortMapOnName(cacheMgr.airportairlineList());
        len = airportDestOrimap.length;
        if (len > 0) {
            for (i = 0; i < len; i++) {
                airportairlineString += '<option value=' + airportDestOrimap[i].key + '>' + airportDestOrimap[i].name + '</option>';
            }
        }
        return airportairlineString;
    },
    //[#73109] [Customer]: EU Instance – CDG login- Departure forms – The Destination dropdown is not sorted alphabetical order for Easy Jet(U2) Airlines
    sortMapOnName: function(map) {
        var sortedMap = [];
        var len = map.size();
        if (len > 0) {
            for (i = 0; i < len; i++) {
                var key = Object.keys(map.dict)[i];
                var item = map.get(key);
                sortedMap.push({
                    key: key,
                    name: item
                });
            }
            sortedMap.sort(function(a, b) {
                return (a).name.localeCompare((b).name)
            });
        }
        return sortedMap;
    },
    initialiseDates: function(startDate, endDate) {
        var stDate = $('#' + startDate),
                edDate = $('#' + endDate),
                startInput = $('#' + startDate + ' input'),
                endInput = $('#' + endDate + ' input');

        stDate.datepicker({
            format: "mm/dd/yyyy",
            autoclose: true,
            orientation: "top left",
            todayHighlight: true,
            startDate: '-3650d',
            endDate: '+0d'
        }).on('changeDate', function(ev) {
            var dateText = $(this).data('date');
            var today = moment(new Date()).format('MM/DD/YYYY')
            var endDateTextBox = endInput;
            if (endDateTextBox.val() != '') {
                var testStartDate = new Date(dateText);
                var testEndDate = new Date(endDateTextBox.val());
                //[#73638] [Customer]:EU instance- CDG – View Forms – If data is filtered through an older date and then filtered through a recent date, no results are found.
                if (testStartDate > testEndDate) {
                    //Client Request to set end date to today's date irrespective of the start date.
                    //The scenario that was present earlier was, end date would be changed to start date.
                    endDateTextBox.val(today);
                    edDate.data('date', today);
                }
            } else {
                endDateTextBox.val(today);
                edDate.data('date', today);
            }
//        endDateTextBox.val(dateText);
//        edDate.data('date', dateText);
            edDate.datepicker('setStartDate', dateText);
            edDate.datepicker('update');
            stDate.datepicker('hide');
        });
        $('#' + endDate).datepicker({
            format: "mm/dd/yyyy",
            autoclose: true,
            orientation: "top left",
            todayHighlight: true,
            startDate: '-3650d',
            endDate: '+0d'
        }).on('changeDate', function(ev) {
            var dateText = $(this).data('date');
            var startDateTextBox = startInput;
            if (startDateTextBox.val() != '') {
                var testStartDate = new Date(startDateTextBox.val());
                var testEndDate = new Date(dateText);
                if (testStartDate > testEndDate) {
                    startDateTextBox.val(dateText);
                }
            } else {
                startDateTextBox.val(dateText);
                stDate.data('date', dateText);
            }
            //stDate.datepicker('setStartDate', dateText);
            edDate.datepicker('hide');
        });
    }

};
