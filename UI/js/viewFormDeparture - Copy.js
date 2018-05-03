var departureForms;
var depTable;
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
            // "ajax": function(data, callback, settings) {
                    // callViewDepForm(callback, uri);
                // },
			"sAjaxSource": uri, 
			"fnServerData": function(sSource, aoData, fnCallback) {
				cnter = aoData[0].value;
				var postData = {
					"Version":"V1",
					"SessionId":sessionStorage.sessionId,
					"DepartureFormFilterDetails":{
					   "StartIndex":aoData[3].value,
					   "OffSet":aoData[4].value,
					   "IsDepartureForm":isDeparture.toString(),
					    "AirportId":sessionStorage.airportLoginId
					}
				};
			   $.ajax({
					"type": "POST",					
					"contentType": "application/json",
					"url": sSource, 
					"data": JSON.stringify(postData),
					"success": function (msg) {
						//departureForms = msg;
						fnCallback(
						{
							"sEcho": cnter,
							"iTotalRecords": msg.GetDepartureFormDetailsResult.TotalRecords,
							"iTotalDisplayRecords": msg.GetDepartureFormDetailsResult.TotalRecords,
							"aaData": msg.GetDepartureFormDetailsResult.DepartureFormDetails === null ? 
                                                                [] :  msg.GetDepartureFormDetailsResult.DepartureFormDetails
						});
					}
				})
				.fail(function(result) {
					console.log(result);                
				});
		   },
            "columns": [
                    {"data": "Interviewer", sWidth : '15%'},
                    {"data": "Airline" , sWidth : '15%'},
                    {"data": "FlightNumber", sWidth : '5%'},
                    {"data": "Destination", sWidth : '15%'},
                    {"data": "DistributionDate", "sType": "eu_date" , sWidth : '5%'},
                    {"data": "Languages", "sClass" : "tblSpan"},
                    {"data": "FormId"}
                ],
             aoColumnDefs: [
				{
					"aTargets": [4],
					"mRender": function(data, type, full) {
						if ((data === null) || (data === undefined) || (data.length == 0)){
							return '';
						}else{
							return moment(data).format("MM/DD/YYYY");
						}
					}
				},
                {
					"aTargets": [5],
					"mRender": function(data, type, full) {
						if ((data === null) || (data === 'undefined') || (data.length == 0)){
							return '';
						}else{
							var languageString = '';
							for(var i = 0; i < data.length; i++){
								languageString += '<span>'+ getLanguageName(data[i].LanguageId) +'</span><span>'+ data[i].FirstSerialNo +'</span><span>'+ data[i].LastSerialNo +'</span>';
								if((i+1) < data.length)
									languageString += '<br>';
							}
							return languageString;
						}
					}
				},
				{
					 "aTargets": [6],
					 "mRender": function(data, type, full) {  
						$('body').data(data.toString(),full);					 
						return '<a href="#editAirlinePopUp" data-toggle="modal" data-backdrop="static"  onclick="callEditAirlines(this,'+ data + ')" class="sepDelEdit" title="Edit"><i class="fa fa-pencil-square-o fa-lg btnColor"></i></a>' +
								'<a href="#" onclick="callDeleteAirline(this, '+data+')" title="Delete"  class="sepDelEdit" ><i class="fa fa-trash fa-lg btnColor"></i></a>' ;

					}
				}
        ]
     });    
}
var cnter= 0, slRI=0;
var callViewDepForm = function (callback, uri){
    
    var postData = {
            "Version":"V1",
            "SessionId":sessionStorage.sessionId,
            "DepartureFormFilterDetails":{
               "StartIndex":"0",
               "OffSet":"100"
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
			result.iTotalRecords= result.GetDepartureFormDetailsResult.TotalRecords;
			result.iTotalDisplayRecords= result.GetDepartureFormDetailsResult.TotalRecords;  
			callback({data: result.GetDepartureFormDetailsResult.DepartureFormDetails});
		})
		.fail(function(result) {
			console.log(result);                
		});
}

function callEditAirlines(ele, data){   
    var sEle;
    sEle = $('body').data(data.toString());	
	var tr = $(ele).closest('tr');
	var row = depTable.row( tr );
	slRI = row.index();
    if(null !== sEle && 'undefined' !== sEle){
		$('body').data('EditedForm' , sEle);
        var template = jQuery.validator.format($.trim((newformDeparture.newAirline('show'))));
        $('#editAirlineBody').empty().append($(template(1)));
        //  Init the validations
        newformDeparture.handleFocusIn();
        newformDeparture.checkValueisNumber('');        
        //  Populate the user preference
        var sOpt = $('#airlinesDD1')[0].options;
        for (var i = 0; i < sOpt.length; i++) {
            if(sOpt[i].value ===  sEle.AirlineId.toString()){
                sOpt[i].selected = true;
                callOnAirlinesChange(1);
                break;
            }
        }
        var sDOpt = $('#destination1')[0].options;
        for (var i = 0; i < sDOpt.length; i++) {
            if(sDOpt[i].value ===  sEle.DestinationId.toString()){
                sDOpt[i].selected = true;
                break;
            }
        }
        var cnt = sEle.Languages.length -1;
        while(cnt > 0){
            $('button.btn.sepH_b.btn-pink.addLanguage').click();
            --cnt;
        }
        for (var i = 0; i < sEle.Languages.length; i++) {
            $('#fserial1_'+(i+1).toString()).val(sEle.Languages[i].FirstSerialNo);
            $('#lserial1_'+(i+1).toString()).val(sEle.Languages[i].LastSerialNo);
            var sLOpt = $('#language1_'+(i+1).toString())[0].options;
            for (var x = 0; x < sLOpt.length; x++) {
                if(sLOpt[x].value ===  sEle.Languages[i].LanguageId.toString()){
                    sLOpt[x].selected = true;
                    break;
                }
            }
        }        
        $('#flight1').val(sEle.FlightNumber);
        $('#businesscard1').val(sEle.BusinessCards);
        //$('#editAirlinePopUp').show();  
		newformDeparture.handleFocusIn();
		newformDeparture.checkValueisNumber('');
    }
}

function callDeleteAirline(ele, id){
    smoke.confirm('Are you sure you want to delete?',function(e){
        if (e){
            var tr = $(ele).closest('tr');
			var row = depTable.row( tr );
			var rowIndex = row.index();
			var uri = getJsonInfoAction(VIEWFORMSDELETE);
			var postData = {
				"Version":"V1",
				"SessionId":sessionStorage.sessionId,
				"FormId":id
			};
			ajaxPost(uri, postData, '', deleteAirline, rowIndex, '','DELETE');
		}
    });
}

function deleteAirline(data, success, rowIndex){    
    if(success)
        smoke.alert('Airline deleted successfully!');
    depTable.row(rowIndex).remove().draw( false )
}

function callChild(rowIndex) {
    count = 0;
    var jsonBase = departureForms.GetDepartureFormDetailsResult.DepartureFormDetails;
    var airlinesCounter = 0;
    var editChild =  '<div id="airlinesChildContainer'+rowIndex+'" class="airlinesChildContainer row-fluid">' +                        
                        '<div class="row-fluid">' +
                        '<div class="span6">' +                        
                            '<div class="span6">' +
                                '<label> Airline </label>' +
                                '<select id="editAirlinesDD"  class="span10 airlinesdrop" onchange="callAirlinesChange(' + airlinesCounter + ')" >' +
                                    '<option selected="true" value='+ jsonBase[rowIndex].AirlineId+'>'+ jsonBase[rowIndex].Airline  +'</option>' +                                    
                                '</select>' +
                            '</div>' +
                            '<div class="span2" style="margin-left:23px;" >' +
                                '<label>Code</label>' +
                                '<input type="text" class="span8" id="code' + airlinesCounter + '"/>' +
                            '</div>' +
                            '<div class="span3">' +
                                '<label>Flight No. </label>' +
                                '<input type="text" class="span10" id="flight' + airlinesCounter + '"/>' +
                            '</div>' +                        
                        '<div class="span12">' +
                            '<div class="span6" style="margin-left:-15px;">' +
                                '<label>Destination</label>' +
                                '<select class="span10" class="destdrop" id="editAirlinesDestination">' +
                                    '<option value="1">New York</option>' +
                                    '<option value="2">Los Angeles</option>' +
                                    '<option value="3">California</option>' +
                                    '<option value="4">Washington</option>' +
                                '</select>' +
                            '</div>' +
                        '<div class="span3" style="margin-left:23px;" >' +
                        '<label>Business Cards</label>' +
                        '<input type="text" class="span5" id="businesscard' + airlinesCounter + '"/>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        
                        '<div id=languageDiv' + airlinesCounter + ' class="span6">' +
                        '<button class="btn sepH_b btn-pink addLanguage" onclick="callAddLanguage()" > Add Language </button>' +
                        '<div id="languageAirline' + airlinesCounter + '">' +
                        '<div class="row-fluid" style="margin-top:-2px;" >' +
                        '<div class="span6">' +
                            '<label>Language </label>' +
                            '<select class="span10" id="editAirlineslanguage" >' +
                                '<option  selected="true" value='+ jsonBase[rowIndex].Languages[0].LanguageId +'>'+ getLanguageName(jsonBase[rowIndex].Languages[0].LanguageId) +'</option>' +
                                '<option value="1">English</option>' +
                                '<option value="2">Spanish</option>' +
                                '<option value="3">French</option>' +
                                '<option value="4">Chinese</option>' +
                            '</select>' +
                        '</div>' +
                        '<div class="span3">' +
                        '<label>First Serial No.</label>' +
                        '<input type="text" class="span10" id="fserial'+ airlinesCounter +'1" value='+ jsonBase[rowIndex].Languages[0].FirstSerialNo +' />' +
                        '</div>' +
                        '<div class="span3">' +
                        '<label>Last Serial No. </label>' +
                        '<input type="text" class="span10" id="lserial' + airlinesCounter + '1" value='+ jsonBase[rowIndex].Languages[0].LastSerialNo +' />' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>' +
                        '</div>';
                
                
       $('#editAirlineBody').empty().append(editChild);     
       
       
}

var count = 0;
function callAddLanguage(){
    
    
    if (count < 5) {
        var languageString = '<div class="row-fluid" style="margin-top:0px;" >' +
                '<div class="span6">' +
                '<select class="span10" id="language">' +
                '<option value="1">English</option>' +
                '<option value="2">Spanish</option>' +
                '<option value="3">French</option>' +
                '<option value="4">Chinese</option>' +
                '</select>' +
                '</div>' +
                '<div class="span3">' +
                '<input type="text" class="span10" id="fserial"/>' +
                '</div>' +
                '<div class="span3">' +
                '<input type="text" class="span10" id="lserial"/>' +
                '</div>' +
                '</div>';

        $('#languageDiv0').append(languageString);
        count++;
    } else {
        alert('Sorry, you cannot add more that 5 languages');
    }
    
    
}

function initialiseAirportName(){
    var airportURL = "./cache/AirportList/AirportList_" + sessionStorage.airportLoginId + ".json";
    if(sessionStorage.roleId !== '0'){
        airportURL = "./cache/AirportList/AirportList_Admin.json";
    }
    $.getJSON(airportURL, function(response, status) {
        if (status === "success") {
            var optionString = '';
            for(var i = 0; i < response.length; i++){
                optionString += '<option value='+ response[i].AirportId +'>'+ response[i].AirportName +'</option>';
            }
            $('#viewDepartureName').append(optionString);
            
            if( $('#viewDepartureName')[0].length === 2){
                $('#viewDepartureName')[0].disabled = true;
                $('#viewDepartureName')[0].selectedIndex = 1;
            }
            
        }
    });
}

function callPopUpClose(data){
    if(null !== data && undefined !== data){
		var d= depTable.row( slRI ).data();
		d.FlightNumber = data.Airlines[0].FlightNumber;
		d.Languages = data.Airlines[0].Languages;
		d.BCardsDistributed = data.Airlines[0].BCardsDistributed;
		depTable.row( slRI ).data(d);
		depTable.row( slRI ).invalidate().draw();
	}
    $('#editAirlinePopUp').modal('hide');
    
}

function updateFormEntries(ele){
	var sEle = $('body').data('EditedForm');
	newformDeparture.submitEditDeparture(ele, sEle, callPopUpClose);
}