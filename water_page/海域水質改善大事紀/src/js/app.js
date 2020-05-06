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
//Left events
var leftEvents = [
    new Event("水體環境水質改善及經營管理計畫", moment("2012-01-01"), moment(), {
        name: '2012-01-01',
        context: {}
    }),
    new Event("河川及海洋水質維護改善計畫(第二期)", moment("2008-01-01"), moment("2011-12-31"), {
        name: '2008-01-01',
        context: {}
    }),
    new Event("河川及海洋水質維護改善計畫(第一期)", moment("2005-01-01"), moment("2007-12-31"), {
        name: '2005-01-01',
        context: {}
    }),
    new Event("臺灣地區河川流域及海洋經營管理方案", moment("2001-01-01"), moment("2004-12-31"), {
        name: '2001-01-01',
        context: {}
    }),

];

//right events
var rightEvents = [
    new Event("海洋棄置許可管理辦法", moment("2009-01-01"), moment("2009-12-31"), {
        name: '2009-01-01',
        context: {}
    }),
    new Event("重大海洋油污染緊急應變計畫", moment("2001-01-01"), moment("2001-12-31"), {
        name: '2001-01-01',
        context: {}
    }),
    new Event("公布實施海洋污染防治法", moment("2000-01-01"), moment("2000-12-31"), {
        name: '2000-01-01',
        context: {}
    }),
    new Event("推動碧海計畫", moment("1988-01-01"), moment("1988-12-31"), {
        name: '1988-01-01',
        context: {}
    }),
];


//Timeline Viewmodel
function ViewModel() {
    this.title = "海域水質改善大事紀";
    this.subtitle = "民國77年-105年";

    this.lastIndex = 0;
    this.leftEvents = ko.observableArray(leftEvents);
    this.rightEvents = ko.observableArray(rightEvents);
    this.currentScale = ko.observable(.3);
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