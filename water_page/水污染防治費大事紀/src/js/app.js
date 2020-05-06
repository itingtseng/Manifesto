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

    new Event("修正「水污染防治法」", moment("1991-01-01"), moment("1991-12-31"), {
        name: '1991-01',
        context: {}
    }),
    new Event("修正「水污染防治法」", moment("2002-01-01"), moment("2002-12-31"), {
        name: '2002-01',
        context: {}
    }),
    new Event("提送「水污染防治費徵收及使用計畫」報院核定", moment("2005-01-01"), moment("2005-12-31"), {
        name: '2005-01',
        context: {}
    }),
    new Event("強化立院溝通", moment("2007-01-01"), moment("2007-12-31"), {
        name: '2007-01',
        context: {}
    }),
    new Event("修正發布「水污染防治費收費辦法」，104年5月1日開徵", moment("2015-01-01"), moment("2015-12-31"), {
        name: '2015-01',
        context: {}
    }),
];

//right events
var rightEvents = [

    new Event("發布「廢（污）水排放收費辦法」", moment("1998-01-01"), moment("1998-12-31"), {
        name: '1998-01',
        context: {}
    }),
    new Event("健全水污提防治費徵收，奠定階段徵收", moment("2004-01-01"), moment("2004-12-31"), {
        name: '2004-01',
        context: {}
    }),
    new Event("發布「水污染防治費收費辦法」", moment("2006-01-01"), moment("2006-12-31"), {
        name: '2006-01',
        context: {}
    }),
    new Event("加強產業輔導，積極謀取水污費開徵共識", moment("2010-01-01"), moment("2010-12-31"), {
        name: '2010-01',
        context: {}
    }),
];


//Timeline Viewmodel
function ViewModel() {
    this.title = "水污染防治費大事紀";
    this.subtitle = "民國91年-104年";

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