/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

function showORHide(domId, cObj){
    
    var ref = $(cObj);
    $("#" + domId).toggle('200');
    if (ref.attr("class") == "fa fa-arrow-circle-up pointer ttip_t"){
        ref.attr("class", "fa fa-arrow-circle-down pointer ttip_t").attr('title','Expand');
        $('#parentDiv').removeClass('formSep');        
    }else{
        //[#72943] [Customer]:the spelling of ‘ Collapse ‘ in the tooltip is incorrect
        ref.attr("class", "fa fa-arrow-circle-up pointer ttip_t").attr('title','Collapse');        
        $('#parentDiv').addClass('formSep');
    }

}
