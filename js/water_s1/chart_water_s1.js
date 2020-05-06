var stock_chart = null;
var area_color = ["#e8e8e8", "#cfcfcf", "#fff9be"];
var line_color = ["#0c3c53", "#8e0f0c"];
var data = [];

var first_load = false;
function change_chart_data(level, code) {
    code = (code) ? code : current_area_code;
    level = (level) ? level : current_area;
    if (stock_chart) {
        var find = false;
        for (var k = 0; k < data.length; k++) {
            var chart_data = data[k];
            for (var i = 0; i < chart_data.length; i++) {
                if ((chart_data[i].area == code) && (chart_data[i].level == level)) {

                    stock_chart.series[k].setData(chart_data[i].time_value, true, 5000, true);

                    //排除回到全台資料沒有更新的問題, 全台建二筆資料
                    if (i == 1) {
                        stock_chart.series[k].setData(chart_data[0].time_value, true, 5000, true);
                    }

                    find = true;
                    break;
                }
            }
            if (find == false) {
                stock_chart.series[k].setData([], true, 5000, true);
            }
        }

        if (first_load == false) {
            first_load = true;
            var rang = stock_chart.xAxis[0].getExtremes();
            stock_chart.xAxis[0].setExtremes(rang.min, rang.max - 1, true);
        }

    }
}

function get_area_title(level, code) {
    level = (level) ? level : current_area;
    code = (code) ? code : current_area_code;

    var area_text = "";
    for (var i = 0; i < area_code.length; i++) {
        if ((area_code[i].level == level) && (area_code[i].code == code)) {
            area_text = area_code[i].text;
            break;
        }
    }
    return area_text;

}

function change_chart_title(level, code) {
    
    var area_text = get_area_title(level, code) + " " + current_site_name + " " + current_map_value_name;
  
    stock_chart.setTitle({
        text: area_text
    }
    , null, true);
    return area_text;

}

function EnabledChartPlot(enable) {
    if (stock_chart) {
        //stock_chart.plotOptions[0].visible = open;
    }
}

function ShowPlotLine(yy, mm) {
    if (stock_chart) {
        if (yy > 0) {
            stock_chart.xAxis[0].plotLinesAndBands[0].options.value = Date.UTC(yy, mm - 1, 1);
            //stock_chart.options.navigator.xAxis.plotLines[0].value = Date.UTC(yy, mm - 1, 1);
        }
        else {
            stock_chart.xAxis[0].plotLinesAndBands[0].options.value = null;
            //stock_chart.options.navigator.xAxis.plotLines[0].value = null;
        }
        stock_chart.xAxis[0].update();
    }
}

function DrawChart(area, container, opts) {

    Highcharts.setOptions({
        lang: {
            months: ["01月", "02月", "03月", "04月", "05月", "06月", "07月", "08月", "09月", "10月", "11月", "12月"]
        }
    });

    var chart_option = {
        chart: {
            spacing: [0, -40, 0, -40],
            events: {
                load: function () {

                },
                click: function (event) {
                    var d = new Date(event.xAxis[0].value);

                    play_current = d;
                    current_sy = play_current.getFullYear();
                    current_sm = play_current.getMonth() + 1;
                    if (current_sm > 6) {
                        current_sy += 1;
                    }
                    current_sm = 1;

                    //draw plot line
                    ShowPlotLine(current_sy, current_sm);

                    //change map data
                    change_year_data(current_sy, current_sm);
                }
            }

        },
        exporting: {
            enabled: false
        },
        credits: {
            enabled: false
        },
        legend: {
            enabled: true,
            itemDistance: 20,
            verticalAlign: "top",
            y: 20
        },
        rangeSelector: {
            selected: 0,
            inputEnabled: false,
            allButtonsEnabled: true,
            buttons: [
            {
                type: 'all'
            }],
            labelStyle: {
                display: "none"
            },
            buttonTheme: {
                style: {
                    display: "none"
                }
            }
        },
        scrollbar: {

        },
        navigator: {
            enabled: true,
            xAxis: {
                labels: {
                    formatter: function () {
                        var d = this.value;
                        var d1 = new Date(d);
                        var v = d1.getYear() + 1900 - 1911;
                        return v;
                    }
                },
                plotLines: [{
                    value: null,
                    color: '#4095d2',
                    dashStyle: 'solid',
                    width: 2,
                    zIndex: 100,
                    label: {
                    }
                }]
            }
        },
        title: {
            useHTML: true,
            text: "圖表"
        },
        tooltip: {
            useHTML: true,
            formatter: function () {
                var d = new Date(this.x);
                var str_d = d.getFullYear() - 1911 + "年";

                var s = "<b>" + str_d + "</b><br />";
                $.each(this.points, function () {
                    if ((this.series.options.tooltip) && (this.series.options.tooltip.pointFormatter)) {
                        s += this.series.options.tooltip.pointFormatter(this);
                    }
                    else {
                        s += '<span style="color:' + this.color + '">\u25CF</span> ' + this.series.name + ': <b>'
                       + this.y + "</b><br/>";
                    }

                });

                return s;
            }
        },
        xAxis: {
            labels: {
                formatter: function () {
                    var d = this.value;
                    var d1 = new Date(d);
                    var v = d1.getYear() + 1900 - 1911 + " 年";
                    return v;
                }
            },
            tickInterval: 365 * 24 * 3600 * 1000,
            showFirstLabel: true,
            tickPixelInterval: 10,
            crosshair: {
                color: "#888",
                zIndex: 999,
            },
            events: {
                setExtremes: function (e) {
                    pause_year();
                }
            },
            plotLines: [{
                value: null,
                color: '#4095d2',
                dashStyle: 'solid',
                width: 2,
                zIndex: 100,
                label: {
                }
            }]
        }
    };

    chart_option = $.extend(true, chart_option, opts);

    // Create the chart
    stock_chart = $('#' + container).highcharts('StockChart', chart_option).highcharts();

}


function open_change_list() {
    var show = $(".chart_change .list").css("display") != "none";
    $(".chart_change .list").css("display", (show == true) ? "none" : "block");
}

function open_map_change_list() {
    var show = $(".map_static_change .list").css("display") != "none";
    $(".map_static_change .list").css("display", (show == true) ? "none" : "block");
}