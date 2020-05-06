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

    new Event("水污染防治法第五次修正", moment("2007-12-12"), moment(), {
        name: '2007-12',
        context: {}
    }),
    new Event("發布「違反水污染防治法按日連續處罰執行原則」", moment("2003-01-01"), moment("2003-12-31"), {
        name: '2003-01-01',
        context: {}
    }),
    new Event("水污染防治法第四次修正", moment("2002-05-22"), moment("2007-12-11"), {
        name: '2002-05',
        context: {}
    }),
    new Event("水污染防治法第三次修正", moment("2000-04-26"), moment("2002-05-21"), {
        name: '2000-04',
        context: {}
    }),
    new Event("水污染防治法第二次修正", moment("1991-05-06"), moment("2000-04-25"), {
        name: '1991-05',
        context: {}
    }),
    new Event("水污染防治法第一次修正", moment("1983-05-27"), moment("1991-05-05"), {
        name: '1983-05',
        context: {}
    }),
    new Event("水污染防治法首次公布實施", moment("1974-07-11"), moment("1983-05-26"), {
        name: '1974-07',
        context: {}
    }),
];

//right events
var rightEvents = [

    new Event("全國首例核定「桃園市新街溪及埔心溪總量管制方式」", moment("2016-01-01"), moment(), {
        name: '',
        context: {}
    }),
    new Event("發布「違反水污染防治法罰鍰額度裁罰準則」", moment("2008-01-01"), moment("2008-12-31"), {
        name: '2008-01',
        context: {}
    }),
    new Event("發布「違反水污染防治法通知限期改善或補正裁量基準」、「水污染防治措施計畫及許可申請審查辦法」及「水污染防治措施及檢測申報管理辦法」", moment("2006-01-01"), moment("2006-12-31"), {
        name: '2006-01',
        context: {}
    }),
    new Event("發布「預鑄試建築物污水處理設施管理辦法」", moment("2003-01-01"), moment("2003-12-31"), {
        name: '2003-01',
        context: {}
    }),
    new Event("發布「土壤處理標準」", moment("1999-01-01"), moment("1999-12-31"), {
        name: '1999-01',
        context: {}
    }),
    new Event("發布「廢(污)水排放收費辦法」", moment("1998-01-01"), moment("1998-12-31"), {
        name: '1998-01',
        context: {}
    }),
    new Event("發布「海洋放流水標準」", moment("1994-01-01"), moment("1994-12-31"), {
        name: '1994-01',
        context: {}
    }),
    new Event("發布「事業廢水處理專責單位或人員設置辦法」", moment("1988-01-01"), moment("1988-12-31"), {
        name: '1988-01',
        context: {}
    }),
    new Event("發布「放流水標準」", moment("1987-01-01"), moment("1987-12-31"), {
        name: '1987-01',
        context: {}
    }),
    new Event("發布「水體分類及水質標準」", moment("1985-01-01"), moment("1985-12-31"), {
        name: '1985-01',
        context: {}
    }),
    new Event("臺灣省政府發布「臺灣省工廠礦場放流水標準」", moment("1976-01-01"), moment("1976-12-31"), {
        name: '1976-01',
        context: {}
    }),
    new Event("經濟部發布「水污染防治法施行細則」", moment("1975-01-01"), moment("1975-12-31"), {
        name: '1975-01',
        context: {}
    }),
    new Event("總統明令公布「水污染防治法」", moment("1974-01-01"), moment("1974-12-31"), {
        name: '1974-01',
        context: {}
    }),
    new Event("經濟部發布「工廠廢水管理辦法」", moment("1970-01-01"), moment("1970-12-31"), {
        name: '1970-01',
        context: {}
    }),
];


//Timeline Viewmodel
function ViewModel() {
    this.title = "水污染防治法重要規定建置相關歷程";
    this.subtitle = "1970-2016";

    this.lastIndex = 0;
    this.leftEvents = ko.observableArray(leftEvents);
    this.rightEvents = ko.observableArray(rightEvents);
    this.currentScale = ko.observable(.2);
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