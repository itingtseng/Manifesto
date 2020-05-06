var stock_chart = null;
var area_color = ["#e8e8e8", "#cfcfcf", "#fff9be"];
var line_color = ["#0c3c53", "#8e0f0c"];

var first_load = false;
function change_chart_data(level, code) {
    code = (code) ? code : current_area_code;
    level = (level) ? level : current_area;
    if (stock_chart) {
        var find = false;
        for (var i = 0; i < co_chart.length; i++) {
            if ((co_chart[i].area == code) && (co_chart[i].level == level)) {
              
                stock_chart.series[0].setData(co_chart[i].time_value, true, 5000, true);

                //排除回到全台資料沒有更新的問題, 全台建二筆資料
                if (i == 1) {
                    stock_chart.series[0].setData(co_chart[0].time_value, true, 5000, true);
                }

                find = true;
                break;
            }
        }

        if (first_load == false) {
            first_load = true;
            var rang = stock_chart.xAxis[0].getExtremes();
            stock_chart.xAxis[0].setExtremes(rang.min, rang.max - 1, true);
        }

        if (find == false) {
            stock_chart.series[0].setData([], true, 5000, true);
        }
    }
}

function get_driver_count(date) {
    var d = new Date(date);
    var yy = d.getFullYear();
    var mm = d.getMonth() + 1;

    var car = "";
    for (var i = 0; i < Car_count.length; i++) {
        var data = Car_count[i];
        if ((data.level == current_area) && (data.area == current_area_code)) {
            var sd = new Date(data.time_value[0][0]);
            var sy = sd.getFullYear();
            var sm = sd.getMonth() + 1;
            var index = ((yy - sy) * 12) + (mm - sm);
            car = "汽油車數量：" + data.time_value[index][1];
            break;
        }
    }
    var moto = "";
    for (var i = 0; i < Scootor_count.length; i++) {
        var data = Scootor_count[i];
        if ((data.level == current_area) && (data.area == current_area_code)) {
            var sd = new Date(data.time_value[0][0]);
            var sy = sd.getFullYear();
            var sm = sd.getMonth() + 1;
            var index = ((yy - sy) * 12) + (mm - sm);
            moto = "機車數量：" + data.time_value[index][1];
            break;
        }
    }

    return "<br>" + car + "<br>" + moto;
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

                    //draw plot line
                    ShowPlotLine(current_sy, current_sm);

                    //change map data
                    change_year_data(current_sy, current_sm);
                }
            }

        },
        exporting: {
            enabled: true,
            buttons: {
                contextButton: {
                    align: "right",
                    x: -100, y: 10,
                }
            }
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
                var str_d = d.getFullYear() - 1911 + "年 " + (d.getMonth() + 1) + "月"

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
                    var v = d1.getYear() + 1900 - 1911 + "/" + (d1.getMonth() + 1);
                    return v;
                }
            },
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
            }],
            maxPadding: 0.5
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