/*<Copyright> Cross-Tab </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> viewform.js </FileName>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var departureForms;
var depTable;
var downloadTimeout;
var formData = {
    FormFilters: {},
    isValid: false
};
var startIndex, offset, sortType, isAsc, searchVal;
var viewFormsMessage = {
    exportSuccess: 'Report successfully exported',
    exportFail: 'Sorry, eporting report failed.',
    initiateDownload: 'Please wait, while we initiate the download process...',
    successfulInitiation: 'Download successfully initiated, please wait till the button is enabled...',
    waitForDownload: 'Please wait, the download is in progress...',
    downloadError: 'Sorry, an error occoured while downloading, please try again later.',
    // [#73350] [CUSTOMER]:� The download has been successful, please click on button to export the file.�
    downloadSuccess: 'The download has been successful, please click on button to export the file.',
    validSearch : 'Please enter a valid value for search'
};

function getDataForDT() {
    var uri = getJsonInfoAction(VIEWFORMS);
    depTable = $('#departureTable').DataTable({
        "bProcessing": true,
        "bServerSide": true,
        "aLengthMenu": [20, 50, 100],
        "iDisplayLength": 100,
        "pagingType": "simple",
        "order": [[isGlobal ? 7 : 4, "desc"]],
        "sAjaxSource": uri,
        "initComplete": function () {
            $("#departureTable_filter input").off().on('keyup', function (e) {
                if (e.keyCode == 13){
                    //[#72591] User is getting the reports even for the space entered in the search text field. 
                    depTable.search(this.value.trim()).draw();
                    this.value = this.value.trim();
                    if(this.value.trim() === ''){
                        pageHelper.clearStickies();
                        pageHelper.notify(viewFormsMessage.validSearch, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);                    
                    }
                }
            });
        },
        "fnServerData": function (sSource, aoData, fnCallback) {
            cnter = aoData[0].value;
            selectedAirportID = $('#AirportList').val();
            var sValue = aoData[55].value, fValue = aoData[57].value, IsASC = aoData[58].value === "asc" ? true : false;
            if(!isGlobal){
                var sValue = aoData[55].value, fValue = aoData[57].value, IsASC = aoData[58].value === "asc" ? true : false;
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
                    case 5:
                        fValue = "LastUpdatedDate";
                        break;
                    default:
                        fValue = "Interviewer";
                        break;
                }
            }else{
                var sValue = aoData[55].value, fValue = aoData[57].value, IsASC = aoData[58].value === "asc" ? true : false;
                switch (fValue) {
                    //#73724] All instances:CT,MS:Global view data:Sort function for ID column is not working properly.
                    case 0:
                        fValue = "FormId";
                        break;
                    case 1:
                        fValue = "AirportCode";
                        break;
                    case 2:
                        fValue = "Interviewer";
                        break;
                    case 3:
                        fValue = "Airline";
                        break;
                    case 4:
                        fValue = "FlightNumber";
                        break;
                    case 5:
                        fValue = "Type";
                        break;    
                    case 6:
                        fValue = "Destination";
                        break;
                    case 7:
                        fValue = "DistributionDate";
                        break;
                    case 8:
                        fValue = "LastUpdatedDate";
                        break;
                    default:
                        fValue = "Interviewer";
                        break;
                }
            }
            startIndex = aoData[3].value, offset = aoData[4].value, searchVal = sValue, isAsc = IsASC, sortType = fValue;
            var postData = {
                "Version": "V1",
                "SessionId": sessionStorage.sessionId,
                "Instance": getInstance(),
                "DepartureFormFilterDetails": {
                    "StartIndex": aoData[3].value,
                    "OffSet": aoData[4].value,
                    "IsDepartureForm": isGlobal ? '-1' : isDeparture.toString(),
                    "AirportId": selectedAirportID,
                    "FilterValue": sValue,
                    "Sort": fValue,
                    "IsSortByAsc": IsASC
                }
            };
            var postExcelData = {
                "Version": "V1",
                "SessionId": sessionStorage.sessionId,
                "Instance": getInstance(),
                "FormFilterDetails": {
                    "StartIndex": aoData[3].value,
                    "OffSet": aoData[4].value,
                    "IsDepartureForm": isGlobal ? '-1' : isDeparture.toString(),
                    "AirportId": selectedAirportID,
                    "FilterValue": sValue,
                    "Sort": fValue,
                    "IsSortByAsc": IsASC
                }
            };
            if (formData.isValid)
                postData.DepartureFormFilterDetails.FormFilters = formData.FormFilters;

            if (formData.forExport) {
                postExcelData.FormFilterDetails.FormFilters = formData.FormFilters;
                postExportParameters(getJsonInfoAction(GLOBALEXPORT), postExcelData);
			}

            $.ajax({
                "type": "POST",
                "contentType": "application/json",
                "url": sSource,
                "data": JSON.stringify(postData),
                "success": function (msg) {
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
                    .fail(function (result) {
                        console.log(result);
                    });
        },
        "columns": getColumns(),
        aoColumnDefs: [
            {
                "aTargets": isGlobal ? [5] : [3],
                "mRender": function (data, type, full) {
                    if (isGlobal) {
                        if (data === 'D')
                            return 'Departure';
                        else
                            return 'Arrival';
                    } else {
                        return data;
                    }
                }
            },
            {
                "aTargets": isGlobal ? [7, 8] : [4, 5],
                "mRender": function (data, type, full) {
                    if ((data === null) || (data === undefined) || (data.length == 0)) {
                        return '';
                    } else {
                        return moment(data).format("MM/DD/YYYY");
                    }
                }
            },
            {
                "bSortable": false,
                "aTargets": isGlobal ? [9] : [6],
                "mRender": function (data, type, full) {
                    if ((data === null) || (data === 'undefined') || (data.length == 0)) {
                        return '';
                    } else {
                        var languageString = '';
                        for (var i = 0; i < data.length; i++) {
                            languageString += '<span>' + airportUtil.languageName(data[i].LanguageId) + '</span>';
                            if ((i + 1) < data.length)
                                languageString += '<br>';
                        }
                        return languageString;
                    }
                }
            },
			{
                "bSortable": false,
                "aTargets": isGlobal ? [10] : [7],
                "mRender": function (data, type, full) {
                    if ((data === null) || (data === 'undefined') || (data.length == 0)) {
                        return '';
                    } else {
                        var firstNo = '';
                        for (var i = 0; i < data.length; i++) {
                            firstNo += '<span>'+data[i].FirstSerialNo + '</span>';
                            if ((i + 1) < data.length)
                                firstNo += '<br>';
                        }
                        return firstNo;
                    }
                }
             },
			{
                "bSortable": false,
                "aTargets": isGlobal ? [11] : [8],
                "mRender": function (data, type, full) {
                    if ((data === null) || (data === 'undefined') || (data.length == 0)) {
                        return '';
                    } else {
                        var lastNo = '';
                        for (var i = 0; i < data.length; i++) {
                            lastNo += '<span>'+data[i].LastSerialNo + '</span>';
                            if ((i + 1) < data.length)
                                lastNo += '<br>';
                        }
                        return lastNo;
                    }
                }
            },
            {
                "bSortable": false,
                "aTargets": isGlobal ? [11] : [9] ,
                "mRender": function (data, type, full) {
                    $('body').data(data.toString(), full);
                    return '<a href="#editAirlinePopUp" data-toggle="modal" data-backdrop="static"  onclick="callEditAirlines(this,' + data + ')" class="sepDelEdit ttip_t" title="Edit Form Entry"><i class="fa fa-pencil-square-o fa-lg btnColor"></i></a>' +
                            '<a href="javascript:void(0);" onclick="callDeleteAirline(this, ' + data + ')" title="Delete Form Entry"  class="sepDelEdit ttip_t" ><i class="fa fa-trash fa-lg btnColor"></i></a>';
                }
            }
        ]
    });
}

function getColumns() {
    if (isGlobal) {
        return [
            { "data": "FormId", sWidth: '4%' },
            { "data": "AirportCode", sWidth: '7%' },
            { "data": "Interviewer", sWidth: '12%' },
            { "data": "Airline", sWidth: '10%' },
            { "data": "FlightNumber", sWidth: '4%' },
            { "data": "Type", sWidth: '4%' },
            { "data": "Destination", sWidth: '12%' },
            { "data": "DistributionDate", "sType": "eu_date", sWidth: '5%' },
            { "data": "LastUpdatedDate", "sType": "eu_date", sWidth: '5%' },
            //{ "data": "Languages", "sClass": "tblSpan" },
			{ "data": "Languages",  sWidth: '6%' },
			{ "data": "Languages",  sWidth: '5%' },
		    { "data": "Languages",  sWidth: '5%' }
        ]
    } else {
        return [
            { "data": "Interviewer", sWidth: '15%' },
            { "data": "Airline", sWidth: '15%' },
            { "data": "FlightNumber", sWidth: '5%' },
            { "data": "Destination", sWidth: '15%' },
            { "data": "DistributionDate", "sType": "eu_date", sWidth: '5%' },
            { "data": "LastUpdatedDate", "sType": "eu_date", sWidth: '5%' },
            //{ "data": "Languages", "sClass": "tblSpan" },
			{ "data": "Languages",  sWidth: '5%' },
			{ "data": "Languages",  sWidth: '5%' },
			{ "data": "Languages",  sWidth: '5%' },
			//{ "data": "Languages",  sWidth: '5%' }
            //Changes for Super Airport{ "data": "FormId", "visible": (sessionStorage.roleId === '2' ? false : true) }
			{ "data": "FormId", "visible": (sessionStorage.roleId === '2' ? ((sessionStorage.IsSpecialUser === "true") ? true : false) : true) }
        ]
    }
}

$('#departureTable').on('draw.dt', function () {
    gebo_tips.init();
});

var cnter = 0,
        slRI = 0,
        needToClear = true; //  Flag to show the success sticky message
var callViewDepForm = function (callback, uri) {
    var postData = {
        "Version": "V1",
        "SessionId": sessionStorage.sessionId,
        "Instance": getInstance(),
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
    }).done(function (result) {
        departureForms = result;
        result.sEcho = ++cnter;
        result.iTotalRecords = result.GetDepartureFormDetailsResult.TotalRecords;
        result.iTotalDisplayRecords = result.GetDepartureFormDetailsResult.TotalRecords;
        callback({ data: result.GetDepartureFormDetailsResult.DepartureFormDetails });
    })
            .fail(function (result) {
                console.log(result);
            });
}








function callEditAirlines(ele, data) {
    var sEle;
    needToClear = true;
    sEle = $('body').data(data.toString());
    var tr = $(ele).closest('tr');
    var row = depTable.row(tr);
    slRI = row.index();
	console.log(sEle);
    if (null !== sEle && 'undefined' !== sEle) {
        $('body').data('EditedForm', sEle);
        var htmlContent = newformDeparture.newAirport() + newformDeparture.newAirline('show');
        var template = jQuery.validator.format($.trim((htmlContent)));
        $('#editAirlineBody').empty().append($(template(1)));
        ////[#72790] [Customer]:Editing entries through View form Entries :
        //newFormUtils.initializeAirports("EditedAirportList", sEle.AirportId, true);
		//Change for Special Airport
		newFormUtils.initializeAirports("EditedAirportList", sEle.AirportId, (sessionStorage.IsSpecialUser === "true" ? false : true));
        newFormUtils.initializeIntervieweControls("InterviewerList", sEle.InterviewerId);
        newFormUtils.initializeDateControls("fieldworkDate", sEle.DistributionDate);
		
		    	//code added by Adil Shaikh
        if (sessionStorage.IsSpecialUser === "true") {
        	$('#airlinesDD1').empty();
        	$("#formAirlinesDD option").each(function () {
        		$('#airlinesDD1').append($(this));
        	});


        	var languages = jQuery.parseJSON(sessionStorage.getItem("languagesList"));
        	$('#language1_1').empty().append('<option value="-1">-- Please select a language --</option>');
        	$('#language1_2').empty().append('<option value="-1">-- Please select a language --</option>');

        	$.each(languages, function (index, element) {
        		$('#language1_1').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');
        	});
        	$('#formAirlinesDD').append('<option value="-1">-- Please select an Airline --</option>');
        	$('#formAirlinesDD').append('<option value="31" direction="WEST" flightid="1350" route="EURASIA" flighttype="null" originid="93">Vietnam Airlines (VN)</option>');

        }
        //end of code added by Adil Shaikh

        //Detination


        $('#airlinesDD1').empty().append('<option value="-1">-- Please select an Airline --</option>');
        $('#airlinesDD1').empty().append(airportUtil.airlineListOptions(isDeparture));
		// $('#airlinesDD1').append('<option value="'+sEle.AirlineId+'" selected>'+sEle.Airline+'</option>');
		 callViewDestinationPage(sEle.AirportId);		
		
        //  Init the validations
        //  Populate the user preference
        var sOpt = $('#airlinesDD1')[0].options;
        for (var i = 0; i < sOpt.length; i++) {
            if (sOpt[i].value === sEle.AirlineId.toString()) {
                sOpt[i].selected = true;
                callOnAirlinesChange(1);
                break;
            }
        }

        //Origin

        var sDOpt = $('#destination1')[0].options;
        for (var i = 0; i < sDOpt.length; i++) {
            if (sDOpt[i].value === sEle.DestinationId.toString()) {
                sDOpt[i].selected = true;
                break;
            }
        }
        //Aircraft Type
        if ($('#aircrafttype1').length > 0) {
            var sairCraftOpt = $('#aircrafttype1')[0].options;
            for (var i = 0; i < sairCraftOpt.length; i++) {
                if (sairCraftOpt[i].value === sEle.AircraftType.toString()) {
                    sairCraftOpt[i].selected = true;
                    break;
                }
            }
        }
        var cnt = sEle.Languages.length - 1;
        while (cnt > 0) {
            $('button.btn.sepH_b.btn-pink.addLanguage')[0].click();
            --cnt;
        }
		
		//code added by Adil Shaikh
        if ( sessionStorage["IsSpecialUser"] === "true" ) {
        	var languages = jQuery.parseJSON(sessionStorage.getItem("languagesList"));
        	$('#language1_2').empty().append('<option value="-1">-- Please select a language --</option>');
        	$('#language1_3').empty().append('<option value="-1">-- Please select a language --</option>');
        	$('#language1_4').empty().append('<option value="-1">-- Please select a language --</option>');
        	$('#language1_5').empty().append('<option value="-1">-- Please select a language --</option>');

        	$.each(languages, function (index, element) {
        		$('#language1_2').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');
        		$('#language1_3').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');
        		$('#language1_4').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');
        		$('#language1_5').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');

        	});

        }
    	//end of code added by Adil Shaikh
		
        for (var i = 0; i < sEle.Languages.length; i++) {
            $('#fserial1_' + (i + 1).toString()).val(parseInt(sEle.Languages[i].FirstSerialNo.toString().trim()));
            $('#lserial1_' + (i + 1).toString()).val(parseInt(sEle.Languages[i].LastSerialNo.toString().trim()));
            var sLOpt = $('#language1_' + (i + 1).toString())[0].options;
            for (var x = 0; x < sLOpt.length; x++) {
                if (sLOpt[x].value === sEle.Languages[i].LanguageId.toString()) {
                    sLOpt[x].selected = true;
                    break;
                }
            }
        }

        //$('#flight1').val(sEle.FlightNumber.match(/\d+/)[0]); //  Airlines with code U2 will not work
        //[#73530] USinternational - Extra space is getting added by default in Flight No of Arrival/departure edit form
        $('#flight1').val(parseInt(sEle.FlightNumber.substring(2).toString().trim()));
        $('#businesscard1').val(parseInt(sEle.BusinessCards.toString().trim()));
        newformDeparture.handleFocusIn();
        newformDeparture.checkValueisNumber('');
        $('#flight1').trigger('focusout');
        $('#editAirlinePopUp').on('hidden.bs.modal', function (e) {
            if (needToClear)
                pageHelper.clearStickies();
        });
    }
}

function bindlanguage(){

	var languages = jQuery.parseJSON(sessionStorage.getItem("languagesList"));
        	$('#language1_2').empty().append('<option value="-1">-- Please select a language --</option>');
        	$('#language1_3').empty().append('<option value="-1">-- Please select a language --</option>');
        	$('#language1_4').empty().append('<option value="-1">-- Please select a language --</option>');
        	$('#language1_5').empty().append('<option value="-1">-- Please select a language --</option>');

        	$.each(languages, function (index, element) {
        		$('#language1_2').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');
        		$('#language1_3').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');
        		$('#language1_4').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');
        		$('#language1_5').append('<option value="' + element.LanguageId + '">' + element.LanguageName + '</option>');

        	});
}
function callDeleteAirline(ele, id) {
    smoke.confirm('Are you sure you want to delete?', function (e) {
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

function callPopUpClose(data) {
    if (null !== data && undefined !== data) {
        var d = depTable.row(slRI).data();
        d.FlightNumber = data.Airlines[0].FlightNumber;
        d.Languages = data.Airlines[0].Languages;
        d.BCardsDistributed = data.Airlines[0].BCardsDistributed;
        depTable.row(slRI).data(d);
        depTable.row(slRI).invalidate().draw();
        pageHelper.notify(newFormMessages.SUCCESS_AIRLINE_UPDATE, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
        needToClear = false;
    }
    $('#popUpClose').click();
}

function updateFormEntries(ele) {
    var sEle = $('body').data('EditedForm');
    validator.updateForm(ele, sEle, callPopUpClose);
}

function callViewFormsPage() {
    var selectedAirportID = $('#AirportList').val();
    cacheMgr.selectedAirport(selectedAirportID);
	airlineString = airportUtil.airlineListOptions(isDeparture);
    $('#formInterviewerDD').empty().append(airportUtil.interviewerListOptions());
    $('#formAirlinesDD').empty().append(airlineString);
    var airlineVsProps = cacheMgr.airlineList(isDeparture);
    if (isDeparture)
        var airlineDestinatons = '<option value="-1">-- Please select a destination --</option>';
    else
        var airlineDestinatons = '<option value="-1">-- Please select a origin --</option>';
    //Lathendra
	//if(airlineVsProps && airlineVsProps.length > 0){
        for (var j = 0; j < airlineVsProps.length; j++) {
            airlineVsProps[j].destinations.sort(function (a, b) { return (a).destName.localeCompare((b).destName) });
            for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
                airlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
            //break;
        }    
	//}
    $('#formDestinationDD').empty().append(airlineDestinatons);
}
//----added by yashas---------
function callViewDestinationPage(val) {
    var selectedAirportID = val;
    cacheMgr.selectedAirport(selectedAirportID);
	airlineString = airportUtil.airlineListOptions(isDeparture);
    $('#formInterviewerDD').empty().append(airportUtil.interviewerListOptions());
    $('#formAirlinesDD').empty().append(airlineString);
    var airlineVsProps = cacheMgr.airlineList(isDeparture);
    if (isDeparture)
        var airlineDestinatons = '<option value="-1">-- Please select a destination --</option>';
    else
        var airlineDestinatons = '<option value="-1">-- Please select a origin --</option>';
    //Lathendra
	//if(airlineVsProps && airlineVsProps.length > 0){
        for (var j = 0; j < airlineVsProps.length; j++) {
            airlineVsProps[j].destinations.sort(function (a, b) { return (a).destName.localeCompare((b).destName) });
            for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
                airlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
            //break;
        }    
	//}
    $('#destination1').empty().append(airlineDestinatons);
}

function arrUnique(arr) {
    var cleaned = [];
    arr.forEach(function(itm) {
        var unique = true;
        cleaned.forEach(function(itm2) {
            if (_.isEqual(itm, itm2)) unique = false;
        });
        if (unique)  cleaned.push(itm);
    });
    return cleaned;
}

function initialiseViewForms() {

if(sessionStorage["IsSpecialUser"] === "true")
	sessionStorage.removeItem("selectedAirportId");
	
    airportUtil.initialiseDates('dp_start', 'dp_end');

    $('#formInterviewerDD').empty().append(airportUtil.interviewerListOptions);
    $('#formAirlinesDD').empty().append(airportUtil.airlineListOptions(isDeparture));
    var airlineVsProps = cacheMgr.airlineList(isDeparture);
	var singleObject = [];
    //[#73606] [Customer_mindset]:View form page to show additional filter options.
    if (isDeparture)
        var airlineDestinatons = '<option value="-1">-- Please select a destination --</option>';
    else
        var airlineDestinatons = '<option value="-1">-- Please select a origin --</option>';
	//Changes for Special AirportCode
	//if(airlineVsProps && airlineVsProps.length > 0){
        for (var j = 0; j < airlineVsProps.length; j++) {
			for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
				singleObject.push(airlineVsProps[j].destinations[k]);
            //break;
        }   
	//}
	var uniqueObject = arrUnique(singleObject);
	uniqueObject = uniqueObject.sort(function (a, b) { return (a).destName.localeCompare((b).destName) });
	for (var k = 0; k < uniqueObject.length; k++)
		airlineDestinatons += '<option value=' + uniqueObject[k].destId + '>' + uniqueObject[k].destName + '</option>';
		
    $('#formDestinationDD').empty().append(airlineDestinatons);
}

function callFormAirlinesChange() {
    var airlineVsProps = cacheMgr.airlineList(isDeparture);
    var airlineId = parseInt(document.getElementById("formAirlinesDD").value);
	var singleObject = [];
	
    if (isDeparture)
        var airlineDestinatons = '<option value="-1">-- Please select a destination --</option>';
    else
        var airlineDestinatons = '<option value="-1">-- Please select a origin --</option>';
    if (-1 !== airlineId) {
        for (var j = 0; j < airlineVsProps.length; j++) {
            if (airlineId === airlineVsProps[j].id) {
                //[#72744] [Customer]:View Departure Forms- Need to show Destinations in alphabetical order under Destination filter.
                airlineVsProps[j].destinations.sort(function (a, b) { return (a).destName.localeCompare((b).destName) });
                for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
                    airlineDestinatons += '<option value=' + airlineVsProps[j].destinations[k].destId + '>' + airlineVsProps[j].destinations[k].destName + '</option>';
                //break;
            }
        }
    }else{
        for (var j = 0; j < airlineVsProps.length; j++) {
            //airlineVsProps[j].destinations.sort(function (a, b) { return (a).destName.localeCompare((b).destName) });
            for (var k = 0; k < airlineVsProps[j].destinations.length; k++)
				singleObject.push(airlineVsProps[j].destinations[k]);
            //break;
        }
		var uniqueObject = arrUnique(singleObject);
		uniqueObject = uniqueObject.sort(function (a, b) { return (a).destName.localeCompare((b).destName) });
		for (var k = 0; k < uniqueObject.length; k++)
			airlineDestinatons += '<option value=' + uniqueObject[k].destId + '>' + uniqueObject[k].destName + '</option>';
    }
    $('#formDestinationDD').empty().append(airlineDestinatons);
}

function updateViewForms() {
    //Code to collapse the filter accordian.
    $('#formHdr').toggle('200');
    $('#generateViewForms').parents().find('#parentDiv').find('i').removeClass('fa-arrow-circle-up').addClass('fa-arrow-circle-down').attr('title', 'Expand');
    $('#parentDiv').removeClass('formSep');

    //Code to update the filter to the post data object.
    var formFilters = {};
    formFilters.AirlineId = $('#formAirlinesDD').val();
    formFilters.DestinationId = $('#formDestinationDD').val();
    formFilters.FlightNumber = $('#formFlightNo').val();
    formFilters.InterviewerId = $('#formInterviewerDD').val();
    var date = $('#dp_start').data('date');
    formFilters.StartDate = date === undefined ? null : pageUtils.returnServerDate(date);
    var date = $('#dp_end').data('date');
    formFilters.EndDate = date === undefined ? null : pageUtils.returnServerDate(date);
    formData.FormFilters = formFilters;
    formData.isValid = true;
    formData.forExport = false;

    //Call the DataTable draw function to redraw with the new parameters.
    depTable.draw();
}

function exportGlobalForms() {
    //Code to update the filter to the post data object.
    var FormData = {},
            FormFilterDetails = {},
            FormFilters = {},
            airline = $('#formAirlinesDD option:selected'),
            destination = $('#formDestinationDD option:selected'),
            interviewer = $('#formInterviewerDD option:selected'),
            airport = $('#AirportList option:selected');

    FormData.Version = communion.Version;
    FormData.SessionId = sessionStorage.sessionId;
    FormData.Instance = getInstance();

    FormFilterDetails.StartIndex = startIndex;
    FormFilterDetails.OffSet = offset;
    FormFilterDetails.IsDepartureForm = isGlobal ? '-1' : isDeparture.toString();
    FormFilterDetails.AirportId = airport.val();
    FormFilterDetails.FilterValue = searchVal;
    FormFilterDetails.Sort = sortType;
    FormFilterDetails.IsSortByAsc = isAsc;

    FormFilters.AirlineId = airline.val();
    FormFilters.DestinationId = $('#formDestinationDD').val();
    FormFilters.FlightNumber = $('#formFlightNo').val();
    FormFilters.InterviewerId = interviewer.val();
    FormFilters.AirlineName = airline.val() === "-1" ? null : airline.text();
    FormFilters.DestinationName = destination.val() === "-1" ? null : destination.text();
    FormFilters.AirportName = airport.val() === "-1" ? null : airport.text();
    FormFilters.InterviewerName = interviewer.val() === "-1" ? null : interviewer.text();
    var startDate = $('#dp_start').data('date');
    FormFilters.StartDate = startDate === undefined ? null : pageUtils.returnServerDate(startDate);
    var endDate = $('#dp_end').data('date');
    FormFilters.EndDate = endDate === undefined ? null : pageUtils.returnServerDate(endDate);

    FormFilterDetails.FormFilters = FormFilters;
    FormData.FormFilterDetails = FormFilterDetails;

    var uri = getJsonInfoAction(GLOBALEXPORT);
    pageHelper.clearStickies();
    pageHelper.addSmokeSignal(viewFormsMessage.initiateDownload);
    ajaxPost(uri, FormData, '', checkExportStatus, '', '', 'POST');
    $('#exportViewFormsLink').addClass('no-show');
}

function checkExportStatus(data, success) {
    if (success) {
        if (data.DownloadFormDetailsResult.ReturnCode === 1) {
            $('#spinnerIndicator').removeClass('no-show');
            $('#exportViewForms').prop('disabled', true);
            pageHelper.removeSmokeSignal();
            pageHelper.notify(viewFormsMessage.successfulInitiation, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);

            var uri = getJsonInfoAction(CHECKEXPORTSTATUS) + '/' + data.DownloadFormDetailsResult.FileName;
            downloadTimeout = setInterval(function () {
                ajaxPost(uri, '', '', confirmDownloadSuccess, data.DownloadFormDetailsResult.FileLink, '', 'GET');
            }, 15000);
        } else {
            pageHelper.removeSmokeSignal();
            pageHelper.notify(viewFormsMessage.downloadError, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);
        }
    }
}

function confirmDownloadSuccess(data, success, fileLink) {
    if (success) {
        if (data.IsDownloadComplete) {
            //[#72938] Europe – Global View Form, multiple issues for CT login.
            $('#spinnerIndicator').addClass('no-show');
            $('#exportViewFormsLink').removeClass('no-show');
            $('#exportViewFormsLink').prop('href', fileLink);
            $('#exportViewForms').prop('disabled', false);

            pageHelper.notify(viewFormsMessage.downloadSuccess, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
            clearInterval(downloadTimeout);
        } else {
            pageHelper.notify(viewFormsMessage.waitForDownload, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);
        }
    } else {
        pageHelper.notify(viewFormsMessage.downloadError, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        $('exportViewFormsLink').prop('href', '');
        $('#exportViewForms').prop('disabled', false);
    }
}

function postExportParameters(url, data) {
    $.ajax({
        "type": "POST",
        "contentType": "application/json",
        "url": url,
        "data": JSON.stringify(data),
        "success": function (res) {
            if (res && res.DownloadFormDetailsResult && res.DownloadFormDetailsResult.ReturnCode === 1) {
                $("#forDwnld").attr("href", encodeURI(res.DownloadFormDetailsResult.Path));
                $("#forDwnld").attr("download", encodeURI(res.DownloadFormDetailsResult.Path).substring(encodeURI(res.DownloadFormDetailsResult.Path).lastIndexOf("/") + 1, (encodeURI(res.DownloadFormDetailsResult.Path).lastIndexOf("."))));
                document.getElementById('forDwnld').click();
            }
        }
    }).fail(function (result) {
        console.log(result);
    });
}

function departureExportExcel(n,name){
	$("#" + n).table2excel({
		exclude: ".noExl",
		name: "Report",
		filename: name,
		exclude_img: true,
		exclude_links: true,
		exclude_inputs: true
		// dtMode: true,
		// qtMode: isQtMode === undefined ? false : true,
		// expandHeader: 'departureTable' === n ? true : false
	});
	pageUtils.notify("Successfully Exported.", 'top-right', pageUtils.msg_SUCCESS);
}