function initialiseMenu(){

var menuItem = '';
    
    switch(sessionStorage.roleId){
        case '0' :
            menuItem = airportNavBar;            
            break;
        case '1' :
            menuItem = adminNavBar;            
            break;
        case '2' :
            menuItem = adminNavBar;            
            break;    
        default:
            break;
    }
    
    $('#leftMenuPanel').append(menuItem);
    
    
    if((sessionStorage.arivalFormAccess !== 'true') && (sessionStorage.depFormAccess !== 'true')){
        $('#newFormsMenu').hide();
        $('#viewFormsMenu').hide();
    }else{    
        if(sessionStorage.arivalFormAccess !== 'true'){
            $('#newArrivalForm').hide();
            $('#viewArrivalForm').hide();
        }else{
            $('#newArrivalForm').show();
            $('#viewArrivalForm').show();
        }

        if(sessionStorage.depFormAccess !== 'true'){
            $('#viewDepartureForm').hide();
            $('#newDepartureForm').hide();
        }else{
            $('#viewDepartureForm').show();
            $('#newDepartureForm').show();
        }
    }
}



$(document).ready(function() {
    initialiseMenu();
});
