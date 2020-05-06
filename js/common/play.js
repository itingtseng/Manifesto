var play_timer = null;
var play_d1 = null, play_d2 = null, play_current = null;
var current_action = "";

function go_next_month() {

    if (play_current > play_d2) play_current = play_d1;

    current_sy = play_current.getFullYear();
    current_sm = play_current.getMonth() + 1;

    //draw plot line
    ShowPlotLine(current_sy, current_sm);

    //change map data
    change_year_data(current_sy, current_sm);

    //play month ++
    play_current = new Date(current_sy, current_sm, 1);
}

function set_play_date_range() {

    var extremes = stock_chart.xAxis[0].getExtremes();
    play_d1 = new Date(extremes.min);
    play_d2 = new Date(extremes.max);

    if ((play_current < play_d1) || (play_current > play_d2)) {
        play_current = play_d1;
    }

}

function play_year() {
    if ((stock_chart == null) || (map_object == null)) {
        window.setTimeout("play_year();", 200);
        return;
    }

    set_play_date_range();

    current_action = "play";
    play_timer = window.setInterval("go_next_month();", 500);
    $("#img_action_play").attr("src", "../../images/pause.png");
    EnabledChartPlot(false);
}

function pause_year() {
    if (play_timer != null) {
        current_action = "pause";
        window.clearInterval(play_timer);
        $("#img_action_play").attr("src", "../../images/play.png");
    }
    EnabledChartPlot(true);
}

function go_year_start() {
    if (current_action == "play") {
        pause_year();
    }

    set_play_date_range();

    play_current = play_d1;
    current_sy = play_current.getFullYear();
    current_sm = play_current.getMonth() + 1;

    //draw plot line
    ShowPlotLine(current_sy, current_sm);

    //change map data
    change_year_data(current_sy, current_sm);

}

function go_year_end() {
    if (current_action == "play") {
        pause_year();
    }

    set_play_date_range();

    play_current = play_d2;
    current_sy = play_current.getFullYear();
    current_sm = play_current.getMonth() + 1;

    //draw plot line
    ShowPlotLine(current_sy, current_sm);

    //change map data
    change_year_data(current_sy, current_sm);
}

function btn_action() {
    if (current_action == "play") {
        pause_year();
    }
    else {
        play_year();
    }
}