/*get a random color in hex
 lifted from -http://stackoverflow.com/questions/5092808/how-do-i-randomly-generate-html-hex-color-codes-using-javascript
*/
function get_random_color() {
    return '#' + (Math.random() * 0xFFFFFF << 0).toString(16);;
}


//Event to display on the timeline
function Event(title, start, end, template) {
    this.start = start;
    this.end = end;
    this.title = title;
    this.color = get_random_color();
    this.template = template;

    /*height and scale properties*/
    this.scale = ko.observable(1);
    this.height = ko.computed(function () {
        var days;
        if (!this.end) {
            days = moment().diff(this.start, "days");
        } else {
            days = this.end.diff(this.start, "days");
        }
        return days * this.scale();
    }, this);

    this.top = ko.computed(function () {
        var now = moment();
        if (!this.end) {
            return 0;
        }
        var diff = now.diff(this.end, 'days');
        return (diff * (this.scale()));

    }, this);

    /*time helpers*/
    this.fromEnd = function () {
        if (!this.end) {
            return "現在"
        }

        var d = new Date(this.end);
        var text = (d.getFullYear() - 1911) + " 年"
        return text;

        return this.end.from(moment());
    };
    this.fromStart = function () {
        var d = new Date(this.start);
        var text = (d.getFullYear() - 1911) + " 年"
        return text;
        //return this.start.from(moment());
    };
    /*selected*/
    this.selected = ko.observable(false);
    this.toggleSelected = function () {
        this.selected(!this.selected());
    };
}

//TODO: no overlapping event paradigm
//Left events
var leftEvents = [

    new Event("獨立訂定特定產業風險管理物質", moment("2010-01-01"), moment(), {
        name: '2010-01',
        context: {}
    }),
    new Event("注重產業製程特性，分類訂定管制限值，並增訂7日平均值", moment("2000-01-01"), moment("2009-12-31"), {
        name: '2000-01',
        context: {}
    }),
    new Event("結合輔導與改善分段加嚴管制限值", moment("1991-01-01"), moment("1999-12-31"), {
        name: '1991-01',
        context: {}
    }),
    new Event("訂定共同標準及個別標準，以二級處理為目標", moment("1983-01-01"), moment("1990-12-31"), {
        name: '1983-01',
        context: {}
    }),
    new Event("特定事業與濃度分別管制，以初級處理為目標", moment("1974-01-01"), moment("1982-12-31"), {
        name: '1974-01',
        context: {}
    }),
    new Event("標準起步統一濃度管制", moment("1970-01-01"), moment("1973-12-31"), {
        name: '1970-01',
        context: {}
    })
];

//right events
var rightEvents = [
    new Event("放流水標準第十六次修正", moment("2016-01-06"), moment(), {
        name: '2016-01-06',
        context: {}
    }),
    new Event("放流水標準第十五次修正", moment("2014-01-22"), moment("2016-01-05"), {
        name: '2014-01-22',
        context: {}
    }),
    new Event("放流水標準第十四次修正", moment("2012-10-12"), moment("2014-01-21"), {
        name: '2012-10-12',
        context: {}
    }),
    new Event("放流水標準第十三次修正", moment("2011-12-01"), moment("2012-10-11"), {
        name: '2011-12-01',
        context: {}
    }),
    new Event("放流水標準第十二次修正", moment("2010-12-15"), moment("2011-11-30"), {
        name: '2010-12-15',
        context: {}
    }),
    new Event("放流水標準第十一次修正", moment("2009-07-28"), moment("2010-12-14"), {
        name: '2009-07-28',
        context: {}
    }),
    new Event("放流水標準第十次修正", moment("2007-09-03"), moment("2009-07-27"), {
        name: '2007-09-03',
        context: {}
    }),
    new Event("放流水標準第九次修正", moment("2003-11-26"), moment("2007-09-02"), {
        name: '2003-11-26',
        context: {}
    }),
    new Event("放流水標準第七、八次修正", moment("2001-02-07"), moment("2003-11-25"), {
        name: '2001-02-07',
        context: {}
    }),
    new Event("放流水標準第六次修正", moment("2000-02-09"), moment("2001-02-06"), {
        name: '2000-02-09',
        context: {}
    }),
    new Event("放流水標準第五次修正", moment("1999-09-22"), moment("2000-02-08"), {
        name: '1999-09-22',
        context: {}
    }),
    new Event("放流水標準第三、四次修正", moment("1997-12-24"), moment("1999-09-21"), {
        name: '1997-12-24',
        context: {}
    }),
    new Event("放流水標準第一、二次修正", moment("1987-05-05"), moment("1997-12-23"), {
        name: '1987-05-05',
        context: {}
    }),
];


//Timeline Viewmodel
function ViewModel() {
    this.title = "放流水標準大事紀";
    this.subtitle = "民國59年-105年";

    this.lastIndex = 0;
    this.leftEvents = ko.observableArray(leftEvents);
    this.rightEvents = ko.observableArray(rightEvents);
    this.currentScale = ko.observable(.5);
    this.currentScale.subscribe(this.updateEventsScale.bind(this));

    //any selected
    this.itemSelected = ko.computed(function () {
        var leftSelected = this.leftEvents().some(function (event) {
            return event.selected();
        });
        //return early
        if (leftSelected) {
            return leftSelected;
        }
        var rightSelected = this.rightEvents().some(function (event) {
            return event.selected();
        });

        return rightSelected;
    }, this);

    //combined and sorted items
    this.combinedSorted = ko.computed(function () {

        var combined = this.leftEvents().concat(this.rightEvents());

        //sort by computed top poisition
        combined.sort(function (a, b) {

            if (a.top() < b.top())
                return -1;
            if (a.top() > b.top())
                return 1;
            return 0;

        });

        return combined;

    }, this);

};

ViewModel.prototype.updateEventsScale = function (value) {
    this.leftEvents().forEach(function (event) {
        event.scale(value);
    });
    this.rightEvents().forEach(function (event) {
        event.scale(value);
    });
};
ViewModel.prototype.scrollNext = function () {

    if (this.lastIndex < this.combinedSorted().length - 1) {
        this.lastIndex++;
    }
    var top = this.combinedSorted()[this.lastIndex].top()

    $.scrollTo(top, 250);

};
ViewModel.prototype.scrollPrevious = function () {

    if (this.lastIndex > 0) {
        this.lastIndex--;
    }
    var top = this.combinedSorted()[this.lastIndex].top()

    $.scrollTo(top, 250);

};

//bind
var vm = new ViewModel();
ko.applyBindings(vm);
vm.updateEventsScale(vm.currentScale());