/*<Copyright> Cross-Tab </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> viewdata.js </FileName>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var viewData = {
    viewDataMessage : {
        validSearch : 'Please enter a valid value for search'
    },
    viewConfirmitDataTable: undefined,
    init: function() {
        var me = this;
        me.populateConfirmitDetails();
        me.generateReport();
    },
    populateConfirmitDetails: function() {
        var me = this, url = getJsonInfoAction(CONFIRMITCOUNT);
        $.ajax({
            type: 'GET',
            url: url,
            contentType: 'application/json',
            dataType: 'json',
            success: function(res, stat, xhr) {
                if (res && xhr.readyState === 4) {
                    $("#completes").text(res.Completes);
                    $("#businesscompletes").text(res.BusinessCompletes);
                }
            },
            error: function(xhr, stat, err) {

            }
        });
    },
   generateReport: function() {
        var me = this;
        var viewDataURL = getJsonInfoAction(RETRIEVECONFORMITDATA);
        me.viewConfirmitDataTable = $('#viewConfirmitDataTable').DataTable({
            "bProcessing": true,
            "bServerSide": true,
            "aLengthMenu": [20, 50, 100],
            "iDisplayLength": 100,
            "order": [[0, "asc"]],
            "pagingType": "simple",
            "initComplete": function() {
                $("#viewConfirmitDataTable_filter input").off().on('keyup', function(e) {
                    if (e.keyCode == 13){
                        me.viewConfirmitDataTable.search(this.value).draw();
                        this.value = this.value.trim();
                        if(this.value.trim() === ''){
                            pageHelper.clearStickies();
                            pageHelper.notify(viewData.viewDataMessage.validSearch, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);                    
                        }
                    }
                });
                gebo_tips.init();
            },
            //[#73528] General - Sorting option is not working for the ID column of View data.
            "sAjaxSource": viewDataURL,
            "fnServerData": function (sSource, aoData, fnCallback) {
                if(aoData.length === 0) return;
                var cnter = aoData[0].value,
                    searchValue = aoData[35].value === '' ? '-1' : aoData[35].value,
                    sortValue = aoData[37].value === '' ? '-1' : aoData[37].value,
                    IsASC = aoData[38].value === "asc" ? true : false;
                switch (sortValue) {
                    case 1:
                        sortValue = "CardNumber";
                        break;
                    case 2:
                        sortValue = "Class";
                        break;
                    case 3:
                        sortValue = "Status";
                        break;
                    case 4:
                        sortValue = "UploadDate";
                        break;
                    case 5:
                        sortValue = "AirportName";
                        break;
                    default:
                        sortValue = '-1';
                        break;
                }                
                var uri = viewDataURL + '/' + aoData[3].value + '/' + aoData[4].value + '/' + searchValue + '/' + sortValue + '/' + IsASC;
                $.ajax({
                        "type": "GET",
                        "contentType": "application/json",
                        "url": uri,
                        "success": function (msg) {
                            fnCallback(
                                    {
                                        "sEcho": cnter,
                                        "iTotalRecords": msg.RecordsCnt,
                                        "iTotalDisplayRecords": msg.RecordsCnt,
                                        "aaData": msg.ConfirmitDetails === null ?
                                                [] : msg.ConfirmitDetails
                                    });
                        }
                    })
                    .fail(function (result) {
                        console.log(result);
                    });
            },
            aoColumnDefs: [
                {
                    "aTargets": [0],
                    'bSortable': false,
                }
            ],
            "columns": [
                {"data": "RowCnt"},
                {"data": "CardNumber"}, //CODE
                {"data": "Class"},
                {"data": "Status"},
                {"data": "UploadeDate", "visible": false}, //Date/Time
                {"data": "AirportName"}
            ]
        });
    }
};