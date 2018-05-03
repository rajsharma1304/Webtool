/* [ ---- Gebo Admin Panel - dashboard ---- ] */
//24 items in array
//var xAxisLabels = ['American Airlines', 'Jet Airways', 'Delta Airlines', 'United Airlines','US Airways', 'SouthWest Airways', 'Alasks Airways','Singapore Airlines', 'Tiger Airways', 'Joe', 'B&H Airlines', 'Petra Airlines', 'Berytos Airlines', 'Air Tanzania', 'Cyprus Airways', 'Smart Wings', 'Felix Airways', 'Mihin Lanka', 'Tajik Air', 'Eagle Air', 'Air Madagascar', 'Air Costa', 'Jagson Airlines', 'IndiGo'];
//var xAxisLabels = [[0, 'American Airlines'], [1, 'Jet Airways'], [2, 'Delta Airlines'], [3, 'United Airlines'], [4, 'US Airways'], [5, 'SouthWest Airways'], [6, 'Alasks Airways'], [7, 'Singapore Airlines'], [8, 'Tiger Airways'], [9, 'Joe'], [10, 'B&H Airlines'], [11, 'Petra Airlines'], [12, 'Berytos Airlines'], [13, 'Air Tanzania'], [14, 'Cyprus Airways'], [15, 'Smart Wings'], [16, 'Felix Airways'], [17, 'Mihin Lanka'], [18, 'Tajik Air'], [19, 'Eagle Air'], [20, 'Air Madagascar'], [21, 'Air Costa'], [22, 'Jagson Airlines'], [23, 'IndiGo']];
//var xAxisLabels = [[0, 'AA'], [1, '9W'], [2, '0J'], [3, 'UA'], [4, 'US'], [5, '2B'], [6, '2G'], [7, 'SQ'], [8, '2L'], [9, '3A'], [10, 'JA'], [11, '4D'], [12, 'BY'], [13, 'TC'], [14, 'CY'], [15, '6A'], [16, '6W'], [17, 'MJ'], [18, '8A'], [19, 'EY'], [20, 'MD'], [21, 'CA'], [22, '9S'], [23, '6E']];
var dataIndex = 0;
function xAxisLabelGenerator(x) {
    dataIndex++;
    dataIndex = dataIndex > d1.length + 0 ? 1 : dataIndex;
    return xAxisLabels[dataIndex - 1];
}
/*var dummy = [ 441,433,324,334,435,346,324,741,414,334,711,345,434,634,334,554,634,134,234,334,434,534,434,334];

 var d1 = [[0, 134],[1, 334],[2, 234],[3, 343],[4, 534],[5, 634],[6, 334],[7, 341],[8, 314],[9, 134],[10, 111],[11, 343],[12, 232],[13, 131],[14, 232],[15, 454],[16, 434],[17, 535],[18, 435],[19, 335],[20, 235],[21, 135],[22, 435],[23, 335]];

 var d2 = [[0, 334],[1, 734],[2, 434],[3, 843],[4, 134],[5, 434],[6, 134],[7, 741],[8, 214],[9, 634],[10, 333],[11, 245],[12, 134],[13, 634],[14, 534],[15, 154],[16, 734],[17, 134],[18, 834],[19, 134],[20, 434],[21, 134],[22, 184],[23, 131]];*/
var d1 = [], d2 = [], d3 = [], d4 = [], xAxisLabels = [], xAxisLabelsMissingTarget = [];

$(document).ready(function () {
    //* small charts
    gebo_peity.init();
    //* charts
    //gebo_charts.fl_1();
    gebo_charts.getQuarter();
    gebo_charts.getDataForTargetComplete();
    gebo_charts.getDataForMissingTargetComplete();
    //gebo_charts.fl_e();
    //gebo_charts.fl_2();
    //* sortable/searchable list
    gebo_flist.init();
    //* calendar
    gebo_calendar.init();
    //* responsive table
    gebo_media_table.init();
    //* resize elements on window resize
    var lastWindowHeight = $(window).height();
    var lastWindowWidth = $(window).width();
    $(window).on("debouncedresize", function () {
        if ($(window).height() != lastWindowHeight || $(window).width() != lastWindowWidth) {
            lastWindowHeight = $(window).height();
            lastWindowWidth = $(window).width();
            //* rebuild calendar
            $('#calendar').fullCalendar('render');
        }
    });
    //* small gallery grid
    gebo_gal_grid.small();

    //* to top
    $().UItoTop({ inDelay: 200, outDelay: 200, scrollSpeed: 500 });
});

//* small charts
gebo_peity = {
    init: function () {
        $.fn.peity.defaults.line = {
            strokeWidth: 1,
            delimeter: ",",
            height: 32,
            max: null,
            min: 0,
            width: 50
        };
        $.fn.peity.defaults.bar = {
            delimeter: ",",
            height: 32,
            max: null,
            min: 0,
            width: 50
        };
        $(".p_bar_up").peity("bar", {
            colour: "#6cc334"
        });
        $(".p_bar_down").peity("bar", {
            colour: "#e11b28"
        });
        $(".p_line_up").peity("line", {
            colour: "#b4dbeb",
            strokeColour: "#3ca0ca"
        });
        $(".p_line_down").peity("line", {
            colour: "#f7bfc3",
            strokeColour: "#e11b28"
        });
    }
};

//* charts
gebo_charts = {
    notify: function (msg) {
        if (msg)
            $.sticky(msg, { autoclose: 2000, position: "top-right", type: "st-error" });
    },
    getQuarter: function () {
        var d = new Date(), month = d.getMonth(), year = d.getFullYear(), nextYear = d.getFullYear()+1, stitle = "", host = getInstance();;
        if(host === 'EUR'){
        switch (true) {
            case (month >=3 && month <=8):
                    stitle = "(" + "Q2" + " - " + year + " + Q3" + " - " + year + ")";
                break;
            case (month >= 0 && month <= 2 || month >= 9 && month <= 11):
                stitle = "(" + "Q4" + " - " + year + " + Q1" + " - " + nextYear + ")";
                break;
            default:
                stitle = "Q1";
                break;
        }
    }else{
        switch (true) {
            case (month >= 0 && month <= 2):
                stitle = "( Q1 - " + year + ")";  
                break;
            case (month >=3  && month <= 5):
                stitle = "( Q2 - " + year + ")";
                if (month = 4) {
                    if (d.getDate() < 11) {
                        stitle = "( Q1 - " + year + ")";
                    }
                }
                break;
            case (month >=6 && month <= 8):
                stitle = "( Q3 - " + year + ")";              
                if (month=7)
                {
                     if (d.getDate() < 11) {
                    stitle = "( Q2 - " + year + ")";
                 }   
                }               
                break;
            case (month >=9 && month <=11):
                stitle = "( Q4 - " + year + ")";
                if (month=10) {
                    if (d.getDate() < 11) {
                        stitle = "( Q3 - " + year + ")";
                    }
                }
                break;
            default:
                stitle = "Q1";
                break;
        }
    }        
		$('#tcHeader').text(stitle);
                $('#tbcHeader').text(stitle);
                
    },
    getDataForTargetComplete: function () {
        var me_ = this, URL_TargetComplete = "", airportID = "";
        if (sessionStorage && sessionStorage.roleId === "1")
            airportID = "Admin";
        else
            airportID = sessionStorage.airportLoginId;
        URL_TargetComplete = "Cache/Charts/TargetsVsCompletes_" + airportID + ".json";

        $.ajax({
            type: "GET",
            cache: false,
            contentType: "application/json",
            dataType: "json",
            url: URL_TargetComplete,
            error: function (e) {
                me_.notify("Error");
            },
            success: function (data) {
                if (data && data.length > 0) {
                    var airlines = [], completes = [], targets = [], obj, airlineName, p = 0;
                    for (var i = 0; i < data.length; i++) {
                        obj = data[i];
                        //[#72857] charts to truncate Target/MissingTarget data if is 0.
                        if (obj !== undefined && obj.Target !== undefined && obj.Target === 0)
                            continue;
                        //[#72760] [Customer]:(PVG login )Target vs Completes chart- The KL airline is displayed as �Ro� in the chart
                        if (obj.Code === null || undefined === obj.Code) {
                            try {
                                var aName = obj.AirlineName;
                                re = /\((.+?)\)/g;
                                found = [];
                                aName.replace(re, function ($0, $1) {
                                    found.push($1)
                                });
                                airlineName = found[found.length - 1];
                            }
                            catch (e) {
                                console.log(e);
                                airlineName = obj.AirlineName.substr(obj.AirlineName.indexOf("(") + 1, 2);
                            }
                        }
                        else
                            airlineName = obj.Code;
                        airlines.push([p, airlineName]);
                        completes.push([p, obj.Completes]);
                        targets.push([p, obj.Target]);
                        p++;
                    }
                    xAxisLabels = [];
                    xAxisLabels = airlines;
                    d1 = completes;
                    d2 = targets;
                    gebo_charts.fl_d();
                }
            }
        });
    },
    getDataForMissingTargetComplete: function () {
        var me_ = this, URL_mTargetComplete = "", airportID = "";
        if (sessionStorage && sessionStorage.roleId === "1")
            airportID = "Admin";
        else
            airportID = sessionStorage.airportLoginId;
        URL_mTargetComplete = "Cache/Charts/MissingTargetsVsBusinessClass_" + airportID + ".json";

        $.ajax({
            type: "GET",
            cache: false,
            contentType: "application/json",
            dataType: "json",
            url: URL_mTargetComplete,
            error: function (e) {
                me_.notify("Error");
            },
            success: function (data) {
                if (data && data.length > 0) {
                    var airlines = [], mcompletes = [], mtargets = [], obj, airlineName, p = 0;
                    for (var i = 0; i < data.length; i++) {
                        obj = data[i];
                        //[#72857] charts to truncate Target/MissingTarget data if is 0.
                        if (obj !== undefined && obj.MissingTarget !== undefined && obj.MissingTarget === 0)
                            continue;
                        //[#72760] [Customer]:(PVG login )Target vs Completes chart- The KL airline is displayed as �Ro� in the chart
                        if (obj.Code === null || undefined === obj.Code) {
                            try {
                                var aName = obj.AirlineName;
                                re = /\((.+?)\)/g;
                                found = [];
                                aName.replace(re, function ($0, $1) {
                                    found.push($1)
                                });
                                airlineName = found[found.length - 1];
                            }
                            catch (e) {
                                console.log(e);
                                airlineName = obj.AirlineName.substr(obj.AirlineName.indexOf("(") + 1, 2);
                            }
                        }
                        else
                            airlineName = obj.Code;
                        airlines.push([p, airlineName]);
                        mcompletes.push([airlineName, obj.MissingCompletes]);
                        mtargets.push([airlineName, obj.MissingTarget]);
                        p++;
                    }

                    xAxisLabelsMissingTarget = [];
                    xAxisLabelsMissingTarget = airlines;
                    d4 = mcompletes.slice(0);
                    d3 = mtargets.slice(0);
                    gebo_charts.fl_e();
                }
            }
        });
    },
    fl_d: function () {
        // Setup the placeholder reference
        var elem = $('#fl_d');

        var dsHook = function (plot, canvasContext, series) {
            if (series.label === "Targets")
                return;
            for (var i = 0; i < series.data.length; i++) {
                var offset = plot.offset();
                var dP = series.data[i];
                var pos = plot.p2c({ x: dP[0], y: dP[1] });
                var barWidth = plot.p2c({ x: dP[0] + series.bars.barWidth, y: dP[1] }).left - pos.left;
                pos.left += offset.left;
                pos.top += offset.top;
                pos.left = pos.left - 10;
                pos.top -= 18;
                var aDiv = $('<div></div>').css({ 'width': barWidth, 'font-size': '10px', 'color': '#000', 'text-align': 'center', 'position': 'absolute', 'left': pos.left, 'top': pos.top }).text(dP[1]).appendTo("body");
                pos.top -= 18;
                var bDiv = $('<div></div>').css({ 'width': barWidth, 'font-size': '10px', 'color': '#000', 'text-align': 'center', 'position': 'absolute', 'left': pos.left, 'top': pos.top }).text(dummy[i]).appendTo("body");
            }
        }

        var dOverlay = function (plot, canvascontext) {
            $(".typeahead.dropdown-menu").first().nextUntil(".typeahead.dropdown-menu").css("display", "none");
        }

        var options = {
            yaxes: [
                { min: 0 },
                { position: "right" }
            ],
            xaxis: {
                //mode: "time",
                //minTickSize: [2, "day"],
                ticks: xAxisLabels,
                //autoscaleMargin: 0.50
                //tickFormatter: xAxisLabelGenerator
            },
            grid: { hoverable: true },
            legend: { position: 'nw' },
            //hooks:{ drawSeries: [ dsHook ], draw:[ dOverlay] },
            colors: ["#EC008C", "#000000"]//["#FFC0CB", "#000000"] //1st color by default #8cc7e0 2nd color by default #3CA0CA
        };
        //	Original colors in blue theme: colors: [ "#5e4223", "#eadac8" ]
        // Setup the flot chart using our data
        fl_d_plot = $.plot(elem,
                [
                    {
                        data: d1,
                        label: "Completes",
                        bars: {
                            align: "center",
                            show: true,
                            barWidth: 0.5,
                            lineWidth: 1,
                            //barWidth: 60 * 360 * 1000,
                            //lineWidth:1,
                            //align: "center",
                            //fill: 1
                        }
                    },
                    {
                        data: d2,
                        label: "Targets",
                        /*curvedLines: {
                         active: true,
                         show: true,
                         lineWidth: 3
                         },*/
                        //yaxis: 2,
                        points: { show: true, symbol: 'circle' },
                        stack: null
                    }
                ], options);

        // Create a tooltip on our chart
        elem.qtip({
            prerender: true,
            content: 'Loading...', // Use a loading message primarily
            position: {
                viewport: $(window), // Keep it visible within the window if possible
                target: 'mouse', // Position it in relation to the mouse
                adjust: { x: 7 } // ...but adjust it a bit so it doesn't overlap it.
            },
            show: false, // We'll show it programatically, so no show event is needed
            style: {
                classes: 'ui-tooltip-shadow ui-tooltip-tipsy',
                tip: false // Remove the default tip.
            }
        });

        // Bind the plot hover
        elem.on('plothover', function (event, coords, item) {
            // Grab the API reference
            var self = $(this),
                    api = $(this).qtip(),
                    previousPoint, content,
                    // Setup a visually pleasing rounding function
                    round = function (x) {
                        return Math.round(x * 1000) / 1000;
                    };

            // If we weren't passed the item object, hide the tooltip and remove cached point data
            if (!item) {
                api.cache.point = false;
                return api.hide(event);
            }

            // Proceed only if the data point has changed
            previousPoint = api.cache.point;
            if (previousPoint !== item.seriesIndex) {
                // Update the cached point data
                api.cache.point = item.seriesIndex;

                // Setup new content
                content = item.series.label + ': ' + round(item.datapoint[1]);

                // Update the tooltip content
                api.set('content.text', content);

                // Make sure we don't get problems with animations
                api.elements.tooltip.stop(1, 1);

                // Show the tooltip, passing the coordinates
                api.show(coords);
            }
        });
    },
    fl_e: function () {
        // Setup the placeholder reference
        var elem = $('#fl_e');

        var options = {
            series: {
                bars: {
                        show: true,
                        barWidth: .4,
                        lineWidth: 1,
                        order: 1, // Include this line to ensure bars will appear side by side.
                        fill:true
                }
                //curvedLines: { active: true }
                //bars:{ show:true }
            },
                
//            },
//            yaxes: [
//                { min: 0 },
//                { position: "right" }
//            ],
            xaxis: {
                mode:"categories"
                //mode: "time"
                //minTickSize: [2, "day"],
                //ticks: xAxisLabelsMissingTarget
                //autoscaleMargin: 0.50
                //					tickFormatter: xAxisLabelGenerator
            },
            grid: { hoverable: true },
            legend: { position: 'nw', backgroundOpacity: 0.1 },
            //hooks:{ drawSeries: [ dsHook ], draw:[ dOverlay] },
            colors: ["#8DC63F", "#006400"]//[ "#90e8a0", "#000000" ]//["#98fb98", "#000000"] //1st color by default #8cc7e0 2nd color by default #3CA0CA
        };
        //	Original colors in blue theme: colors: [ "#5e4223", "#eadac8" ]
        // Setup the flot chart using our data
        fl_e_plot = $.plot(elem,
                [
                    {
                        data: d3,
                        label: "Missing Targets"
                    },
                    {
                        data: d4,
                        label: "Missing Business Class"
                    }
                ], options);

        // Create a tooltip on our chart
        elem.qtip({
            prerender: true,
            content: 'Loading...', // Use a loading message primarily
            position: {
                viewport: $(window), // Keep it visible within the window if possible
                target: 'mouse', // Position it in relation to the mouse
                adjust: { x: 7 } // ...but adjust it a bit so it doesn't overlap it.
            },
            show: false, // We'll show it programatically, so no show event is needed
            style: {
                classes: 'ui-tooltip-shadow ui-tooltip-tipsy',
                tip: false // Remove the default tip.
            }
        });

        // Bind the plot hover
        elem.on('plothover', function (event, coords, item) {
            // Grab the API reference
            var self = $(this),
                    api = $(this).qtip(),
                    previousPoint, content,
                    // Setup a visually pleasing rounding function
                    round = function (x) {
                        return Math.round(x * 1000) / 1000;
                    };

            // If we weren't passed the item object, hide the tooltip and remove cached point data
            if (!item) {
                api.cache.point = false;
                return api.hide(event);
            }

            // Proceed only if the data point has changed
            previousPoint = api.cache.point;
            if (previousPoint !== item.seriesIndex) {
                // Update the cached point data
                api.cache.point = item.seriesIndex;

                // Setup new content
                content = item.series.xaxis.ticks[item.dataIndex].label + ': ' + round(item.datapoint[1]) //item.series.label + ': ' + round(item.datapoint[1]);

                // Update the tooltip content
                api.set('content.text', content);

                // Make sure we don't get problems with animations
                api.elements.tooltip.stop(1, 1);

                // Show the tooltip, passing the coordinates
                api.show(coords);
            }
        });
    },
    fl_1: function () {
        // Setup the placeholder reference
        elem = $('#fl_1');
        var sin = [], cos = [];
        for (var i = 0; i < 14; i += 0.5) {
            sin.push([i, Math.sin(i)]);
            cos.push([i, Math.cos(i)]);
        }
        // Setup the flot chart using our data
        $.plot(elem,
                [
                    { label: "sin(x)", data: sin },
                    { label: "cos(x)", data: cos }
                ],
                {
                    lines: { show: true },
                    points: { show: true },
                    yaxis: { min: -1.2, max: 1.2 },
                    grid: {
                        hoverable: true,
                        borderWidth: 1
                    },
                    colors: ["#8cc7e0", "#2d83a6"]
                }
        );
        // Create a tooltip on our chart
        elem.qtip({
            prerender: true,
            content: 'Loading...', // Use a loading message primarily
            position: {
                viewport: $(window), // Keep it visible within the window if possible
                target: 'mouse', // Position it in relation to the mouse
                adjust: { x: 8, y: -30 } // ...but adjust it a bit so it doesn't overlap it.
            },
            show: false, // We'll show it programatically, so no show event is needed
            style: {
                classes: 'ui-tooltip-shadow ui-tooltip-tipsy',
                tip: false // Remove the default tip.
            }
        });

        // Bind the plot hover
        elem.on('plothover', function (event, coords, item) {
            // Grab the API reference
            var self = $(this),
                    api = $(this).qtip(),
                    previousPoint, content,
                    // Setup a visually pleasing rounding function
                    round = function (x) {
                        return Math.round(x * 1000) / 1000;
                    };

            // If we weren't passed the item object, hide the tooltip and remove cached point data
            if (!item) {
                api.cache.point = false;
                return api.hide(event);
            }

            // Proceed only if the data point has changed
            previousPoint = api.cache.point;
            if (previousPoint !== item.dataIndex) {
                // Update the cached point data
                api.cache.point = item.dataIndex;

                // Setup new content
                content = item.series.label + '(' + round(item.datapoint[0]) + ') = ' + round(item.datapoint[1]);

                // Update the tooltip content
                api.set('content.text', content);

                // Make sure we don't get problems with animations
                //api.elements.tooltip.stop(1, 1);

                // Show the tooltip, passing the coordinates
                api.show(coords);
            }
        });
    },
    fl_2: function () {
        // Setup the placeholder reference
        elem = $('#fl_2');

        var data = [
            {
                label: "United States",
                data: 560
            },
            {
                label: "Brazil",
                data: 360
            },
            {
                label: "France",
                data: 320
            },
            {
                label: "Turkey",
                data: 280
            },
            {
                label: "India",
                data: 160
            }
        ];

        // Setup the flot chart using our data
        $.plot(elem, data,
                {
                    label: "Visitors by Location",
                    series: {
                        pie: {
                            show: true,
                            highlight: {
                                opacity: 0.2
                            }
                        }
                    },
                    grid: {
                        hoverable: true,
                        clickable: true
                    },
                    colors: ["#b3d3e8", "#8cbddd", "#65a6d1", "#3e8fc5", "#3073a0", "#245779", "#183b52"]
                    //colors: [ "#eadac8", "#dcc1a3", "#cea97e", "#c09059", "#a8763f", "#835c31", "#5e4223", "#392815" ]
                }
        );
        // Create a tooltip on our chart
        elem.qtip({
            prerender: true,
            content: 'Loading...', // Use a loading message primarily
            position: {
                viewport: $(window), // Keep it visible within the window if possible
                target: 'mouse', // Position it in relation to the mouse
                adjust: { x: 7 } // ...but adjust it a bit so it doesn't overlap it.
            },
            show: false, // We'll show it programatically, so no show event is needed
            style: {
                classes: 'ui-tooltip-shadow ui-tooltip-tipsy',
                tip: false // Remove the default tip.
            }
        });

        // Bind the plot hover
        elem.on('plothover', function (event, pos, obj) {
            // Grab the API reference
            var self = $(this),
                    api = $(this).qtip(),
                    previousPoint, content,
                    // Setup a visually pleasing rounding function
                    round = function (x) {
                        return Math.round(x * 1000) / 1000;
                    };

            // If we weren't passed the item object, hide the tooltip and remove cached point data
            if (!obj) {
                api.cache.point = false;
                return api.hide(event);
            }

            // Proceed only if the data point has changed
            previousPoint = api.cache.point;
            if (previousPoint !== obj.seriesIndex) {
                percent = parseFloat(obj.series.percent).toFixed(2);
                // Update the cached point data
                api.cache.point = obj.seriesIndex;
                // Setup new content
                content = obj.series.label + ' ( ' + percent + '% )';
                // Update the tooltip content
                api.set('content.text', content);
                // Make sure we don't get problems with animations
                //api.elements.tooltip.stop(1, 1);
                // Show the tooltip, passing the coordinates
                api.show(pos);
            }
        });
    }
};

//* filterable list
gebo_flist = {
    init: function () {
        //*typeahead
        var list_source = [];
        $('.user_list li').each(function () {
            var search_name = $(this).find('.sl_name').text();
            //var search_email = $(this).find('.sl_email').text();
            list_source.push(search_name);
        });
        $('.user-list-search').typeahead({ source: list_source, items: 5 });

        var pagingOptions = {};
        var options = {
            valueNames: ['sl_name', 'sl_status', 'sl_email'],
            page: 10,
            plugins: [
                ['paging', {
                    pagingClass: "bottomPaging",
                    innerWindow: 1,
                    left: 1,
                    right: 1
                }]
            ]
        };
        var userList = new List('user-list', options);

        $('#filter-online').on('click', function () {
            $('ul.filter li').removeClass('active');
            $(this).parent('li').addClass('active');
            userList.filter(function (item) {
                if (item.values().sl_status == "online") {
                    return true;
                } else {
                    return false;
                }
            });
            return false;
        });
        $('#filter-offline').on('click', function () {
            $('ul.filter li').removeClass('active');
            $(this).parent('li').addClass('active');
            userList.filter(function (item) {
                if (item.values().sl_status == "offline") {
                    return true;
                } else {
                    return false;
                }
            });
            return false;
        });
        $('#filter-none').on('click', function () {
            $('ul.filter li').removeClass('active');
            $(this).parent('li').addClass('active');
            userList.filter();
            return false;
        });

        $('#user-list').on('click', '.sort', function () {
            $('.sort').parent('li').removeClass('active');
            if ($(this).parent('li').hasClass('active')) {
                $(this).parent('li').removeClass('active');
            } else {
                $(this).parent('li').addClass('active');
            }
        }
        );
    }
};

//* gallery grid
gebo_gal_grid = {
    small: function () {
        //* small gallery grid
        $('#small_grid ul').imagesLoaded(function () {
            // Prepare layout options.
            var options = {
                autoResize: true, // This will auto-update the layout when the browser window is resized.
                container: $('#small_grid'), // Optional, used for some extra CSS styling
                offset: 6, // Optional, the distance between grid items
                itemWidth: 120, // Optional, the width of a grid item (li)
                flexibleItemWidth: false
            };

            // Get a reference to your grid items.
            var handler = $('#small_grid ul li');

            // Call the layout function.
            handler.wookmark(options);

            //            $('#small_grid ul li > a').attr('rel', 'gallery').colorbox({
            //                maxWidth: '80%',
            //                maxHeight: '80%',
            //                opacity: '0.2',
            //                loop: false,
            //                fixed: true
            //            });
        });
    }
};

//* calendar
gebo_calendar = {
    init: function () {
        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();
        var calendar = $('#calendar').fullCalendar({
            header: {
                left: 'prev,next',
                center: 'title,today',
                right: 'month,agendaWeek,agendaDay'
            },
            buttonText: {
                prev: '<i class="icon-chevron-left cal_prev" />',
                next: '<i class="icon-chevron-right cal_next" />'
            },
            aspectRatio: 1.5,
            selectable: true,
            selectHelper: true,
            select: function (start, end, allDay) {
                var title = prompt('Event Title:');
                if (title) {
                    calendar.fullCalendar('renderEvent',
                            {
                                title: title,
                                start: start,
                                end: end,
                                allDay: allDay
                            },
                    true // make the event "stick"
                            );
                }
                calendar.fullCalendar('unselect');
            },
            editable: true,
            theme: false,
            events: [
                {
                    title: 'All Day Event',
                    start: new Date(y, m, 1),
                    color: '#aedb97',
                    textColor: '#3d641b'
                },
                {
                    title: 'Long Event',
                    start: new Date(y, m, d - 5),
                    end: new Date(y, m, d - 2)
                },
                {
                    id: 999,
                    title: 'Repeating Event',
                    start: new Date(y, m, d + 8, 16, 0),
                    allDay: false
                },
                {
                    id: 999,
                    title: 'Repeating Event',
                    start: new Date(y, m, d + 15, 16, 0),
                    allDay: false
                },
                {
                    title: 'Meeting',
                    start: new Date(y, m, d + 12, 15, 0),
                    allDay: false,
                    color: '#aedb97',
                    textColor: '#3d641b'
                },
                {
                    title: 'Lunch',
                    start: new Date(y, m, d, 12, 0),
                    end: new Date(y, m, d, 14, 0),
                    allDay: false
                },
                {
                    title: 'Birthday Party',
                    start: new Date(y, m, d + 1, 19, 0),
                    end: new Date(y, m, d + 1, 22, 30),
                    allDay: false,
                    color: '#cea97e',
                    textColor: '#5e4223'
                },
                {
                    title: 'Click for Google',
                    start: new Date(y, m, 28),
                    end: new Date(y, m, 29),
                    url: 'http://google.com/'
                }
            ],
            eventColor: '#bcdeee'
        })
    }
};

//* responsive tables
gebo_media_table = {
    init: function () {
        $('.mediaTable').mediaTable();
    }
};