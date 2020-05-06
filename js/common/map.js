var map_object = null;
var map_level_ary = new Array();
var map_tooltip_value_func = null;
var current_site_name = "";
var map_legend_sub_text = null;
var data_unit = "month";

var map_color = [
    ["#bcdbed", "#9dc6d5", "#5890a8", "#195b7a", "#0c3c53"],
    ["#fde8de", "#f39a87", "#e93b5c", "#bb2f48", "#8e0f0c"]
];
var color_index = 0;
var _map_container = "";
var _default_map_optn = null;
var _current_color_index = 0;
var _map_max_value = null;


function change_area_level(action) {
    var area_level = ["ALL", "Air", "City", "Station"];
    var index = -1;
    for (var i = 0; i < area_level.length; i++) {
        if (current_area == area_level[i]) {
            index = i;
            break;
        }
    }

    var index = index + ((action == "down") ? 1 : -1);
    current_area = area_level[index];

    //更改 chart 的 title

    return current_area;
}

function get_map_color(value) {

    var n = _map_max_value / 5;
    if (value == null) {
        return "#ffffff";
    }

    if (value < _map_max_value[0]) return map_color[_current_color_index][0];
    if (value > _map_max_value[_map_max_value.length - 1]) return map_color[_current_color_index][4];

    for (var i = 0; i < _map_max_value.length - 1; i++) {
        var n1 = _map_max_value[i];
        var n2 = _map_max_value[i + 1];
        if (value >= n1 && value <= n2) {
            return map_color[_current_color_index][i + 1];
        }
    }

    return "#ffffff";
}

function num_thou(n) {
    n += "";
    var arr = n.split(".");
    var re = /(\d{1,3})(?=(\d{3})+$)/g;
    return arr[0].replace(re, "$1,") + (arr.length == 2 ? "." + arr[1] : "");
}

function set_color_class(value, index, is_percent, fixed) {

    _current_color_index = index;
    _map_max_value = value;

    var f = (!isNaN(fixed)) ? fixed : 2;
    var color = new Array();
    for (var i = 0; i <= value.length; i++) {
        var c = {
            color: map_color[index][i]
        };
        if (i == 0) {
            c.to = value[0];
            if (is_percent == true) {
                c.name = "< " + num_thou((value[0] * 100).toFixed(0)) + " %"
            }
            else {
                c.name = "< " + num_thou((value[0]).toFixed(f));
            }
        }
        if (i == value.length) {
            c.from = value[value.length - 1];
            if (is_percent == true) {
                c.name = "> " + num_thou((value[value.length - 1] * 100).toFixed(0)) + " %"
            }
            else {
                c.name = "> " + num_thou((value[value.length - 1]).toFixed(f))
            }
        }
        if (i >= 1 && i < value.length) {
            c = {
                color: map_color[index][i],
                from: value[i - 1],
                to: value[i]
            }
            if (is_percent == true) {
                var v1 = num_thou((value[i - 1] * 100).toFixed(0)) + " %";
                var v2 = num_thou((value[i] * 100).toFixed(0)) + " %";
                c.name = v1 + " - " + v2;
            }
            else {
                var v1 = num_thou((value[i - 1]).toFixed(f));
                var v2 = num_thou((value[i]).toFixed(f));
                c.name = v1 + " - " + v2;
            }
        }
        color.push(c);
    }

    if (is_percent == true) {
        for (var i = 0; i < color.length; i++) {

        }
    }

    var color_class = {
        colorAxis: {
            dataClasses: color
        }
    };

    var map_opt = $.extend(true, _default_map_optn, color_class);
    show_map(current_area, _map_container, map_opt);

}

function get_station_value(yy, mm, code) {
    for (var i = 0; i < co_chart.length; i++) {
        var data = co_chart[i];
        if ((data.level == "Station") && (data.area == code)) {

            var first_day = new Date(data.time_value[0][0]);
            var fy = first_day.getFullYear();
            var fm = first_day.getMonth() + 1
            var index = ((yy - fy) * 12) + (mm - fm);
            if (index > data.time_value.length - 1) break;
            return data.time_value[index][1];

            break;
        }
    }
}

function get_next_level() {

    var area_level = ["ALL", "Air", "City", "Station"];
    if (current_area == "Station") return "Station";

    for (var i = 0; i < area_level.length; i++) {
        if (current_area == area_level[i]) {
            return area_level[i + 1];
            break;
        }
    }
}

function get_station_code_by_name(name) {
    for (var i = 0; i < area_code.length; i++) {
        var data = area_code[i];
        if ((data.level == "Station") && (data.text == name)) {
            return data.code;
        }
    }
}

function get_station_create_by_name(name) {
    for (var i = 0; i < area_code.length; i++) {
        var data = area_code[i];
        if ((data.level == "Station") && (data.text == name)) {
            var create_date = data.create_date;
            if (create_date == "") {
                create_date = "無提供資料";
            }
            return create_date;
        }
    }
    return "";
}

//顯示測站
function show_station() {
    var county_code = current_area_code;
    var s = new Array();
    for (var i = 0; i < co_chart.length; i++) {

        var data = co_chart[i];
        var value = get_station_value(current_sy, current_sm, data.area);
        var color = get_map_color(value);

        if ((data.level == "Station") && (data.CountyCode == county_code)) {

            s.push({
                name: data.SITENAME,
                x: data.x,
                y: data.y * -1,
                marker: {
                    fillColor: color,
                    lineColor: "#000000 ",
                    lineWidth: 1,
                    radius: 9,
                    symbol: "circle",
                    states: {
                        hover: {
                            enabled: false
                        }
                    }
                },
                events: {
                    click: function () {
                        var code = get_station_code_by_name(this.name);
                        change_chart_title("Station", code);
                        change_chart_data("Station", code);
                    }
                }
            });
        }
    }

    var series = null;
    for (var i = 0; i < map_object.series.length; i++) {
        if (map_object.series[i].name == "station") {
            series = map_object.series[i];
        }
    }

    if (s.length == 0 && series != null) {
        series.remove();
        return;
    }

    if (series == null) {
        map_object.addSeries({
            name: "station",
            type: "mappoint",
            mapData: geo_all,
            data: s,
            allAreas: false,
            showInLegend: false
        }, true);
    }
    else {
        series.setData(s, true);
    }

}

function hide_station() {
    var series = null;
    for (var i = 0; i < map_object.series.length; i++) {
        if (map_object.series[i].name == "station") {
            series = map_object.series[i];
        }
    }

    if (series != null) {
        series.remove();
    }
}

function show_map(area, container, opts) {
    // Make codes uppercase to match the map data

    _map_container = container;
    _default_map_optn = $.extend(true, {}, opts);

    Highcharts.setOptions({
        lang: {
            drillUpText: "返回上一層"
        }
    });

    var geo = null;
    var geo_data = [];
    if (area == "ALL") {
        geo = geo_all;
    }

    //傳回空品區 geo
    function get_air_geo(mapKey) {

        var geo = null;
        var geo_data = null;

        var area_country = new Array();
        var geo_c2 = $.extend(true, {}, geo_country);

        //找出空區的區域
        if (current_area == "Air") {
            for (var i = 0; i < area_code.length; i++) {
                if (area_code[i].level == "City" && area_code[i].AreaId == mapKey) {
                    area_country.push(area_code[i].code);
                }
            }
        }
        else if (current_area == "City") {
            for (var i = 0; i < area_code.length; i++) {
                if (area_code[i].level == "City" && area_code[i].code == mapKey) {
                    area_country.push(area_code[i].code);
                }
            }
        }

        //刪除其它的區
        for (var i = geo_c2.features.length - 1; i >= 0; i--) {
            var code = geo_c2.features[i].properties.CODE;
            var ck = false;
            for (var k = 0; k < area_country.length; k++) {
                if (area_country[k] == code) {
                    ck = true;
                    break;
                }
            }
            if (ck == false) {
                geo_c2.features.splice(i, 1);
            }
        }

        return geo_c2;

    }

    var map_opts = {

        chart: {
            type: "map",
            events: {
                drillup: function () {

                    pause_year();
                    change_area_level("up");
                    hide_station();

                    //找出上一層 code
                    map_level_ary.splice(map_level_ary.length - 1, 1);
                    var last_code = map_level_ary[map_level_ary.length - 1];
                    current_area_code = last_code;

                    //重設 chart 資料
                    change_chart_data();
                    var area_text = change_chart_title();

                    //設定地圖資料
                    change_year_data(current_sy, current_sm);
                    var area_text = change_chart_title();
                },
                drilldown: function (e) {

                    pause_year();

                    if (!e.seriesOptions) {

                        var chart = this;
                        if (current_area == "City") return;

                        // Show the spinner
                        chart.showLoading('載入地圖'); // Font Awesome spinner

                        //設定下一層級
                        change_area_level("down");
                        current_area_code = e.point.drilldown;
                        map_level_ary.push(current_area_code);

                        //找出下層的 geojson
                        var geo_c2 = get_air_geo(current_area_code);

                        // Load the drilldown map
                        var series = chart.addSeriesAsDrilldown(e.point, {
                            name: e.point.drilldown,
                            mapData: geo_c2,
                            joinBy: ['CODE', 'code'],
                            colors: ["#fff", "#fff"],
                            nullColor: "#fff",
                            tooltip: {
                                enabled: true
                            },
                            allAreas: false,
                            events: {
                                click: function () {
                                    if (current_area == "City") {
                                        change_chart_data();
                                        change_chart_title();
                                    }
                                }
                            },
                            legend: {
                                enabled: false
                            }
                        });

                        //重設 chart 資料
                        change_chart_data();
                        var area_text = change_chart_title();

                        //設定地圖資料
                        change_year_data(current_sy, current_sm);
                        chart.hideLoading();
                    }

                    //this.setTitle(null, { text: e.point.name });
                },
                load: function () {
                    //change_year_data(data_sy, data_sm, this);
                }
            }
        },

        drilldown: {
            //series: drilldownSeries,
            activeDataLabelStyle: {
                color: '#FFFFFF',
                textDecoration: 'none',
                textShadow: '0 0 3px #000000'
            },
            drillUpButton: {
                relativeTo: 'spacingBox',
                position: {
                    x: 0,
                    y: 45
                }
            }
        },

        title: {
        },

        exporting: {
            enabled: true,
            buttons: {
                contextButton: {
                    align: "right",
                    text: "輸出",
                }
            }
        },

        mapNavigation: {
            enabled: true,
            enableTouchZoom: true
        },

        tooltip: {
            enabled: true,
            formatter: function () {
                var code = this.point.code;
                var value = this.point.value;
                var level = get_next_level();
                var area_text = "";
                var create_date = "";

                if (level == "Station") {
                    var sn = this.series.name;
                    if (sn == "station") {
                        //測站
                        code = get_station_code_by_name(this.point.name);
                        area_text = this.point.name
                        value = get_station_value(current_sy, current_sm, code);
                        create_date = get_station_create_by_name(area_text);
                        create_date = "運作日期：" + create_date + "<br />";
                        if (value == null) value = "無";
                    }
                    else {
                        //city
                        level = "City"
                    }
                }

                for (var i = 0; i < area_code.length; i++) {
                    if ((area_code[i].level == level) && (area_code[i].code == code)) {
                        area_text = area_code[i].text;
                        break;
                    }
                }

                if (map_tooltip_value_func) {
                    value = map_tooltip_value_func(value);
                }

                return area_text + "<br />" + create_date + current_map_value_name + ": " + value;
            }
        },

        credits: {
            enabled: false
        },

        legend: {
            enabled: true,
            verticalAlign: 'bottom',
            align: "right",
            floating: true,
            width: 100,
            borderWidth: 0.5,
            borderRadius: 2,
            backgroundColor: "#ffffff",
            title: {
                text: current_map_value_name + " <br>"
                    + ((map_legend_sub_text == null) ? "月平均濃度值" : map_legend_sub_text)
            }
        },

        colorAxis: {

        },

        series: [{
            animation: {
                duration: 1000
            },
            allAreas: true,
            joinBy: ['sn', 'code'],
            mapData: geo,
            name: '全國空品區',
            nullColor: "#fff",
            tooltip: {
                pointFormatter: function (obj) {
                    var s = '<span style="color:' + this.color + '">\u25CF</span> ' + this.name + ': <b>';
                    var y = 2;
                    var v = parseFloat(this.y.toFixed(2));
                    s = s + v + ' </b><br/>';
                    return s;
                }
            },
        }]
    };

    map_opts = $.extend(true, map_opts, opts);

    // Instanciate the map
    map_object = $("#" + container).highcharts('Map', map_opts).highcharts();
}

function change_year_data(sy, sm, chart) {
    var area_code = current_area_code;
    var level = current_area;
    if (level != "ALL") level = "City";

    var geo_data = null;

    if (map_object == null) map_object = chart;
    if (map_object) {


        var has = false;
        for (var i = 0; i < co_map.length; i++) {
            if (co_map[i].level == level.toString()) {

                //var first_year = co_map[i].value[0].year;
                //var yy = parseInt(first_year.split("_")[0]);
                //var mm = parseInt(first_year.split("_")[1]);

                for (var k = 0; k < co_map[i].value.length; k++) {
                    var year = co_map[i].value[k].year;
                    var yy = parseInt(year.split("_")[0]);
                    var mm = parseInt(year.split("_")[1]);
                    if ((yy == sy) && (mm == sm)) {
                        geo_data = $.extend(true, [], co_map[i].value[k].data);
                        has = true;
                        break; //exit for [i]
                    }
                }

                //var index = ((sy - yy) * 12) + (sm - mm);
                //if (index < 0) break;
                //if (index > co_map[i].value.length - 1) break;

                //geo_data = $.extend(true, [], co_map[i].value[index].data);
                //has = true;
                //break; //exit for [i]
            }
        }

        if (geo_data == null) {
            map_object.series[0].setData([], true);
        }
        else {
            if (current_area != "City") {
                for (var i = 0; i < geo_data.length; i++) {
                    geo_data[i].drilldown = geo_data[i].code;
                }
                map_object.series[0].setData(geo_data, true);
            }
        }

        if (current_area == "City") {
            var null_data = $.extend(true, [], geo_data);
            for (var i = 0; i < null_data.length; i++) {
                null_data[i].drilldown = null;
                null_data[i].value = null;
            }
            map_object.series[0].setData(null_data, true);
            show_station();
        }

        var isnull = true;
        if (geo_data != null){
             for (var i = 0; i < geo_data.length; i++) {
                if (geo_data[i].value != null) {
                    isnull = false;
                    break;
                }
            }
        }

        var area_text = get_area_title();
        map_object.setTitle(
            {
                text: "" + area_text + "  " + current_site_name 
                + " " + (sy - 1911).toString() + " 年 "
                + ((data_unit == "month") ? sm + " 月" : "")
                + ((isnull == true) ? " 無資料" : "") ,
                style: {
                    fontSize: "14px"
                }
            }, {
                //text: current_map_value_name + " 月平均濃度值"
            }
        );

    }

}
