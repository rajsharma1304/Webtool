/*<Copyright> Cross-Tab </Copyright>
 <ProjectName>SICT</ProjectName>
 <FileName> manageinterviewers.js </FileName>
 <CreatedOn>15 Jan 2015</CreatedOn>*/
var manageInterviewers = {
    interviewersMessages: {
        AdditionProcessing: 'Please wait while the interviewer is being added',
        UpdateProcessing: 'Please wait while the interviewer is being updated',
        Succes: 'Interviewer successfully added',
        Update:'Interviewer information successfully updated',
        DataExists : 'Sorry, an interviewer with the same name already exists for this airport',
        ValideError:'Please re-check the highlighted controls',
        validSearch : 'Please enter a valid value for search'
    },
    interviewersTables: '',
    isInitialised: false,
    rowID : '',
    postObj : {},
    initManageInterviewers: function() {
        var me = this;
        me.initAirlines();
    },
    initAirlines: function(add) {
        cacheMgr.selectedAirport(sessionStorage.airportLoginId);
        //[#72732] For the login CT, Edit interviewers is not saving in the manage interviewers page
        //[#72673] International:Airport list is displaying MSP airport name twice in a list.
        var airportString = airportUtil.validAirportOptions();
        $(".sict_error").removeClass('sict_error');
        if (add) {            
            $("#addAirportDD").empty().append(airportString);
            $('#addInterviewerName').val("");
            $(".modal-header h3").html("Add Interviewer");
            $('#saveInterviewers').attr('onclick', 'manageInterviewers.addOrEditInterviewers(true)');
            manageInterviewers.handleValidations();
        } else {
            $("#interviewerAirlines").empty().append(airportString);
        }
    },
    getInterviewersReport: function() {
        var me = this;
        me.uri = getJsonInfoAction(RETRIEVEINTERVIEWERS);
        var interviewersURL = me.uri + '/0/5000/' + pageUtils.returnValue($('#interviewerAirlines').val()) + '/' + pageUtils.returnValue($('#interviewerName').text());       
            me.isInitialised = true;
            me.interviewersTables = $('#interviewersTables').DataTable({
                "bProcessing": true,
                "aLengthMenu": [20, 50, 100],
                "iDisplayLength": 100,
                "order": [[1, "asc"]],
                "pagingType": "simple",
                //[#72731] For the login CT, Airport Filter is not working.
                "bDestroy": true,
                "initComplete": function() {
                    $("#interviewersTables_filter input").off().on('keyup', function(e) {
                        if (e.keyCode == 13){
                            me.interviewersTables.search(this.value).draw();
                            this.value = this.value.trim();
                            if(this.value.trim() === ''){
                                pageHelper.clearStickies();
                                pageHelper.notify(manageInterviewers.interviewersMessages.validSearch, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.INFO);                    
                            }
                        }                            
                    });
                    gebo_tips.init();
                },
                "ajax": function(data, callback, settings) {
                    $.get(interviewersURL)
                            .done(function(result) {
                                var nodeName = result.InterviewerDetail;
                                if (nodeName !== null)
                                    callback({data: nodeName});
                                else
                                    callback({data: []});
                            });
                },
                "columns": [
                    {"data": "InterviewerId", sWidth: '8%'},
                    {"data": "InterviewerName", sWidth: '34%'},
                    {"data": "AirportName", sWidth: '34%'},
                    {"data": "IsActive", sWidth: '14%'},
                    {"data": "InterviewerId", sWidth: '10%'}
                ],
                aoColumnDefs: [
                    {
                        "aTargets": [3],
                        "mRender": function(data, type, full) {
                            if (data === true)
                                return 'Active';
                            else
                                return 'Inactive';
                        }
                    },
                    {
                        "aTargets": [4],
                        'bSortable': false,
                        "mRender": function(data, type, full) {
                            $('body').data(data.toString(), full);
                            return '<a data-toggle="modal" data-backdrop="static" href="#addOrEditInterviewer" onclick="manageInterviewers.editInterviewers(this)" class="sepDelEdit ttip_t" title="Edit Interviewer"><i class="fa fa-pencil-square-o fa-lg btnColor"></i></a>'
                        }
                    }
                ]
            });
        
    },
    editInterviewers: function(ele) {
        var me = this;
        me.handleValidations();
        var tr = $(ele).closest('tr');
        var row = me.interviewersTables.row(tr);
        manageInterviewers.rowID = row;
        var rowData = row.data();
        var interviewerID = rowData.InterviewerId;
        var code = rowData.Code;
        $(".sict_error").removeClass('sict_error');
        cacheMgr.selectedAirport(sessionStorage.airportLoginId);
        var airLineString = airportUtil.validAirportOptions();
        $("#addAirportDD").empty().append(airLineString).val(rowData.AirportId).attr("selected", "selected");
        $('#addInterviewerName').val(rowData.InterviewerName);
        $('#addInterviewerStatus').val(rowData.IsActive.toString());
        $(".modal-header h3").html("Edit Interviewers");
        $('#saveInterviewers').attr('onclick', 'manageInterviewers.addOrEditInterviewers(false, ' + interviewerID + ')');

    },
    addOrEditInterviewers: function(add, interviewerID) {
        var me = this, addURL, method, msg;
        if (add) {            
            addURL = getJsonInfoAction(ADDINTERVIEWER);
            method = 'PUT';
            msg = manageInterviewers.interviewersMessages.AdditionProcessing;
            manageInterviewers.postObj = {
                "SessionId": sessionStorage.sessionId,
                "Version": communion.Version,
                "Instance":getInstance(),
                "InterviewerDetail": {
                    "AirportId": '',
                    "InterviewerName": '',
                    "IsActive": ''
                }
            };
        } else {
            addURL = getJsonInfoAction(UPDATEINTERVIEWER);
            method = 'POST';
            msg = manageInterviewers.interviewersMessages.UpdateProcessing;
            manageInterviewers.postObj = {
                "SessionId": sessionStorage.sessionId,
                "Version": communion.Version,
                "Instance":getInstance(),
                "InterviewerDetail": {
                    "AirportId": '',
                    "InterviewerName": '',
                    "InterviewerId": interviewerID,
                    "IsActive": ''
                }
            };
        }
        if (($('#addAirportDD').val() === '-1')) {            
            $('#addAirportDD').parent().addClass('sict_error');
        }
        if (($('#addInterviewerName').val() === "")) {            
            $('#addInterviewerName').parent().addClass('sict_error');
        }
        
        if ($('.sict_error').length === 0) {
            manageInterviewers.postObj.InterviewerDetail.AirportId = parseInt($('#addAirportDD').val());
            var code = $('#addAirportDD option:selected').text();
            if(add)
                manageInterviewers.postObj.InterviewerDetail.InterviewerName = code + ' - ' + $('#addInterviewerName').val();
            else{
                //[#73680] [Customer]:Aircraft Instances – CT login- Manage Interviewer- While editing the interviewer name if the Airport id is cleared, the name of the Interviewer is saved as ‘ undefined’.
                if($('#addInterviewerName').val().indexOf('-') === -1)
                    manageInterviewers.postObj.InterviewerDetail.InterviewerName = code + ' - '+ $('#addInterviewerName').val();
                else
                    manageInterviewers.postObj.InterviewerDetail.InterviewerName = code + ' - '+ $('#addInterviewerName').val().split(' - ')[1];
            }
            //[#73737] HTML is not encoded and decoded properly
            manageInterviewers.postObj.InterviewerDetail.InterviewerName = manageInterviewers.postObj.InterviewerDetail.InterviewerName.replace(/<(?:.|\n)*?>/gm, '');
            
            manageInterviewers.postObj.InterviewerDetail.IsActive = $('#addInterviewerStatus').val();

            pageHelper.clearStickies();
            pageHelper.addSmokeSignal(msg);
            ajaxPost(addURL, manageInterviewers.postObj, '', me.acknowledgeAddOrEdit, add, '', method);
        } else {
            pageHelper.clearStickies();
            pageHelper.notify(manageInterviewers.interviewersMessages.ValideError, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
        }
    },
    acknowledgeAddOrEdit: function(data, success, add) {
        if (success) {
            var message = '';
            pageHelper.removeSmokeSignal();
            if(data.ReturnCode === 1){  
                if(add){
                    message = manageInterviewers.interviewersMessages.Succes;
                }else{
                    message = manageInterviewers.interviewersMessages.Update;
                }
                //[#72954] Europe – Manage Interviewers, multiple issues for CT user.
                manageInterviewers.interviewersTables.ajax.reload(null, false);
                pageHelper.notify(message, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.SUCCESS);
                $('#addOrEditInterviewer').modal('hide');
                //[#73712] Aircraft – CT login – Manage Airports: After clicking the ‘Deactivate Airport’ icon in Manage Airports page, the ‘Active’ checkbox does not get unchecked in Edit Airports pop-up 
                cacheMgr.clearCache(true);//pass true if required session needs to be cleared.
                manageInterviewers.initAirlines();
            }else if(data.ReturnCode === 4){
                pageHelper.notify(manageInterviewers.interviewersMessages.DataExists, pageHelper.stickyPosition.top.RIGHT, pageHelper.stickyTypes.ERROR);
            }
        }
    },
    handleValidations: function() {
        $("input").focusin(function() {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('.sict_error');
        });
        $("select").focusin(function() {
            var parent = $(this).parent();
            if (parent.hasClass("sict_error"))
                parent.removeClass('sict_error');
        });
        $("input[type=text]").focusout(function(e) {
            var me = this, elemId = e.target.id, inputValue = me.value.replace(/\s+/g, ""),
                    parent = $(me).parent();
            if ('' !== inputValue)
                parent.removeClass('sict_error');
            else
                parent.addClass('sict_error');
        });
        $("select").focusout(function(e) {
            var target = $(e.target);
            if ("-1" === target.val())
                target.parent().addClass('sict_error');
            else
                target.parent().removeClass('sict_error');
        });
    },
};