
(function ($) {

    function _line(sender, opt) {
        var def_opt = {
            sender: "",
            img1: "",
            img2: "",
            event_obj: [],
            go_url: ""
        };

        var fn_opt = $.extend(def_opt, opt);
        var load_timer = null;

        //load img
        var _img1 = new Image();
        var _img2 = new Image();
        var _img1_load = false;
        var _img2_load = false;
        var _img_width = 0;
        var _img_height = 0;
        var _canvas = null;
        var context = null;

        _img1.onload = function () { _img1_load = true; }
        _img2.onload = function () { _img2_load = true; }

        _img1.src = fn_opt.img1;
        _img2.src = fn_opt.img2;

        var check_img_load = function () {
            if (_img1_load && _img2_load) {
                _img_width = _img1.width;
                _img_height = _img1.height;

                clearInterval(load_interval);
                DrawImage();
            }
        };
        var load_interval = window.setInterval(check_img_load, 0);

        var DrawImage = function () {
            _canvas = $("<canvas>").attr("width", _img_width).attr("height", _img_height);
            sender.append(_canvas);

            context = _canvas.get(0).getContext("2d");
            var p = 1;
            context.drawImage(_img1, 0, 0, _img_width, _img_height, 0, 0, _img_width, _img_height);

            //set event
            $(_canvas).mouseover(function () {
                func_play();
            });
            $(_canvas).mouseout(function () {
                func_stop();
            });
            $(_canvas).click(function () {
                func_go_url();
            });

        }

        var _play_interval = null;
        var _play_index = 0;
        this.Play = function () {
            _play_index = 0;
            clearInterval(_play_interval);
            _play_interval = window.setInterval(play_func, 10);
        };

        var play_func = function () {
            if (_img1_load && _img2_load) {
                _play_index = _play_index + 2;
                context.clearRect(0, 0, _img_width, _img_height);
                context.drawImage(_img1, 0, 0, _img_width, _img_height, 0, 0, _img_width, _img_height);
                context.drawImage(_img2, 0, _play_index, _img_width, _img_height, 0, _play_index, _img_width, _img_height);
                if (_play_index > _img_height) { _play_index = 0; }
            }
        };

        this.Stop = function () {
            if (_img1_load && _img2_load) {
                if (_play_interval) {
                    clearInterval(_play_interval);
                    context.clearRect(0, 0, _img_width, _img_height);
                    context.drawImage(_img1, 0, 0, _img_width, _img_height, 0, 0, _img_width, _img_height);
                }
            }
        }

        var func_play = (function (obj) {
            return function () {
                obj.Play();
            }
        })(this);

        var func_stop = (function (obj) {
            return function () {
                obj.Stop();
            }
        })(this);

        var func_go_url = function () {
            if (fn_opt.go_url != "") {
                location.href = fn_opt.go_url;
            }
        }

        //set event
        $(fn_opt.event_obj).each(function () {
            $("" + this).mouseover(function () {
                func_play();
            });
            $("" + this).mouseout(function () {
                func_stop();
            });
            $("" + this).click(function () {
                func_go_url();
            });
        });

    };

    $.fn.extend({
        line_canvas: function (opt) {
            var obj = new _line($(this), opt);
            return obj;
        }
    });
})(jQuery);

