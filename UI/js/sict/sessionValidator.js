/*<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
<ProjectName>SICT</ProjectName>
<FileName> sessionvalidator.js </FileName>
<Author>Vivek.A</Author>
<CreatedOn>15 Jan 2015</CreatedOn>*/
var timer;
var url;
var httpRequest;

self.onmessage = function(e) {
    url = e.data;
    update(url);
};


function update(url)
{
    function infoReceived()
    {
        var output = httpRequest.responseText;
        if (output) {
            postMessage(output);
        }
        httpRequest = null;
    }

    httpRequest = new XMLHttpRequest();
    httpRequest.open("GET", url, true);
    httpRequest.onload = infoReceived;
    httpRequest.send(null);
}

setInterval(function() {
    update(url);
}, 60 * 1000);
