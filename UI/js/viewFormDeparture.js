var departureForms;
var depTable;
gebo_tips = {
		init: function() {
			if(!is_touch_device()){
				var shared = {
				style		: {
						classes: 'ui-tooltip-shadow ui-tooltip-tipsy'
					},
					show		: {
						delay: 100
					},
					hide		: {
						delay: 0
					}
				};
				if($('.ttip_b').length) {
					$('.ttip_b').qtip( $.extend({}, shared, {
						position	: {
							my		: 'top center',
							at		: 'bottom center',
							adjust: { screen: true }
						}
					}));
				}
				if($('.ttip_t').length) {
					$('.ttip_t').qtip( $.extend({}, shared, {
						position: {
							my		: 'bottom center',
							at		: 'top center',
							adjust: { screen: true }
						}
					}));
				}
				if($('.ttip_l').length) {
					$('.ttip_l').qtip( $.extend({}, shared, {
						position: {
							my		: 'right center',
							at		: 'left center',
							adjust: { screen: true }
						}
					}));
				}
				if($('.ttip_r').length) {
					$('.ttip_r').qtip( $.extend({}, shared, {
						position: {
							my		: 'left center',
							at		: 'right center',
							adjust: { screen: true }
						}
					}));
				};
			}
		}
	};
	
function getDataForDT(){

    var uri = getJsonInfoAction(VIEWFORMS);


    depTable = $('#departureTable').DataTable({
        "bJQueryUI": true,
        "bProcessing": true,
        "bServerSide": true,
        // "sPaginationType": "two_button",
        "aLengthMenu": [20, 50, 100],
        "iDisplayLength": 50,
        "pagingType": "simple",
        "order": [[ 4, "desc" ]],
        "sAjaxSource": uri,
        "initComplete": function() {
            $("#departureTable_filter input").off().on('keyup', function(e) {
                if (e.keyCode == 13)
                    depTable.search(this.value).draw();
            });
        },
        "fnServerData": function(sSource, aoData, fnCallback) {
            cnter = aoData[0].value;
            selectedAirportID = $('#AirportList').val();
            var sValue = aoData[40].value, fValue = aoData[42].value, IsASC = aoData[43].value === "asc" ? true : false;
            switch (fValue) {
                case 0:
                    fValue = "Interviewer";
                    break;
                case 1:
                    fValue = "Airline";
                    break;
                case 2:
                    fValue = "FlightNumber";
                    break;
                case 3:
                    fValue = "Destination";
                    break;
                case 4:
                    fValue = "DistributionDate";
                    break;
                default:
                    fValue = "Interviewer";
                    break;
            }
            var postData = {
                "Version": "V1",
                "SessionId": sessionStorage.sessionId,
                "DepartureFormFilterDetails": {
                    "StartIndex": aoData[3].value,
                    "OffSet": aoData[4].value,
                    "IsDepartureForm": isDeparture.toString(),
                    "AirportId": selectedAirportID,
                    "FilterValue": sValue,
                    "Sort": fValue,
                    "IsSortByAsc": IsASC
                }
            };
            $.ajax({
                "type": "POST",
                "contentType": "application/json",
                "url": sSource,
                "data": JSON.stringify(postData),
                "success": function(msg) {
                    //departureForms = msg;
                    fnCallback(
                            {
                                "sEcho": cnter,
                                "iTotalRecords": msg.GetDepartureFormDetailsResult.TotalRecords,
                                "iTotalDisplayRecords": msg.GetDepartureFormDetailsResult.TotalRecords,
                                "aaData": msg.GetDepartureFormDetailsResult.DepartureFormDetails === null ?
                                        [] : msg.GetDepartureFormDetailsResult.DepartureFormDetails
                            });
                }
            })
                    .fail(function(result) {
                        console.log(result);
                    });
        },
        "columns": [
            {"data": "Interviewer", sWidth: '15%'},
            {"data": "Airline", sWidth: '15%'},
            {"data": "FlightNumber", sWidth: '5%'},
            {"data": "Destination", sWidth: '15%'},
            {"data": "DistributionDate", "sType": "eu_date", sWidth: '5%'},
            {"data": "Languages", "sClass": "tblSpan"},
            {"data": "FormId"}
        ],
        aoColumnDefs: [
            {
                "aTargets": [4],
                "mRender": function(data, type, full) {
                    if ((data === null) || (data === undefined) || (data.length == 0)) {
                        return '';
                    } else {
                        return moment(data).format("MM/DD/YYYY");
                    }
                }
            },
            {
                "bSortable": false,
                "aTargets": [5],
                "mRender": function(data, type, full) {
                    if ((data === null) || (data === 'undefined') || (data.length == 0)) {
                        return '';
                    } else {
                        var languageString = '';
                        for (var i = 0; i < data.length; i++) {
                            languageString += '<span>' + airportUtil.languageName(data[i].LanguageId) + '</span><span>' + data[i].FirstSerialNo + '</span><span>' + data[i].LastSerialNo + '</span>';
                            if ((i + 1) < data.length)
                                languageString += '<br>';
                        }
                        return languageString;
                    }
                }
            },
            {
                "bSortable": false,
                "aTargets": [6],
                "mRender": function(data, type, full) {
                    $('body').data(data.toString(), full);
                    return '<a href="#editAirlinePopUp" data-toggle="modal" data-backdrop="static"  onclick="callEditAirlines(this,' + data + ')" class="sepDelEdit" title="Edit"><i class="fa fa-pencil-square-o fa-lg btnColor"></i></a>' +
                            '<a href="#" onclick="callDeleteAirline(this, ' + data + ')" title="Delete"  class="sepDelEdit" ><i class="fa fa-trash fa-lg btnColor"></i></a>';

                }
            }
        ]
    });
}
var cnter = 0, slRI = 0;
var callViewDepForm = function(callback, uri) {

    var postData = {
        "Version": "V1",
        "SessionId": sessionStorage.sessionId,
        "DepartureFormFilterDetails": {
            "StartIndex": "0",
            "OffSet": "100"
        }
    };

    $.ajax({
        type: "POST",
        url: uri,
        data: JSON.stringify(postData),
        contentType: "application/json" // content type sent to server
    }).done(function(result) {
        departureForms = result;
        result.sEcho = ++cnter;
        result.iTotalRecords = result.GetDepartureFormDetailsResult.TotalRecords;
        result.iTotalDisplayRecords = result.GetDepartureFormDetailsResult.TotalRecords;
        callback({data: result.GetDepartureFormDetailsResult.DepartureFormDetails});
    })
            .fail(function(result) {
                console.log(result);
            });
}

function callEditAirlines(ele, data) {
    var sEle;
    sEle = $('body').data(data.toString());
    var tr = $(ele).closest('tr');
    var row = depTable.row(tr);
    slRI = row.index();
    if (null !== sEle && 'undefined' !== sEle) {
        $('body').data('EditedForm', sEle);
        var template = jQuery.validator.format($.trim((newformDeparture.newAirline('show'))));
        $('#editAirlineBody').empty().append($(template(1)));
        //  Init the validations
        //newformDeparture.handleFocusIn();
        //newformDeparture.checkValueisNumber('');
        //  Populate the user preference
        var sOpt = $('#airlinesDD1')[0].options;
        for (var i = 0; i < sOpt.length; i++) {
            if (sOpt[i].value === sEle.AirlineId.toString()) {
                sOpt[i].selected = true;
                callOnAirlinesChange(1);
                break;
            }
        }
        var sDOpt = $('#destination1')[0].options;
        for (var i = 0; i < sDOpt.length; i++) {
            if (sDOpt[i].value === sEle.DestinationId.toString()) {
                sDOpt[i].selected = true;
                break;
            }
        }
        var cnt = sEle.Languages.length - 1;
        while (cnt > 0) {
            $('button.btn.sepH_b.btn-pink.addLanguage')[0].click();
            --cnt;
        }
        for (var i = 0; i < sEle.Languages.length; i++) {
            $('#fserial1_' + (i + 1).toString()).val(sEle.Languages[i].FirstSerialNo);
            $('#lserial1_' + (i + 1).toString()).val(sEle.Languages[i].LastSerialNo);
            var sLOpt = $('#language1_' + (i + 1).toString())[0].options;
            for (var x = 0; x < sLOpt.length; x++) {
                if (sLOpt[x].value === sEle.Languages[i].LanguageId.toString()) {
                    sLOpt[x].selected = true;
                    break;
                }
            }
        }
		
        $('#flight1').val(sEle.FlightNumber.match(/\d+/)[0]);
        $('#businesscard1').val(sEle.BusinessCards);
        newformDeparture.handleFocusIn();
        newformDeparture.checkValueisNumber('');
    }
}

function callDeleteAirline(ele, id) {
    smoke.confirm('Are you sure you want to delete?', function(e) {
        if (e) {
            var tr = $(ele).closest('tr');
            var row = depTable.row(tr);
            var rowIndex = row.index();
            var uri = getJsonInfoAction(VIEWFORMSDELETE);
            var postData = {
                "Version": "V1",
                "SessionId": sessionStorage.sessionId,
                "FormId": id
            };
            ajaxPost(uri, postData, '', deleteAirline, rowIndex, '', 'DELETE');
        }
    });
}

function deleteAirline(data, success, rowIndex) {
    if (success)
        smoke.alert('Airline deleted successfully!');
    depTable.row(rowIndex).remove().draw(false);
}    

function initialiseAirportName() {
    var airportURL = "./cache/AirportList/AirportList_" + sessionStorage.airportLoginId + ".json";
    if (sessionStorage.roleId !== '0') {
        airportURL = "./cache/AirportList/AirportList_Admin.json";
    }
    $.getJSON(airportURL, function(response, status) {
        if (status === "success") {
            var optionString = '';
            for (var i = 0; i < response.length; i++) {
                optionString += '<option value=' + response[i].AirportId + '>' + response[i].AirportName + '</option>';
            }
            $('#AirportList').append(optionString);

            if ($('#AirportList')[0].length === 2) {
                $('#AirportList')[0].disabled = true;
                $('#AirportList')[0].selectedIndex = 1;
            }

        }
    });
}

function callPopUpClose(data) {
    if (null !== data && undefined !== data) {
        var d = depTable.row(slRI).data();
        d.FlightNumber = data.Airlines[0].FlightNumber;
        d.Languages = data.Airlines[0].Languages;
        d.BCardsDistributed = data.Airlines[0].BCardsDistributed;
        depTable.row(slRI).data(d);
        depTable.row(slRI).invalidate().draw();
    }
    $('#popUpClose').click();

}

function updateFormEntries(ele) {
    var sEle = $('body').data('EditedForm');
    newformDeparture.submitEditDeparture(ele, sEle, callPopUpClose);
}


function callViewFormsPage() {

    selectedAirportID = $('#AirportList').val();
    depTable.draw();

}
