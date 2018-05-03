/*<Copyright> Cross-Tab </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> flightcombinations.js </FileName> 
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var manageFlightCombo = {
    flightCombinationsMessage: {
        Addition: 'Flight combination successfully added!',
        ConfirmDelete: 'Are you sure you want to delete?',
        Deletion: 'Flight Combination successfully deleted!',
        AdditionProcessing: 'Adding flight combinations...',
        UpdateProcessing: 'Updating flight combinations...',
        DeletionProgress: 'Deleting filght combination, please wait...',
        ValidError: 'Please re-check the highlighted controls.',
        SuccessfulAddition: 'Flight combo sucessfully added.',
        SuccessfulEdit: 'Flight combo sucessfully updated.',
        DuplicateEntry: 'Sorry, an entry with the said details already exists.',
        TargeyEntry: 'Succesfully added the flight combination. Do you want to set target?',
        validSearch : 'Please enter a valid value for search'
    },
    flightComboTable: '',
    isInitialised: false,
    filghtComboResponse: '',
    rowID: '',
    postData: {},
    initFlightCombinations: function (add) {
        var me = this;
        $(".modal-header h3").html("Add Flight Combination");
        $('#saveFlightCombo').attr('onclick', 'manageFlightCombo.addOrEditFlightCombo(true)');
        if (add) {
            manageFlightCombo.handleValidations();
            $(".sict_error").removeClass('sict_error');
        }
        me.initilizeFlightForms(add);
        me.initilizeFlightAirlines(add);
        me.initilizeFlightOrigin(add);
        me.initilizeFlightDestinations(add);
        me.initilizeFlightRoute(add);
        me.initilizeFlightDirection(add);
        me.initializeFlightType(add);
    },
    initilizeFlightForms: function (add) {
        var optionString = airportUtil.formTypeListOptions(true), data = communion, host = data.hostInstance;
        
        if (add) {
            //[#72944] [Customer]:MS login – Manage Flight Combinations – The Form type dropdown should not have ‘ Arrival + Departure’ option.
            if (host === 'USI' || host === 'AIR')
                $('#addFlightComboForm').empty().append(optionString);
            else
                $('#addFlightComboForm').empty().append(optionString).val('D').attr("selected", "selected");
        } else {
            if (host === 'USI' || host === 'AIR')
                $('#flightComboFomr').empty().append(optionString);
            else
                $('#flightComboFomr').empty().append(optionString).val('D').attr("selected", "selected");
        }
    },
    initilizeFlightAirlines: function (add) {
        cacheMgr.selectedAirport(sessionStorage.airportLoginId);
        var airLineString = airportUtil.buildairportairlineOptions();
        if (add)
            $("#addFlightComboAirlines").empty().append(airLineString);
        else
            $("#flightComboAirlines").empty().append(airLineString);
    },
    initilizeFlightOrigin: function (add) {
        var originString = airportUtil.airportListOptions('reports'), firstText = "";
        if (add)
            $('#addFlightComboOrigin').empty().append(originString);
        else
            $('#flightComboOrigin').empty().append(originString);
        if (add)
            firstText = $('#addFlightComboOrigin').siblings("label").text().trim();
        else
            firstText = $('#flightComboOrigin').siblings("label").text().trim();
        if (firstText && /^Origin/g.test(firstText)) {
            if (add)
                $('#addFlightComboOrigin')[0].options[0].text = "-- Please select an Origin --";
            else
                $('#flightComboOrigin')[0].options[0].text = "-- Please select an Origin --";
        }
    },
    initilizeFlightDestinations: function (add) {
        var optionString = airportUtil.buildairportDestinationOriginOptions(true), firstText = "";
        if (add)
            $("#addFlightComboDestination").empty().append(optionString);
        else
            $("#flightComboDestination").empty().append(optionString);
        if (add)
            firstText = $('#addFlightComboDestination').siblings("label").text().trim();
        else
            firstText = $('#flightComboDestination').siblings("label").text().trim();
        if (firstText && /^Destination/g.test(firstText)) {
            if (add)
                //[#73719] all instances:CT,ms:Flight combination:Destination/origin drop down is displaying as 'please select destination' it should display as 'Please select a destination/origin'
                $('#addFlightComboDestination')[0].options[0].text = "-- Please select a Destination / Origin --";
            else
                $('#flightComboDestination')[0].options[0].text = "-- Please select a Destination / Origin --";
        }
    },
    initilizeFlightRoute: function (add) {
        var optionString = airportUtil.routeListOptions();
        if (add)
            $('#addFlightComboRoute').empty().append(optionString);
        else
            $('#flightComboRoute').empty().append(optionString);
    },
    initilizeFlightDirection: function (add) {
        var optionString = airportUtil.directionListOptions();
        if (add)
            $('#addFlightComboDirection').empty().append(optionString);
        else
            $('#flightComboDirection').empty().append(optionString);
    },
    initializeFlightType: function (add) {
        var optionString = airportUtil.flightTypeListOptions();
        if (add)
            $('#addFlightComboFlightType').empty().append(optionString);
        else
            $('#flightComboFlightType').empty().append(optionString);
    },
    generateFlightCombinations: function () {
        //[#72956] Europe – Flight Combination Report, multiple issues for CT user.
        var me = this, data = communion, host = data.hostInstance,
                formType = (host === 'USI' || host === 'AIR') ? pageUtils.returnValue($('#flightComboFomr').val()) : '-1',
                routeType = host === 'USI' ? pageUtils.returnValue($('#flightComboRoute').val()) : '-1',
                directionType = host === 'USI' ? pageUtils.returnValue($('#flightComboDirection').val()) : '-1',
                flightType = host === 'EUR' ? pageUtils.returnValue($('#flightComboFlightType').val()) : '-1';
        me.uri = getJsonInfoAction(GETFLIGHTCOMBO);
        var flightComboURL = me.uri + '/0/2000/' +
                formType + '/' +
                pageUtils.returnValue($('#flightComboOrigin').val()) + '/' +
                pageUtils.returnValue($('#flightComboDestination').val()) + '/' +
                pageUtils.returnValue($('#flightComboAirlines').val()) + '/' +
                routeType + '/' +
                directionType + '/' +
                flightType;

        me.flightComboTable = $('#flightComboTable').DataTable({
            "bProcessing": true,
            "aLengthMenu": [20, 50, 100],
            "iDisplayLength": 100,
            "order": [[0, "asc"]],
            "pagingType": "simple",
            //[#72733] For the login CT, Filters are not working in Flight Combination Report
            "bDestroy": true,
            "initComplete": function () {
                $("#flightComboTable_filter input").off().on('keyup', function (e) {
                    if (e.keyCode == 13){
                        me.flightComboTable.search(this.value).draw();
                        this.value = this.value.trim();
                        if(this.value.trim() === ''){
                            pageHelper.clearStickies();
                            pageHelper.notify(manageFlightCombo.flightCombinationsMessage.validSearch, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);                    
                        }
                    }                        
                });
                gebo_tips.init();
            },
            "ajax": function (data, callback, settings) {
                $.get(flightComboURL)
                        .done(function (result) {
                            var nodeName = result.FlightDetails;
                            if ((nodeName !== null))
                                callback({ data: nodeName });
                            else
                                callback({ data: [] });
                        });
            },
            "columns": [
                { "data": "AirlineId", sWidth: '6%' },
                { "data": "AirlineName", sWidth: '20%' },
                { "data": "Type", sWidth: '10%' },
                { "data": "Origin", sWidth: '15%' },
                { "data": "Destination", sWidth: '10%' },
                { "data": "Route", sWidth: '14%', "visible": ((host === "EUR" || host === "AIR" || host === "USD") ? false : true) },
                { "data": "Direction", sWidth: '14%', "visible": ((host === "EUR" || host === "AIR" || host === "USD") ? false : true) },
                { "data": "FlightType", sWidth: '8%', "visible": ((host === "EUR") ? true : false) },
                { "data": "AirlineId", sWidth: '8%' }
            ],
            aoColumnDefs: [
                {
                    "aTargets": [2],
                    "mRender": function (data, type, full) {
                        if (data === 'D')
                            return 'Departure';
                        else
                            return 'Arrival';
                    }
                },
                {
                    "aTargets": [3, 4],
                    "mRender": function (data, type, full) {
                        return data.toUpperCase();
                    }
                },
                {
                    "aTargets": [8],
                    //[#73647] [Customer]:All instances- CT login - Flight combination report- Sorting function is not available for Airlines ID, Origin and Destination columns. Also, the sorting does not work on the Airlines column.
                    //[#73720] International:Flight combination:It is showing sort function symbol for Action column also.
                    'bSortable': false,
                    "mRender": function (data, type, full) {
                        $('body').data(data.toString(), full);
                        return '<a href="#addFlightCombination" data-toggle="modal" data-backdrop="static"  onclick="manageFlightCombo.editFlightCombinations(this)" class="sepDelEdit ttip_t" title="Edit Flight Combination"><i class="fa fa-pencil-square-o fa-lg btnColor"></i></a>' +
                                '<a href="#"  onclick="manageFlightCombo.deleteFlightCombo(this)" title="Delete Flight Combination"  class="sepDelEdit ttip_t" ><i class="fa fa-trash fa-lg btnColor"></i></a>';
                    }
                },
            ]
        });
    },
    addOrEditFlightCombo: function (add, flightCombinationID) {
        var postObj, message, flightComboURL, method;
        var data = communion, host = data.hostInstance;
        if (($('#addFlightComboForm').val() === '-1')) {
            $('#addFlightComboForm').parent().addClass('sict_error');
        }
        if (($('#addFlightComboAirlines').val() === '-1')) {
            $('#addFlightComboAirlines').parent().addClass('sict_error');
        }
        if (($('#addFlightComboOrigin').val() === '-1')) {
            $('#addFlightComboOrigin').parent().addClass('sict_error');
        }
        if (($('#addFlightComboDestination').val() === '-1')) {
            $('#addFlightComboDestination').parent().addClass('sict_error');
        }
        if (($('#addFlightComboRoute').val() === '-1')) {
            $('#addFlightComboRoute').parent().addClass('sict_error');
        }
        if (($('#addFlightComboDirection').val() === '-1')) {
            $('#addFlightComboDirection').parent().addClass('sict_error');
        }
        //[#73355] [Customer]:EU- CT login – Manage Flight combination- Flight combination can be added without selecting a Flight type.
        if (($('#addFlightComboFlightType').val() === '-1')) {
            $('#addFlightComboFlightType').parent().addClass('sict_error');
        }
        postObj = {
            "SessionId": sessionStorage.sessionId,
            "Version": communion.Version,
            "Instance": getInstance(),
            "FlightDetail": {}
        };
        var me = this;
        if (add) {
            method = 'PUT';
            flightComboURL = getJsonInfoAction(ADDFLIGHTCOMBO);
            message = manageFlightCombo.flightCombinationsMessage.AdditionProcessing;
        } else {
            method = 'POST';
            flightComboURL = getJsonInfoAction(UPDATEFLIGHTCOMBO);
            postObj.FlightDetail.FlightCombinationId = flightCombinationID;
            message = manageFlightCombo.flightCombinationsMessage.UpdateProcessing;
        }
        if ($('.sict_error').length === 0) {
            postObj.FlightDetail.AirlineId = $('#addFlightComboAirlines').val();
            postObj.FlightDetail.OriginId = $('#addFlightComboOrigin').val();
            postObj.FlightDetail.DestinationId = $('#addFlightComboDestination').val();
            postObj.FlightDetail.Type = $('#addFlightComboForm').val();
            //[#72696] Europe – Add Flight Combination is not working for the Mind set user. Route and Destination filters are empty.
            postObj.FlightDetail.Route = host === 'USI' ? $('#addFlightComboRoute').val() : null;
            postObj.FlightDetail.Direction = host === 'USI' ? $('#addFlightComboDirection').val() : null;
            postObj.FlightDetail.FlightType = host === 'EUR' ? $('#addFlightComboFlightType').val() : null;
            pageHelper.clearStickies();
            pageHelper.addSmokeSignal(message);
            this.postData = postObj;

            ajaxPost(flightComboURL, postObj, '', me.acknowledgeAddOrEdit, add, '', method);
        } else {
            pageHelper.clearStickies();
            pageHelper.notify(manageFlightCombo.flightCombinationsMessage.ValidError, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }

        setTimeout(function () {
            $.ajax({
               // url: "http://80.80.229.30/AutoUpload/UserDetailsServices.svc/CacheFileAirportAirlineList/US",
                url: "http://SICTSERVICE/AutoUpload/UserDetailsServices.svc/CacheFileAirportAirlineList/US",
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                async: false,
                success: function (r) {
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }, 2500);
    },
    acknowledgeAddOrEdit: function (data, success, add) {
        if (success) {
            if (data.ReturnCode === 1) {
                var msg;
                if (add)
                    msg = manageFlightCombo.flightCombinationsMessage.SuccessfulAddition;
                else
                    msg = manageFlightCombo.flightCombinationsMessage.SuccessfulEdit;

                //[#73056] International:Edit flight combination showing some popup message while submitting it.
                //manageFlightCombo.flightComboTable.ajax.reload(null, false);
                pageHelper.removeSmokeSignal();
                pageHelper.notify(msg, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
                $('#addFlightCombination').modal('hide');
                cacheMgr.clearCache(true);//pass true if required session needs to be cleared.
                //manageFlightCombo.initFlightCombinations();
                manageFlightCombo.generateFlightCombinations();
                //  [#73608] [#72870] [Customer_mindset]:Linking Flight combinations with Targets so that whenever a
                //  new flight combination is added and no targets are set, the system should prompt an appropriate message automatically
                if (!data.IsTargetPresent)
                    smoke.confirm(manageFlightCombo.flightCombinationsMessage.TargeyEntry,
                        manageFlightCombo.handleNewFlightTargetConfirm,
                        {
                            ok: "Yes",
                            cancel: "No"
                        });
            } else if (data.ReturnCode === 4) {
                pageHelper.removeSmokeSignal();
                pageHelper.notify(manageFlightCombo.flightCombinationsMessage.DuplicateEntry, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            }
        }
    },
    deleteFlightCombo: function (ele) {
        smoke.confirm('Are you sure you want to delete?', function (e) {
            if (e) {
                var tr = $(ele).closest('tr');
                var row = manageFlightCombo.flightComboTable.row(tr);
                var rowIndex = row.index();
                var rowData = row.data();
                var uri = getJsonInfoAction(DELETEFLIGHTCOMBO);
                var postData = {
                    "Version": communion.Version,
                    "SessionId": sessionStorage.sessionId,
                    "Instance": getInstance(),
                    "FlightCombinationId": rowData.FlightCombinationId
                };
                pageHelper.clearStickies();
                pageHelper.addSmokeSignal(manageFlightCombo.flightCombinationsMessage.DeletionProgress);
                ajaxPost(uri, postData, '', manageFlightCombo.acknoledgeDeleteFlightCombo, rowIndex, '', 'DELETE');
                setTimeout(function () {
                    $.ajax({
                       // url: "http://80.80.229.30/AutoUpload/UserDetailsServices.svc/CacheFileAirportAirlineList/US",
                        url: "http://SICTSERVICE/AutoUpload/UserDetailsServices.svc/CacheFileAirportAirlineList/US",
                        type: "GET",
                        contentType: "application/json",
                        dataType: "json",
                        async: false,
                        success: function (r) {
                        },
                        error: function (xhr, ajaxOptions, thrownError) {

                        }
                    });
                }, 2500);
            }
        });
    },
    acknoledgeDeleteFlightCombo: function (data, success, rowIndex) {
        if (success) {
            pageHelper.removeSmokeSignal();
            pageHelper.notify(manageFlightCombo.flightCombinationsMessage.Deletion, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
            manageFlightCombo.flightComboTable.row(rowIndex).remove().draw(false);
        }
    },
    editFlightCombinations: function (ele) {
        var data = communion, host = data.hostInstance;
        manageFlightCombo.initFlightCombinations(true);
        var tr = $(ele).closest('tr');
        var row = manageFlightCombo.flightComboTable.row(tr);
        manageFlightCombo.rowID = row;
        var rowData = row.data();
        var formType = (host === 'USI' || host === 'AIR') ? rowData.Type : 'D';
        var airlineID = rowData.AirlineId;
        var destinationID = rowData.DestinationId;
        var originID = rowData.OriginId;
        var routeName = rowData.Route;
        var directionName = rowData.Direction;
        var flightCombinationID = rowData.FlightCombinationId;
        var flightType = rowData.FlightType;

        $("#addFlightComboForm").val(formType).attr("selected", "selected");
        $("#addFlightComboAirlines").val(airlineID).attr("selected", "selected");
        $("#addFlightComboOrigin").val(originID).attr("selected", "selected");
        $("#addFlightComboDestination").val(destinationID).attr("selected", "selected");
        $("#addFlightComboRoute").val(routeName).attr("selected", "selected");
        $("#addFlightComboDirection").val(directionName).attr("selected", "selected");
        $("#addFlightComboFlightType").val(flightType).attr("selected", "selected");
        $(".modal-header h3").html("Edit Flight Combination");
        $('#saveFlightCombo').attr('onclick', 'manageFlightCombo.addOrEditFlightCombo(false, ' + flightCombinationID + ')');
        $(".sict_error").removeClass('sict_error');
        manageFlightCombo.handleValidations();
    },
    handleValidations: function () {
        $("select").focusin(function () {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('sict_error');
        });
        $("select").focusout(function (e) {
            var target = $(e.target);
            if ("-1" === target.val())
                target.parent().addClass('sict_error');
            else
                target.parent().removeClass('sict_error');
        });
    },
    handleNewFlightTargetConfirm: function (e) {
        if (e) {
            cacheMgr.storeFlightInfo(manageFlightCombo.postData);
            window.location.href = "target.html";
        }
    }
};