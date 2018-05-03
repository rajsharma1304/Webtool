var searchPage = {
    searchTable: '',
    isInitiated : false,
    searchMessages : {
      ValidationError:'Please re-check the highlighted controls.'
    },
    initSearch: function() {
        searchPage.validateEntries();
        $("#startCodeValue").focusout(function(e) {
            var me = this, elemId = e.target.id, inputValue = me.value.replace(/\s+/g, ""),
                    parent = $(me).parent();
            if (('' === inputValue) || ("0" === inputValue)){
                parent.addClass('sict_error');
                $('#Generate').attr("disabled", true);                
            } else {
                parent.removeClass('sict_error');
                $('#Generate').removeAttr("disabled");
            }
        });
        $('#Generate').on('click', function() {
            $('#formHdr').toggle('200');
            $(this).parents().find('#parentDiv').find('i').removeClass('fa-arrow-circle-up').addClass('fa-arrow-circle-down');
            $('#parentDiv').removeClass('formSep');
        });
    },
    retrieveSearchResults: function() {
        var me = this;
        if(me.isInitiated){
            me.searchTable.draw();
        }else{
            me.isInitiated = true;
        var uri = getJsonInfoAction(RETRIEVESEARCH);
        me.searchTable = $('#searchTable').DataTable({
            "bProcessing": true,
            "bServerSide": true,
            "aLengthMenu": [20, 50, 100],
            "iDisplayLength": 100,
            "pagingType": "simple",
            "order": [[0, "desc"]],
            "sAjaxSource": uri,
            "initComplete": function() {
                $("#searchTable_filter input").off().on('keyup', function(e) {
                    if (e.keyCode == 13)
                        me.searchTable.search(this.value).draw();
                });
                gebo_tips.init();
            },
            "fnServerData": function(sSource, aoData, fnCallback) {
                cnter = aoData[0].value;
                var sValue = aoData[50].value, fValue = aoData[52].value, IsASC = aoData[48].value === "asc" ? true : false;
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
                        fValue = "";
                        break;
                }
                var startVal = $("#startCodeValue").val(), endVal = $("#endCodeValue").val() === '' ? $("#startCodeValue").val() : $("#endCodeValue").val();
                var postData = {
                    "Version": communion.Version,
                    "SessionId": sessionStorage.sessionId,
                    "Instance":getInstance(),
                    "SerialNoFilterDetails": {
                        "StartIndex": aoData[3].value,
                        "OffSet": aoData[4].value,
                        "FilterValue": sValue,
                        "Sort": fValue,
                        "IsSortByAsc": IsASC,
                        "StartSerialNo": startVal,
                        "EndSerialNo": endVal
                    }
                };

                $.ajax({
                    "type": "POST",
                    "contentType": "application/json",
                    "url": sSource,
                    "data": JSON.stringify(postData),
                    "success": function(msg) {
                        fnCallback(
                                {
                                    "sEcho": cnter,
                                    "iTotalRecords": msg.GetFormsbySerialNoResult.TotalRecords,
                                    "iTotalDisplayRecords": msg.GetFormsbySerialNoResult.TotalRecords,
                                    "aaData": msg.GetFormsbySerialNoResult.DepartureFormDetails === null ?
                                            [] : msg.GetFormsbySerialNoResult.DepartureFormDetails
                                });
                    }
                })
                        .fail(function(result) {
                            console.log(result);
                        });
            },
            "columns": [
                {"data": "FormId", sWidth: '5%'},
                {"data": "DistributionDate", sWidth: '7%', "sType": "eu_date"},
                {"data": "Airline", sWidth: '12%'},
                {"data": "Interviewer", sWidth: '10%'},
                {"data": "Airline", sWidth: '5%'},
                {"data": "FlightNumber", sWidth: '5%'},
                {"data": "Type", sWidth: '5%'},
                {"data": "Destination", sWidth: '5%'},
                {"data": "Languages", "sClass": "tblSpan"}
            ],
            aoColumnDefs: [
                {
                    "aTargets": [1],
                    "mRender": function(data, type, full) {
                        if ((data === null) || (data === undefined) || (data.length == 0)) {
                            return '';
                        } else {
                            return moment(data).format("MM/DD/YYYY");
                        }
                    }
                },
                {
                    "aTargets": [6],
                    "mRender": function(data, type, full) {
                        if ((data === "D") ) {
                            return 'Departure';
                        } else {
                            return 'Arrival';
                        }
                    }
                },
                {
                    "bSortable": false,
                    "aTargets": [8],
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
                }
            ]
        });
    }
    },
    validateEntries : function (){
        $("input[type=text]").focusin(function() {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('.sict_error');                          
        });
        $('input[type=text]').keydown(function (e) {
            var keyCodes = [46, 8, 9, 27, 13, 110, 190];            
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, keyCodes) !== -1 ||
                 //[#72946] Search Page label needs correction and also validation
                // Allow:  Ctrl+C, Ctrl+V
                (( e.keyCode == 67 || e.keyCode == 86 || 
                        // Ctrl+c, Ctrl+v
                        e.keyCode == 99 || e.keyCode == 118)  &&
                                     e.ctrlKey === true) ||
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
        //[#73065] Search Page: On Copy Pasting the Start/End Code in respective fields doesn't Validate.
        $("input[type=text]").focusout(function(e) {
            var target = $(e.target);
            var targetString = $(e.target).val();
            if(targetString.length > 0){
                for (var i = 0; i < targetString.length; i++) {
                    var charCode = targetString.charCodeAt(i);
                    //To check if the string has alphabets.
                    if(charCode > 65 && charCode < 122){
                        target.parent().addClass('sict_error'); 
                        $(e.target).val('');
                        return;
                    }else{
                        target.parent().removeClass('sict_error');
                    }
                }
            }else{
                target.parent().addClass('sict_error');
            }
        });
        
    }
};

