/*<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> cachemanagers.js </FileName>
 <Author> Raghavendra G.N, Akhilesh M.S, Vivek.A </Author>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var g_ArrivalMap = new Map(), g_DepartureMap = new Map(), g_AirportLoginMap = new Map(), g_airportinterviewer = new Map(), g_airportDestinationOrigin = new Map(), g_airportAirline = new Map(), g_airportNameMap = new Map(), g_airportNameCodeMap = new Map(), g_airportNameCodeOBJ = [];
var cacheMgr = {
    get_airportList: function (type) {
        var airportURL = "./cache/AirportList/AirportList_" + sessionStorage.airportLoginId + ".json";
        if (sessionStorage.roleId === '1'){
            //Client request to change the filedwork airport to valid airport and not all airports.
            if(type === 'reports')
                airportURL = "./cache/AirportIdVstLoginId.json";
            else
                airportURL = "./cache/AirportList/AirportList_Admin.json";
        }
             
        $.ajax({
            type: 'GET',
            url: airportURL,
            dataType: 'json',
            async: false,
            data: {},
            success: function (response) {
                if (response && response.length > 0) {
                    //TODO: Check for session time out and validtity
                    response.sort(function (a, b) {
                        if (sessionStorage.roleId === '1'){
                            if(type === 'reports')
                                return (a).AName.localeCompare((b).AName)
                            else
                                return (a).AirportName.localeCompare((b).AirportName)
                        }else{
                            return (a).AirportName.localeCompare((b).AirportName)
                        }
                    });
                    sessionStorage.airportList = JSON.stringify(response);
                }
            }
            //TODO: Check for failures
        });
    },
    airportList: function (type) {
//        if (undefined === sessionStorage.airportList || sessionStorage.airportList.length === 0) {
//              this.get_airportList(type);    
//        }
//        if (undefined !== sessionStorage.airportList && sessionStorage.airportList.length > 0) {
//            return JSON.parse(sessionStorage.airportList);
//        }
        this.get_airportList(type);
        return JSON.parse(sessionStorage.airportList);
    },
    get_interviewerList: function (loginId) {
        var interviewerURL = "./cache/InterviewerList/InterviewerList_" + loginId + ".json";
        if (sessionStorage.roleId === '1')
            interviewerURL = "./cache/InterviewerList/InterviewerList_Admin.json";
        sessionStorage.interviewerList = JSON.stringify([]);
        $.ajax({
            url: interviewerURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response) {
                    //TODO: Check for session time out and validtity
                    response.sort(function (a, b) {
                        return (a).InterviewerName.localeCompare((b).InterviewerName)
                    });
                    sessionStorage.interviewerList = JSON.stringify(response);
                }
            }
            //TODO: Check for failures
        });
    },
    interviewerList: function () {
        if (undefined === sessionStorage.interviewerList || sessionStorage.interviewerList.length === 0) {
            //	interviewers list is based on login id . not based on airport
            this.get_interviewerList(sessionStorage.airportLoginId);
        }
        if (undefined !== sessionStorage.interviewerList) {
            return JSON.parse(sessionStorage.interviewerList);
        }
    },
    get_aircraftTypeList: function () {
        if (sessionStorage.roleId)
            aircrafttypeURL = "./cache/AircraftTypes.json";
        $.ajax({
            url: aircrafttypeURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
                    response.sort(function (a, b) {
                        return (a).Name.localeCompare((b).Name)
                    });
                    sessionStorage.aircraftTypeList = JSON.stringify(response);
                }
            }
        });
    },
    aircraftTypeList: function () {
        if (undefined === sessionStorage.aircraftTypeList || sessionStorage.aircraftTypeList.length === 0) {
            this.get_aircraftTypeList();
        }
        if (undefined !== sessionStorage.aircraftTypeList && sessionStorage.aircraftTypeList.length > 0) {
            return JSON.parse(sessionStorage.aircraftTypeList);
        }
    },
    get_flightTypeList: function () {
        var flighttypeURL = "./cache/FlightTypes.json";
        if (sessionStorage.roleId)
            flighttypeURL = "./cache/FlightTypes.json";
        $.ajax({
            url: flighttypeURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
//                        response.sort(function (a, b) {
//                            return (a).Name.localeCompare((b).Name)
//                        });                    
                    sessionStorage.flightTypeList = JSON.stringify(response);
                }
            }
        });
    },
    flightTypeList: function () {
        if (undefined === sessionStorage.flightTypeList || sessionStorage.flightTypeList.length === 0) {
            this.get_flightTypeList();
        }
        if (undefined !== sessionStorage.flightTypeList && sessionStorage.flightTypeList.length > 0) {
            return JSON.parse(sessionStorage.flightTypeList);
        }
    },
    get_routeList: function (airportId) {
        var routeURL = "./cache/RouteList.json";
        if (sessionStorage.roleId)
            routeURL = "./cache/RouteList.json";
        $.ajax({
            url: routeURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
                    //TODO: Check for session time out and validtity
                    response.sort(function (a, b) {
                        return (a).RouteName.localeCompare((b).RouteName)
                    });
                    sessionStorage.routeList = JSON.stringify(response);
                }
            }
            //TODO: Check for failures
        });
    },
    routeList: function () {
        if (undefined === sessionStorage.routeList || sessionStorage.routeList.length === 0) {
            //	interviewers list is based on login id . not based on airport
            this.get_routeList();
        }
        if (undefined !== sessionStorage.routeList && sessionStorage.routeList.length > 0) {
            return JSON.parse(sessionStorage.routeList);
        }
    },
    get_directionList: function (airportId) {
        var directionURL = "./cache/DirectionList.json";
        if (sessionStorage.roleId)
            directionURL = "./cache/DirectionList.json";
        $.ajax({
            url: directionURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
                    //TODO: Check for session time out and validtity
                    response.sort(function (a, b) {
                        return (a).DirectionName.localeCompare((b).DirectionName)
                    });
                    sessionStorage.directionList = JSON.stringify(response);
                }
            }
            //TODO: Check for failures
        });
    },
    directionList: function () {
        if (undefined === sessionStorage.directionList || sessionStorage.directionList.length === 0) {
            //	interviewers list is based on login id . not based on airport
            this.get_directionList();
        }
        if (undefined !== sessionStorage.directionList && sessionStorage.directionList.length > 0) {
            return JSON.parse(sessionStorage.directionList);
        }
    },
    get_formTypeList: function (manage) {
        var formObject =
                [
                    {
                        formID: 'D',
                        formName: 'Departure'
                    },
                    {
                        formID: 'A',
                        formName: 'Arrival'
                    }
                    
                ];
        if (!manage) {
            formObject.push({
                formID: '-1',
                formName: 'Departure + Arrival'
            })
        }
        sessionStorage.formTypeList = JSON.stringify(formObject);
    },
    formTypeList: function (manage) {
        //As the form type chnges across tabs, we are calling it everytime.
        this.get_formTypeList(manage);
        //            return JSON.parse(sessionStorage.formTypeList);
        //        if (undefined === sessionStorage.formTypeList || sessionStorage.formTypeList.length === 0) {
        //            this.get_formTypeList(manage);
        //        }
        if (undefined !== sessionStorage.formTypeList && sessionStorage.formTypeList.length > 0) {
            return JSON.parse(sessionStorage.formTypeList);
        }
    },
    get_airlinesList: function (airportId, isDeparture) {
        var airlineURL = "./cache/AirportAirlineList/Departure_AirportAirlineList_" + airportId + ".json";
        if (!isDeparture)
            airlineURL = "./cache/AirportAirlineList/Arrival_AirportAirlineList_" + airportId + ".json";
        var languageString = '', airlineString = '', firstAirlineCode = '', firstAirlineDestinatons = '', fALName = '', airlineVsProps = [];
        firstAirlineDestinatons = '', airlineString = '', firstAirlineCode = '';
        var optionString = '', curAirlineID = '', airlineCode = {}, isAdded = true;
        $.ajax({
            url: airlineURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
                    //TODO: Check for session time out and validtity

                    for (var i = 0; i < response.length; i++) {
                        isAdded = false;
                        if (curAirlineID !== response[i].AirlineId) {
                            isAdded = true;
                            if (airlineCode !== undefined && airlineCode.destinations !== undefined) {
                                airlineCode.destinations.sort(function (a, b) {
                                    return (a).destName.localeCompare((b).destName)
                                });
                            }
                            curAirlineID = response[i].AirlineId;

                            airlineCode = {
                                id: response[i].AirlineId,
                                code: response[i].AirlineCode,
                                name: response[i].AirlineName,
                                direction: response[i].Direction,
                                flightId: response[i].FlightId,
                                route: response[i].Route,
                                originId: response[i].OriginId,
                                flightType: response[i].FlightType,
                                destinations: [{
                                    destId: isDeparture ? response[i].DestinationId : response[i].OriginId,
                                    destName: isDeparture ? response[i].DestinationName : response[i].OriginName,
                                    flightType: response[i].FlightType
                                }]
                            };

                            airlineVsProps.push(airlineCode);
                        }
                        if (!isAdded) {
                            airlineCode.destinations.push(
                                    {
                                        destId: isDeparture ? response[i].DestinationId : response[i].OriginId,
                                        destName: isDeparture ? response[i].DestinationName : response[i].OriginName,
                                        flightType: response[i].FlightType
                                    }
                            );
                        }
                    }
                    airlineVsProps.sort(function (a, b) {
                        return (a).name.localeCompare((b).name)
                    });
                }
            }
            //TODO: Check for failures
        });

        airlineVsProps.sort(function (a, b) {
            return (a).name.localeCompare((b).name)
        });
        return airlineVsProps;
    },
    airlineList: function (isDeparture) {
        var airlineVsProps = [], airportId = sessionStorage.selectedAirportId;
        //[#72729] For the login CT, By default “Abu Dhahi” is selecting for the Origin in the Interview reports.
        if(sessionStorage.roleId !== '1'){
            if (airportId === "-1")
                return undefined;
        }
        var airlineMap = isDeparture ? g_DepartureMap : g_ArrivalMap;
        var isAdded = airlineMap.containsKey(airportId);
        if (!isAdded) {
            var alList = this.get_airlinesList(airportId, isDeparture);
            if (undefined !== alList) {
                airlineMap.put(airportId, alList);
                isAdded = true;
            }
        }
        if (isAdded)
            airlineVsProps = airlineMap.get(airportId);

        return airlineVsProps;
    },
    get_languagesList: function () {
        var languageURL = "./cache/LanguageList.json";

        $.ajax({
            type: 'GET',
            url: languageURL,
            dataType: 'json',
            async: false,
            data: {},
            success: function (response) {
                if (response && response.length > 0) {
                    //TODO: Check for session time out and validtity
                    sessionStorage.languagesList = JSON.stringify(response);
                }
            }
            //TODO: Check for failures
        });
    },
    languagesList: function () {
        if (undefined === sessionStorage.languagesList || sessionStorage.languagesList.length === 0) {
            this.get_languagesList();
        }
        if (undefined !== sessionStorage.languagesList && sessionStorage.languagesList.length > 0) {
            return JSON.parse(sessionStorage.languagesList);
        }
    },
    selectedAirport: function (id) {
        if (id !== sessionStorage.selectedAirportId) {
            sessionStorage.selectedAirportId = id;
            var lId = this.selectedAirportLoginId();
            this.get_interviewerList(lId);
        }
    },
    get_airportLoginMap: function () {
        var interviewerURL = "./cache/AirportIdVstLoginId.json";

        $.ajax({
            url: interviewerURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response) {
                    g_AirportLoginMap = new Map();

                    //TODO: Check for session time out and validtity
                    $.each(response, function (i, ele) {
                        g_AirportLoginMap.put(ele.AId, ele.LId);
                        g_airportNameMap.put(ele.AId, ele.AName);
                        g_airportNameCodeMap.put(ele.AName, ele.Code);
                        g_airportNameCodeOBJ.push({
                            airportID: ele.AId,
                            airportName: ele.AName,
                            airportCode: ele.Code
                        })
                    });
                }
            }
            //TODO: Check for failures
        });
    },
    selectedAirportLoginId: function () {
        if (undefined === g_AirportLoginMap || g_AirportLoginMap.size() === 0) {
            this.get_airportLoginMap();
        }
        var lId;
        if (g_AirportLoginMap.containsKey(sessionStorage.selectedAirportId))
            lId = g_AirportLoginMap.get(sessionStorage.selectedAirportId);

        return lId;
    },
    //[#72732] For the login CT, Edit interviewers is not saving in the manage interviewers page
    validAirportList: function () {
        if (undefined === g_airportNameMap || g_airportNameMap.size() === 0) {
            this.get_airportLoginMap();
        }
        //return g_airportNameMap;
        return g_airportNameCodeOBJ;
    },
    get_airportDestinationOriginList: function (fromFlightCombo) {
        var airportDestOriURL, isAdded = false, key;
        //        if (sessionStorage.airportLoginId === "38")
        //            airportDestOriURL = "./cache/AirportReport/OriginAndDestinationList_" + sessionStorage.airportLoginId + ".json";
        //        else
        //            airportDestOriURL = "./cache/AirportReport/OriginAndDestinationList_Admin.json";
        if (fromFlightCombo) {
            airportDestOriURL = "./cache/AirportList/AirportList_Admin.json";
        } else {
            if (sessionStorage.roleId === "1")
                airportDestOriURL = "./cache/AirportReport/OriginAndDestinationList_Admin.json";
            else
                airportDestOriURL = "./cache/AirportReport/OriginAndDestinationList_" + sessionStorage.airportLoginId + ".json";
        }
        $.ajax({
            url: airportDestOriURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
                    if (fromFlightCombo)
                        response.sort(function (a, b) {
                            return (a).AirportName.trim().localeCompare((b).AirportName.trim())
                        });
                    else
                        response.sort(function (a, b) {
                            return (a).Name.localeCompare((b).Name)
                        });
                    for (var i = 0; i < response.length; i++) {
                        key = fromFlightCombo === true ? response[i].AirportId : response[i].Id;
                        isAdded = g_airportDestinationOrigin.containsKey(key);
                        if (!isAdded) {
                            if (fromFlightCombo)
                                g_airportDestinationOrigin.put(key, response[i].AirportName.trim());
                            else
                                g_airportDestinationOrigin.put(key, response[i].Name.trim());
                        }
                    }
                }
            }
        });
    },
    airportDestinationOriginList: function (fromFlightCombo) {
        var me = this;
        me.get_airportDestinationOriginList(fromFlightCombo);
        return g_airportDestinationOrigin;
        //        if (g_airportDestinationOrigin.isEmpty())
        //            me.get_airportDestinationOriginList(fromFlightCombo);
        //        if (!g_airportDestinationOrigin.isEmpty())
        //            return g_airportDestinationOrigin;
    },
    get_airportairlineList: function () {
        var airportairlineURL, isAdded = false, key;
        //        if (sessionStorage.airportLoginId === "38")
        //            airportairlineURL = "./cache/AirportReport/AirlineList_" + sessionStorage.airportLoginId + ".json";
        //        else
        //            airportairlineURL = "./cache/AirportReport/AirlineList_Admin.json";
        if (sessionStorage.roleId === "1")
            airportairlineURL = "./cache/AirportReport/AirlineList_Admin.json";
        else
            airportairlineURL = "./cache/AirportReport/AirlineList_" + sessionStorage.airportLoginId + ".json";
        $.ajax({
            url: airportairlineURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
                    response.sort(function (a, b) {
                        return (a).Name.localeCompare((b).Name)
                    });
                    for (var i = 0; i < response.length; i++) {
                        key = response[i].Id;
                        isAdded = g_airportAirline.containsKey(key);
                        if (!isAdded) {
                            g_airportAirline.put(key, response[i].Name.trim());
                        }
                    }
                }
            }
        });
    },
    airportairlineList: function () {
        var me = this;
        if (g_airportAirline.isEmpty())
            me.get_airportairlineList();
        if (!g_airportAirline.isEmpty())
            return g_airportAirline;
    },
    get_airportInterviewerList: function () {
        var airportinterviewerURL, isAdded = false, key, len;
        if (sessionStorage.roleId === "1")
            airportinterviewerURL = "./cache/InterviewerList/InterviewerList_Admin.json";
        else
            airportinterviewerURL = "./cache/InterviewerList/InterviewerList_" + sessionStorage.airportLoginId + ".json";
        $.ajax({
            url: airportinterviewerURL,
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response && response.length > 0) {
                    response.sort(function (a, b) {
                        return (a).InterviewerName.localeCompare((b).InterviewerName)
                    });
                    for (var i = 0; i < response.length; i++) {
                        key = response[i].InterviewerId;
                        isAdded = g_airportinterviewer.containsKey(key);
                        if (!isAdded) {
                            g_airportinterviewer.put(key, response[i].InterviewerName.trim());
                        }
                    }
                }
            }
        });
    },
    airportInterviewerList: function () {
        var me = this;
        if (g_airportinterviewer.isEmpty())
            me.get_airportInterviewerList();
        if (!g_airportinterviewer.isEmpty())
            return g_airportinterviewer;
    },
    clearCache: function (purgeSession) {
        //Clearing all Maps & Gloabl Code Obj.
        g_ArrivalMap = new Map(),
                g_DepartureMap = new Map(),
                g_AirportLoginMap = new Map(),
                g_airportinterviewer = new Map(),
                g_airportDestinationOrigin = new Map(),
                g_airportAirline = new Map(),
                g_airportNameMap = new Map(),
                g_airportNameCodeMap = new Map(),
                g_airportNameCodeOBJ = [];
        if (purgeSession)
            this.clearSession();
    },
    clearSession: function () {
        delete sessionStorage.airportList;
        delete sessionStorage.interviewerList;
    },
    storeFlightInfo: function (data) {
        sessionStorage.FlightInfo = JSON.stringify(data);
    },
    getFlightInfo: function () {
        return undefined === sessionStorage.FlightInfo ? undefined : JSON.parse(sessionStorage.FlightInfo);
    },
    deleteFlightInfo: function () {
        delete sessionStorage.FlightInfo;
    }
};

/**
 * An object that maps keys to values. A map cannot contain duplicate keys; each key can map to at most one value.
 * For those familiar with the Java programming language, this is similar to a HashMap; it implements most of the
 * methods defined by Java's java.util.Map interface.
 *
 * @constructor
 * @version 1.1.0
 * @author cody@base22.com Burleson, Cody
 */
function Map() {
    this.dict = {};

    /**
     * Returns the number of key-value mappings in this map.
     * @method
     */
    this.size = function () {
        return Object.keys(this.dict).length;
    };

    /**
     * Returns true if this map contains no key-value mappings.
     * @method
     */
    this.isEmpty = function () {
        return Object.keys(this.dict).length == 0;
    };

    /**
     * Returns the value to which the specified key is mapped, or null if this map contains no mapping for the key.
     * @method
     */
    this.get = function (key) {
        return this.dict[key];
    };

    /**
     * Returns true if this map contains a mapping for the specified key.
     * @method
     */
    this.containsKey = function (key) {
        if (this.get(key) !== undefined) {
            return true;
        } else {
            return false;
        }
    };

    /**
     * Associates the specified value with the specified key in this map. If the map previously contained a mapping for the key, the old value is replaced.
     * @method
     */
    this.put = function (key, value) {
        this.dict[key] = value;
    };

    /**
     * Removes the mapping for the specified key from this map if present.
     * @method
     */
    this.remove = function (key) {
        try {
            'use strict';
            delete this.dict[key];
        } catch (e) {
            console.log(e);
        }
    };

    /**
     * Removes all of the mappings from this map. The map will be empty after this call returns.
     * @method
     */
    this.clear = function () {
        this.dict = {};
    };

    /**
     * Executes the given callback for each entry in this map until all entries have been processed.
     * The given callback will be passed a map entry as parameter. So, for example...
     *
     * function myCallback(mapEntryItem) {
     * 		console.log('I will process this item: ' + mapEntryItem.text);
     * }
     *
     * myMap.forEach(myCallback);
     *
     * @method
     */
    this.forEach = function (callback) {
        var len = this.size();
        for (i = 0; i < len; i++) {
            var item = this.get(Object.keys(this.dict)[i]);
            callback(item);
        }
    };

    this.first = function (callback) {
        var len = this.size();
        for (i = 0; i < len && i < 1; i++) {
            var item = this.get(Object.keys(this.dict)[0]);
            callback(Object.keys(this.dict)[0], item);
        }
    };

    this.forAll = function (callback) {
        var len = this.size();
        for (i = 0; i < len; i++) {
            var key = Object.keys(this.dict)[i];
            var item = this.get(key);
            callback(key, item);
        }
    };
}