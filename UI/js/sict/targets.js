/*<Copyright> Cross-Tab </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> targets.js </FileName>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var targetMessages = {
    updating: 'Updating changes. Please wait...',
    deletingProgress: 'Deleting target, please wait...',
    deleteSuccess: 'Target deleted successfully!',
    confirmDelete: 'Are you sure you want to delete?',
    emptyUpdates: 'There are no targets to update',
    updateSuccess: 'Targets successfully updated',
    validError: 'Please re-check the highlighted controls',
    targetAdded: 'Target combination added successfully',
    duplicateEntry: 'Sorry, an entry with the said details already exists',
    addTargetProcessing: 'Processing your request. Please wait...',
    validSearch : 'Please enter a valid value for search'
};

var dynamicTargets = {
    targetsTable: '',
    formType: '',
    airlineVsProps: [],
    targetFormType: '',
    isInitialised: false,
    targetResponse: '',
    targetModified: [],
    targetCount: 0,
    uri: '',
    reference: '',
    filledUpHtml: [],
    enableSave: false,
    updateTarget: { "SessionId": sessionStorage.sessionId, "Version": communion.Version, "Instance": getInstance(), "TargetUpdateDetails": [] },
    initTarget: function () {
        var me = this;
        $('#Generate').on('click', function () {
            $('#formHdr').toggle('200');
            //[#73699] Incorrect tool tip and icon is showing on Interviewers, Flight Combination and target.
            $(this).parents().find('#parentDiv').find('i').removeClass('fa-arrow-circle-up').addClass('fa-arrow-circle-down').attr('title','Expand');
            $('#parentDiv').removeClass('formSep');
            me.retrieveTargets();
        });
        me.initializeTargerForms();
        me.initializeTargetOrigin();
        me.initializeTargetRoutes();
        me.initializeTargetDirections();
        me.initializeTargerAirlines();
        me.initializeFlightType();
        me.initializeAircrafttType();
        //  Iniitalize the content of modal based
        var children = $($('#formHdr').children()).clone();
        filledUpHtml = children.slice(0, children.length - 1);
        $('#addFormHdr').prepend(filledUpHtml);

        //  Register for modal events
        $('#addTargetCombination').on('hidden.bs.modal', function (e) {
            me.reference = '';
        });

        $('#addTargetCombination').on('show.bs.modal', function (e) {
            //  Check for flight combination addition
            var flightCombinationData = cacheMgr.getFlightInfo();
            if (undefined !== flightCombinationData) {
                var formType = (communion.hostInstance === 'USI' || communion.hostInstance === 'AIR') ? flightCombinationData.FlightDetail.Type : 'D';
                var airlineID = flightCombinationData.FlightDetail.AirlineId;
                var destinationID = flightCombinationData.FlightDetail.DestinationId;
                var originID = flightCombinationData.FlightDetail.OriginId;
                var routeName = flightCombinationData.FlightDetail.Route;
                var directionName = flightCombinationData.FlightDetail.Direction;
                var flightCombinationID = flightCombinationData.FlightDetail.FlightCombinationId;
                var flightType = flightCombinationData.FlightDetail.FlightType;

                $(me.reference + "#targetFormTypeDD").val(formType).attr("selected", "selected");
                $(me.reference + "#targetAirlineDD").val(airlineID).attr("selected", "selected");
                $(me.reference + "#targetOriginDD").val(originID).attr("selected", "selected");
                //$(this.reference + "#addFlightComboDestination").val(destinationID).attr("selected", "selected");
                $(me.reference + "#targetRouteDD").val(routeName).attr("selected", "selected");
                $(me.reference + "#targetDirectionDD").val(directionName).attr("selected", "selected");
                $(me.reference + "#targetFlightDD").val(flightType).attr("selected", "selected");
                $(me.reference + '#targetInput').focus();
                me.reference = '';
                cacheMgr.deleteFlightInfo();
            }

            if (communion.hostInstance === 'USI' || communion.hostInstance === 'AIR')
                $('.modal #targetFormTypeDD').find('option:last').hide();
            else
                $('.modal #targetFormTypeDD').val('D').attr("selected", "selected");
        });
        //  Wait for UI to initialize
        //  [#73608] [#72870] [Customer_mindset]:Linking Flight combinations with Targets so that whenever a
        //  new flight combination is added and no targets are set, the system should prompt an appropriate message automatically
        setTimeout(function () {
            //  Initialize the flight combination data
            var flightCombinationData = cacheMgr.getFlightInfo();
            if (undefined !== flightCombinationData) {
                me.reference = '.modal ';
                $('#addTargetCombination').modal('show');
            }
        }, 1500);
    },
    initializeTargerForms: function () {
        var formString = airportUtil.formTypeListOptions();
        var data = communion, host = data.hostInstance;
        if (host !== 'USI' && host !== 'AIR')
            $(this.reference + '#targetFormTypeDD').empty().append(formString).val('D').attr("selected", "selected");
        else
            $(this.reference + '#targetFormTypeDD').empty().append(formString);
    },
    initializeTargetOrigin: function () {
        var originString = airportUtil.airportListOptions('reports'), firstText = "";
        $(this.reference + '#targetOriginDD').empty().append(originString);
        firstText = $(this.reference + '#targetOriginDD').siblings("label").text().trim();
        if (firstText && /^Fieldwork Airport/g.test(firstText))
            $(this.reference + '#targetOriginDD')[0].options[0].text = "-- Please select an Airport --";
        $(this.reference + "#targetOriginDD").selectedIndex = 1;
    },
    initializeTargetRoutes: function () {
        var routeString = airportUtil.routeListOptions();
        $(this.reference + '#targetRouteDD').append(routeString);
    },
    initializeTargetDirections: function () {
        var directionString = airportUtil.directionListOptions();
        $(this.reference + '#targetDirectionDD').append(directionString);
    },
    initializeTargerAirlines: function () {
        //        var selectedId = $("#targetOriginDD").val(), firstText = "";
        //        cacheMgr.selectedAirport(selectedId);
        //
        //        var airlinesString = airportUtil.airlineOriginDestOptions();
        //[#73675] [Customer]: Aircraft instance –CT login – Manage Targets : Need the ability to filter the Target report when only Airlines filter is to be used. Currently, Airlines dropdown is not populated unless the Origin is selected.
        var airlinesString = airportUtil.buildairportairlineOptions();
        $(this.reference + "#targetAirlineDD").empty().append(airlinesString);
        var firstText = $(this.reference + '#targetAirlineDD').siblings("label").text().trim();
        if (firstText && /^Airline/g.test(firstText))
            $(this.reference + '#targetAirlineDD')[0].options[0].text = "-- Please select an Airline --";
    },
    initializeFlightType: function () {
        if ($(this.reference + '#targetFlightDD') && $(this.reference + '#targetFlightDD').length > 0) {
            var flightString = airportUtil.flightTypeListOptions();
            $(this.reference + '#targetFlightDD').append(flightString);
        }
    },
    initializeAircrafttType: function () {
        var aircraftString = airportUtil.aircraftTypeListOptions();
        $(this.reference + '#targetAircraftDD').append(aircraftString);
    },
    changeOrigin: function () {
        var selectedId = $(this.reference + "#targetOriginDD").val(), airlinesString;
        if("-1" !== selectedId){
            cacheMgr.selectedAirport(selectedId);
            airlinesString = airportUtil.airlineOriginDestOptions();
        }else{
            airlinesString = airportUtil.buildairportairlineOptions();
        }
        $(this.reference + "#targetAirlineDD").empty().append(airlinesString);
        var firstText = $(this.reference + '#targetAirlineDD').siblings("label").text().trim();
        if (firstText && /^Airline/g.test(firstText))
            $(this.reference + '#targetAirlineDD')[0].options[0].text = "-- Please select an Airline --";
    },
    retrieveTargets: function () {
        var me = this, data = communion, host = data.hostInstance;
        me.uri = getJsonInfoAction(RETRIEVETARGETS);
        me.targetsTable = $('#targetsTable').DataTable({
            "bProcessing": true,
            "bServerSide": true,
            "aLengthMenu": [20, 50, 100],
            "iDisplayLength": 50,
            "order": [[1, "asc"]],
            "pagingType": "simple",
            "bDestroy": true,
            "initComplete": function () {
                me.initializeInputEvent();
                me.listenToDataTableEvents();
                $("#targetsTable_filter input").off().on('keyup', function (e) {
                    if (e.keyCode == 13){
                        me.targetsTable.search(this.value.trim()).draw();
                        this.value = this.value.trim();
                        if(this.value.trim() === ''){
                            pageHelper.clearStickies();
                            pageHelper.notify(targetMessages.validSearch, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);                    
                        }
                    }
                });
                gebo_tips.init();
            },
            "sAjaxSource": me.uri,
            "fnServerData": function (sSource, aoData, fnCallback) {
                cnter = aoData[0].value;
                searchValue = aoData[55].value === '' ? '-1' : aoData[55].value,
                        sortValue = aoData[57].value === '' ? '-1' : aoData[57].value,
                        IsASC = aoData[58].value === "asc" ? true : false;
                switch (sortValue) {
                    case 1:
                        sortValue = "AirlineName";
                        break;
                    case 2:
                        sortValue = "Origin";
                        break;
                    case 3:
                        sortValue = "Type";
                        break;
                    case 4:
                        sortValue = "Target";
                        break;
                    case 5:
                        sortValue = "Route";
                        break;
                    case 6:
                        sortValue = "Direction";
                        break;
                    case 7:
                        sortValue = 'FlightType';
                        break;
                    case 8:
                        sortValue = 'AircraftType';
                        break;
                    default:
                        sortValue = '-1';
                        break;
                }
                //[#72964] Europe � Target Report, multiple issues for CT user.
                var formType = (host === 'USI' || host === 'AIR') ? me.returnValue($('#targetFormTypeDD').val()) : '-1';
                var route = host === 'USI' ? me.returnValue($('#targetRouteDD').val()) : '-1';
                var direction = host === 'USI' ? me.returnValue($('#targetDirectionDD').val()) : '-1';
                var flightType = host === 'EUR' ? me.returnValue($('#targetFlightDD').val()) : '-1';
                var aircraftType = host === 'AIR' ? me.returnValue($('#targetAircraftDD').val()) : '-1';
                var uri = me.uri + '/' +
                        aoData[3].value + '/' +
                        aoData[4].value + '/' +
                        formType + '/' +
                        me.returnValue($('#targetOriginDD').val()) + '/' +
                        me.returnValue($('#targetAirlineDD').val()) + '/' +
                        route + '/' +
                        direction + '/' +
                        flightType + '/' +
                        aircraftType + '/' +
                        searchValue + '/' +
                        sortValue + '/' +
                        IsASC;
                $.ajax({
                    "type": "GET",
                    "contentType": "application/json",
                    "url": uri,
                    "success": function (msg) {
                        dynamicTargets.targetResponse = msg;
                        fnCallback(
                                {
                                    "sEcho": cnter,
                                    "iTotalRecords": msg.RecordsCnt,
                                    "iTotalDisplayRecords": msg.RecordsCnt,
                                    "aaData": msg.TargetDetails === null ?
                                            [] : msg.TargetDetails
                                });
                    }
                })
                        .fail(function (result) {
                            console.log(result);
                        });
            },
            "columns": [
                { "data": "DistributionTargetId", sWidth: '6%' },
                { "data": "AirlineName", sWidth: '15%' },
                { "data": "Origin", sWidth: '15%' },
                { "data": "Type", sWidth: '15%', "visible": (host === 'USI' || host === 'AIR') ? true : false },
                { "data": "Target", sWidth: '15%', "sClass": "cCursorPointer" },
                { "data": "Route", sWidth: '10%', "visible": host === 'USI' ? true : false },
                { "data": "Direction", sWidth: '14%', "visible": host === 'USI' ? true : false },
                { "data": "FlightType", sWidth: '20%', "visible": host === 'EUR' ? true : false },
                { "data": "AircraftType", sWidth: '20%', "visible": host === 'AIR' ? true : false },
                { "data": "DistributionTargetId", sWidth: '10%' }
            ],
            aoColumnDefs: [
                {
                    "aTargets": [0],
                    'bSortable': false,
                },
                {
                    "aTargets": [3],
                    "mRender": function (data, type, full) {
                        if (data === 'D')
                            return 'Departure';
                        else
                            return 'Arrival';
                    }
                },
                {
                    "aTargets": [4],
                    "mRender": function (data, type, full) {
                        //return '<label id="dataLabel" class="cCursorPointer" onclick="dynamicTargets.editTargetsTable(this)">' + data + '</label><input id="dataInput" type="text" value=' + data + ' style="display:none;width:50px;" />';
                        return '<input id="dataInput" type="text" value=' + data + ' original="' + data + '" class="targetinput" />';
                    }
                },
                {
                    "aTargets": [9],
                    'bSortable': false,
                    "mRender": function (data, type, full) {
                        $('body').data(data.toString(), full);
                        return '<a href="#"  onclick="dynamicTargets.deleteTarget(this)" title="Delete"  class="sepDelEdit ttip_t" ><i class="fa fa-trash fa-lg btnColor"></i></a>';
                    }
                }
            ]
        });
    },
    returnValue: function (value) {
        var val;
        if (value === 'select')
            val = '-1';
        else
            val = value;
        return val;
    },
    deleteTarget: function (ele, id) {
        smoke.confirm(targetMessages.confirmDelete, function (e) {
            if (e) {
                var tr = $(ele).closest('tr');
                var row = dynamicTargets.targetsTable.row(tr);

                var rowIndex = row.index();
                var targetID = row.data().DistributionTargetId;
                var uri = getJsonInfoAction(DELETETARGETS);
                var postData = {
                    "Version": communion.Version,
                    "SessionId": sessionStorage.sessionId,
                    "Instance": getInstance(),
                    "DistributionTargetId": targetID
                };
                //  Delete if it is present in the updated list
                for (var k = 0; k < dynamicTargets.updateTarget.TargetUpdateDetails.length ; k++) {
                    if (dynamicTargets.updateTarget.TargetUpdateDetails[k].DistributionTargetId === targetID) {
                        dynamicTargets.updateTarget.TargetUpdateDetails.slice(k, 1);
                        break;
                    }
                }

                pageHelper.clearStickies();
                pageHelper.addSmokeSignal(targetMessages.deletingProgress);
                ajaxPost(uri, postData, '', dynamicTargets.deleteTargets, rowIndex, '', 'DELETE');
            }
        });
    },
    deleteTargets: function (data, success, rowIndex) {
        if (success)
            pageHelper.removeSmokeSignal();
        pageHelper.notify(targetMessages.deleteSuccess, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
        dynamicTargets.targetsTable.row(rowIndex).remove().draw(false);
    },
    saveChanges: function () {
        if (dynamicTargets.updateTarget.TargetUpdateDetails.length > 0) {
            var uri = getJsonInfoAction(UPDATETARGETS);
            pageHelper.addSmokeSignal(targetMessages.updating);
            ajaxPost(uri, dynamicTargets.updateTarget, '', dynamicTargets.acknowledgeSave, '', '', 'POST');
        } else {
            pageHelper.notify(targetMessages.emptyUpdates, pageUtils.msg_POSITION.top.RIGHT, pageUtils.msg_ERROR);
        }
    },
    acknowledgeSave: function (data, success) {
        if (success) {
            dynamicTargets.updateTarget.TargetUpdateDetails = [];
            pageHelper.removeSmokeSignal();
            pageHelper.notify(targetMessages.updateSuccess, pageUtils.msg_POSITION.top.RIGHT, pageUtils.msg_SUCCESS);
            window.onbeforeunload = null;
            //dynamicTargets.targetsTable.draw(false);
            $("#submitChanges").attr("disabled", true);
            dynamicTargets.initializeInputEvent();
        }
    },
    //[#73380] General - UI is allowing to submit the user with blank target.
    //Also user is not able to update the blank target value. Some time UI will keep on showing the message �updating changes, please wait��
    initializeInputEvent: function () {
        $("input[type=text]").focusout(function (e) {
            var isValid = false;
            var me = this, elemId = e.target.id, inputValue = me.value.replace(/\s+/g, ""), original = $(this).attr('original');
            if ('' !== original) {
                if ('' !== inputValue && inputValue.indexOf('.') === -1) {
                    if (!isNaN(parseFloat(inputValue)) && isFinite(inputValue) && parseInt(inputValue) > -1)
                        isValid = inputValue.length > 7 ? false : original !== me.value;
                }
                if (!isValid)
                    $(this).val(original);
                else {
                    $(this).attr('original', $(this).val())
                    var tr = $(this).closest('tr');
                    var row = dynamicTargets.targetsTable.row(tr);
                    dynamicTargets.enableSave = true;
                    var found = false;
                    for (var k = 0; k < dynamicTargets.updateTarget.TargetUpdateDetails.length ; k++) {
                        if (dynamicTargets.updateTarget.TargetUpdateDetails[k].DistributionTargetId === row.data().DistributionTargetId) {
                            dynamicTargets.updateTarget.TargetUpdateDetails[k].Target = $(this).val();
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        dynamicTargets.updateTarget.TargetUpdateDetails.push({
                            "DistributionTargetId": row.data().DistributionTargetId,
                            "Target": $(this).val()
                        });
                        $('#submitChanges').removeAttr('disabled');
                        window.onbeforeunload = function () {
                            return "You have some unsubmitted data,";
                        }
                    }
                }
            }
        });

        $('input[type=text]').keydown(function (e) {
            var keyCodes = [46, 8, 9, 27, 13];
            if ($.inArray(e.keyCode, keyCodes) !== -1 ||
                                    ((e.keyCode == 65 || e.keyCode == 67 || e.keyCode == 86 ||
                                            e.keyCode == 97 || e.keyCode == 99 || e.keyCode == 118 ||
                                            e.keyCode == 90 || e.keyCode == 122) &&
                                            e.ctrlKey === true) ||
                                            (e.keyCode >= 35 && e.keyCode <= 39)) {
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
            else {
                if (e.target.value.length > 6) {
                    e.preventDefault();
                }
            }
        });
    },
    listenToDataTableEvents: function () {
        var me = this;
        $('#targetsTable').on('draw.dt', function () {
            me.initializeInputEvent();
        });
    },
    showNewTarget: function () {
        this.reference = '.modal ';
        $(".sict_error").removeClass('sict_error');
        //  register for focus in and out evenet
        $(".modal select").focusin(function () {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('sict_error');
        });
        $(".modal select").focusout(function (e) {
            var target = $(e.target);
            if ("-1" === target.val())
                target.parent().addClass('sict_error');
            else
                target.parent().removeClass('sict_error');
        });
        //  restet all data
        $(".modal select").each(function (i, v) {
            $(this).find('option:first').prop('selected', 'selected');
        });
        $('.modal #targetInput').val('0');
    },
    addNewTagetCombination: function () {
        var host = communion.hostInstance;
        $(".sict_error").removeClass('sict_error');
        $(".modal select").each(function (i, v) {
            var ctrl = $(v), parentCtrl = ctrl.parent();
            if (ctrl.val() === '-1' && !parentCtrl.hasClass("sict_error"))
                parentCtrl.addClass("sict_error");
            else
                parentCtrl.removeClass("sict_error");
        });

        if ($(".modal .sict_error").length !== 0) {
            pageHelper.clearStickies();
            pageHelper.notify(targetMessages.validError, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }
        else {//    We have valid data to store
            var postObj = {
                "SessionId": sessionStorage.sessionId,
                "Version": communion.Version,
                "Instance": getInstance(),
                TargetDetail: {}
            },
            method = 'PUT',
            targetComboURL = getJsonInfoAction(ADDTARGET);

            postObj.TargetDetail.AirlineId = $('.modal #targetAirlineDD').val();
            postObj.TargetDetail.OriginId = $('.modal #targetOriginDD').val();
            postObj.TargetDetail.Type = $('.modal #targetFormTypeDD').val();
            postObj.TargetDetail.Route = host === 'USI' ? $('.modal #targetRouteDD').val() : null;
            postObj.TargetDetail.Direction = host === 'USI' ? $('.modal #targetDirectionDD').val() : null;
            postObj.TargetDetail.FlightType = host === 'EUR' ? $('.modal #targetFlightDD').val() : null;
            postObj.TargetDetail.AircraftType = host === 'AIR' ? $('.modal #targetAircraftDD').val() : null;
            postObj.TargetDetail.Target = $('.modal #targetInput').val();

            pageHelper.clearStickies();
            pageHelper.addSmokeSignal(targetMessages.addTargetProcessing);
            ajaxPost(targetComboURL, postObj, '', this.acknowledgeAddTarget, true, '', method);
        }
    },
    acknowledgeAddTarget: function (data, success, add) {
        if (success) {
            pageHelper.removeSmokeSignal();
            if (data.ReturnCode === 1) {
                var msg = targetMessages.targetAdded;
                dynamicTargets.targetsTable.ajax.reload();
                pageHelper.notify(msg, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
                $('#addTargetCombination').modal('hide');
            } else if (data.ReturnCode === 4) {
                pageHelper.notify(targetMessages.duplicateEntry, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            }
        }
    }
};