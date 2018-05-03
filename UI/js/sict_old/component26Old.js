var TOTAL_PRODUCTS = 0;
/*
 * Creating html page. 
 */
function createHtml() {
    var htmlStr = "<div class='main_container comp26'>";
    htmlStr += "<div class='absoluteDiv'></div>";
    htmlStr += '<ul id="sortable1" class="droptrue">';
    htmlStr += '</ul>';
    htmlStr += '<ul id="sortable2" class="droptrue">';
    htmlStr += '</ul>';
    htmlStr += '</div>';
    /*creating products*/
    if (inputFormat == 'confirmit') {
        $('fieldset').children('table').hide();
        TOTAL_PRODUCTS = $('fieldset').children('table').children('tbody').children('tr').length;
        $('fieldset').children('table').after(htmlStr);
    }
    else {
        TOTAL_PRODUCTS = $('form').children('table').first().children().children().length-2;
        $('form').children('table').first().children().children().hide();
        $('form').children('table').first().children().children(':last-child').show();
        $('form').children('table').first().children().children(':first-child').show();
        $('.main').after(htmlStr);
    }
    getProducts();
    $('.main').hide();
    centerAlign('.comp26');
    verticalAlign();
    varticalAlignNumber();
    customReady();
}

function customReady(){
    var hoverStatus = true;
    $( "ul.droptrue" ).sortable({
        containment: "body",
        connectWith: "ul",
        start:function(){
            hoverStatus = false;
            $('.absoluteDiv').show();
        },
        stop:function(){
            addRemoveOrangeBox();
            hoverStatus = true;
            $('.absoluteDiv').hide();
        }
    });
 
    $( ".droptrue" ).disableSelection();
    $('.droptrue li.sortableBlock').mouseover(function(){
        hoverLook(this);
    });
    $('.droptrue li.sortableBlock').mouseleave(function(){
        if(hoverStatus)
            initialLook(this);
    });
}

function getProducts(){
    var str1 = '', str2 = '',cnt = 1,cnt1 = 1,tempStr = '';
    if(inputFormat == 'confirmit'){
        $('fieldset').children('table').children('tbody').children('tr').each(function(){
            if($(this).children(':last-child').children('input').val() == ''){
				tempStr = replaceTextWithImage($(this).children(':first-child').text());
                str1 += '<li class="sortableBlock" sequence="'+cnt+'"><div class="grayBg"></div><table class="textTable" sequence="'+cnt+'"><tr><td sequence="'+cnt+'">'+tempStr+'</td></tr></table><div class="liBgImg"><img src="http://www.cross-tab.us/ctutility/html5/images/comp26/gray-vertical-small.jpg"/></div></li>';
            }
            cnt++;
        });
        for(var i = 1; i <= TOTAL_PRODUCTS ; i++){
            $('fieldset').children('table').children('tbody').children('tr').each(function(){
                if($(this).children(':last-child').children('input').val() != '' && parseInt($(this).children(':last-child').children('input').val()) == i){
					tempStr = replaceTextWithImage($(this).children(':first-child').text());
                    str2 += '<li class="sortableBlock" sequence="'+cnt1+'" style="position: relative; z-index: 999;"><div class="grayBg"></div><table class="textTable" sequence="'+cnt1+'"><tbody><tr><td sequence="'+cnt1+'">'+tempStr+'</td></tr></tbody></table><div class="liBgImg"><img src="	"></div><div class="orangeBox"><table><tbody><tr><td>'+$(this).children(':last-child').children('input').val()+'</td></tr></tbody></table><div class="orangeDiv"><img src="http://www.cross-tab.us/adhoc/p1844310259/blue-vertical.png"></div></div></li>'
                }
                cnt1++;
            });
            cnt1 = 1;
        }
    }else{
        var askCount = 1;
        $('form').children('table').first().children().children().each(function(){
            if(askCount > 1 && askCount <= TOTAL_PRODUCTS+1 ){
                if($(this).children(':last-child').children('input').val() == ''){
				tempStr = replaceTextWithImage($(this).children(':first-child').text());
                    str1 += '<li class="sortableBlock" sequence="'+cnt+'"><div class="grayBg"></div><table class="textTable" sequence="'+cnt+'"><tr><td sequence="'+cnt+'">'+tempStr+'</td></tr></table><div class="liBgImg"><img src="http://www.cross-tab.us/ctutility/html5/images/comp26/gray-vertical-small.jpg"/></div></li>';
                }
                cnt++;
            }
            askCount++;
        });
        askCount = 1;
        for(var i = 1; i <= TOTAL_PRODUCTS ; i++){
            $('form').children('table').first().children().children().each(function(){
                if(askCount > 1 && askCount <= TOTAL_PRODUCTS+1 ){
                    if($(this).children(':last-child').children('input').val() != '' && parseInt($(this).children(':last-child').children('input').val()) == i){
						tempStr = replaceTextWithImage($(this).children(':first-child').text());
                        str2 += '<li class="sortableBlock" sequence="'+cnt1+'" style="position: relative; z-index: 999;"><div class="grayBg"></div><table class="textTable" sequence="'+cnt1+'"><tbody><tr><td sequence="'+cnt1+'">'+tempStr+'</td></tr></tbody></table><div class="liBgImg"><img src="http://www.cross-tab.us/ctutility/html5/images/comp26/gray-vertical-small.jpg"></div><div class="orangeBox"><table><tbody><tr><td>'+$(this).children(':last-child').children('input').val()+'</td></tr></tbody></table><div class="orangeDiv"><img src="http://www.cross-tab.us/adhoc/p1844310259/blue-vertical.png"></div></div></li>'
                    }
                    cnt1++;
                }
                askCount++;
            });
            askCount = 1;
            cnt1 = 1;
        }
    }
    $('#sortable1').append(str1);
    $('#sortable2').append(str2);
}

function replaceTextWithImage(txt)
{
	if(txt.indexOf('#') >= 0)
	{
	var arrStr = txt.split('#');
	if(arrStr[1])
		return '<img src="arrStr[1]" width="25" height="25"/>";
	}
	else if(txt.toLowerCase() == 'other')
		return txt + ' ' +<input type="text"/>;
	return txt;
}          
		  
function initialLook(handler){
    $(handler).children('.liBgImg').children('img').attr('src','http://www.cross-tab.us/ctutility/html5/images/comp26/gray-vertical-small.jpg');
    $(handler).children('.smallOrangeBox').remove();
    $(handler).children('.textTable').children().children().children().css('color','#000000');
}

function hoverLook(handler){
    $(handler).children('.liBgImg').children('img').attr('src','http://www.cross-tab.us/ctutility/html5/images/comp26/gray-dark-small-v.jpg');
    $(handler).append('<div class="smallOrangeBox"><img src="http://www.cross-tab.us/adhoc/p1844310259/blue-vertical.png" class=""/></div>');
    $(handler).children('.textTable').children().children().children().css('color','#ffffff');
}

function addRemoveOrangeBox(){
    var cnt = 1;
    $('#sortable2 li').each(function(){
        if(!$(this).children().hasClass('orangeBox')){
            $(this).append('<div class="orangeBox"><table><tr><td></td></tr></table><div class="orangeDiv"><img src="http://www.cross-tab.us/adhoc/p1844310259/blue-vertical.png"/></div></div>');
        }
        $(this).children('.orangeBox').children('table').children().children().children().text(cnt);
        initialLook(this);
        if(inputFormat == 'confirmit'){
            $('fieldset').children('table').children('tbody').children(':nth-child('+(parseInt($(this).attr('sequence')))+')').children(':last-child').children('input').val(cnt);
        }else{
            $('form').children('table').first().children().children(':nth-child('+(parseInt($(this).attr('sequence'))+1)+')').children(':last-child').children('input').val(cnt);
        }
        cnt++;
    });
    $('#sortable1 li').each(function(){
        $(this).children('.orangeBox').remove();
        initialLook(this);
        if(inputFormat == 'confirmit'){
            $('fieldset').children('table').children('tbody').children(':nth-child('+(parseInt($(this).attr('sequence')))+')').children(':last-child').children('input').val('');
        }else{
            $('form').children('table').first().children().children(':nth-child('+(parseInt($(this).attr('sequence'))+1)+')').children(':last-child').children('input').val('');
        }
    });
    verticalAlign();
    varticalAlignNumber();
}

function centerAlign(className) {
    if ($(className).width() < $(className).parent().width()) {
        $(className).css('margin-left', ($(className).parent().width() - $(className).width()) / 2 );
    }
}

function verticalAlign(){
    $('.sortableBlock').each(function(){
        $(this).css('height',($(this).children('.textTable').height()+5));
        var h = parseInt($(this).children('.textTable').height()+5);
        if(h < 38){
            $(this).children('.liBgImg').children('img').css('height',38);
            $(this).children('.orangeBox').children('.orangeDiv').children('img').css('height',38);
        }else{
            $(this).children('.liBgImg').children('img').css('height',h);
            $(this).children('.orangeBox').children('.orangeDiv').children('img').css('height',h);
        }
    });
}

function varticalAlignNumber(){
    $('.orangeBox').each(function(){
        $(this).children('table').css('height',($(this).height()-4));
    });
}