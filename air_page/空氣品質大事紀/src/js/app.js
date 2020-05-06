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

    new Event("空氣污染防制法第六、七次修正", moment("2011-01-01"), moment(), {
        name: '67',
        context: {}
    }),
    new Event("空氣污染防制法第三、四、五、六次修正", moment("1999-01-01"), moment("2010-12-31"), {
        name: '3456',
        context: {}
    }),
    new Event("空氣污染防制法第二次修正", moment("1992-01-01"), moment("1998-12-31"), {
        name: '2',
        context: {}
    }),
    new Event("空氣污染防制法第一次修正", moment("1982-01-01"), moment("1991-12-31"), {
        name: '1',
        context: {}
    }),
    new Event("制定空氣污染防制法", moment("1975-01-01"), moment("1981-12-31"), {
        name: '1975-01',
        context: {}
    }),
];

//right events
var rightEvents = [

    new Event("訂定公告「高屏地區空氣污染物總量管制計畫」", moment("2015-01-01"), moment(), {
        name: '2015-01',
        context: {}
    }),
    new Event("「空氣污染防制法」第八次修正、實施汽/柴油第五期排放標準、車用汽油硫含量降至0.001 %", moment("2012-01-01"), moment("2012-12-31"), {
        name: '2012-01',
        context: {}
    }),
    new Event("「空氣污染防制法」第七次修正、實施車用高級柴油硫含量降至0.001 %、訂定公布室內空氣品質管理法", moment("2011-01-01"), moment("2011-12-31"), {
        name: '2011-01',
        context: {}
    }),
    new Event("全國加油站裝設油氣回收設備、實施機車第五期排放標準", moment("2007-01-01"), moment("2007-12-31"), {
        name: '2007-01',
        context: {}
    }),
    new Event("「空氣污染防制法」第六次修正、實施柴油車第五期排放標準", moment("2006-01-01"), moment("2006-12-31"), {
        name: '2006-01-01',
        context: {}
    }),
    new Event("「空氣污染防制法」第五次修正、開始增設監測細懸浮微粒", moment("2005-01-01"), moment("2005-12-31"), {
        name: '2005-01-01',
        context: {}
    }),
    new Event("「空氣污染防制法」第四次修正、實施機車第四期排放標準", moment("2002-01-01"), moment("2002-12-31"), {
        name: '2002-01-01',
        context: {}
    }),
    new Event("全面使用無鉛汽油", moment("2000-01-01"), moment("2000-12-31"), {
        name: '2000-01',
        context: {}
    }),
    new Event("「空氣污染防制法」第三次修正、實施汽/柴油第三期排放標準", moment("1999-01-01"), moment("1999-12-31"), {
        name: '1999-01-01',
        context: {}
    }),
    new Event("修正空污費改為依實際排放量徵收、實施機車第三期排放標準、全面實施機車排氣定期檢驗、實施高級柴油含硫量降至0.05 %", moment("1998-01-01"), moment("1998-12-31"), {
        name: '1998-01',
        context: {}
    }),
    new Event("開徵空氣污染防制費", moment("1995-01-01"), moment("1995-12-31"), {
        name: '1995-01-01',
        context: {}
    }),
    new Event("實施固定污染源空氣污染排放許可制度、完成臺灣空氣品質測站設置、實施柴油車第二期排放標準、全面禁止使用合硫量超過1.0 %之燃料油", moment("1993-01-01"), moment("1993-12-31"), {
        name: '1993-01',
        context: {}
    }),
    new Event("「空氣污染防制法」第二次修正、訂定發布「空氣品質標準」", moment("1992-01-01"), moment("1992-12-31"), {
        name: '1992-01-01',
        context: {}
    }),
    new Event("實施機車第二期排放標準", moment("1991-01-01"), moment("1991-12-31"), {
        name: '1991-01-01',
        context: {}
    }),
    new Event("實施汽油車第二期排放標準管制、全面禁止使用合硫量超過1.5 %之燃料油、新汽車出廠一律限用無鉛汽油", moment("1990-01-01"), moment("1990-12-31"), {
        name: '1990-01-01',
        context: {}
    }),
    new Event("實施機車第一期排放標準", moment("1988-01-01"), moment("1988-12-31"), {
        name: '1988-01-01',
        context: {}
    }),
    new Event("環保署成立；實施汽油車及柴油車第一期排放標準", moment("1987-01-01"), moment("1987-12-31"), {
        name: '1987-01-01',
        context: {}
    }),
    new Event("固定污染源空氣污染物排放標準第一次修正", moment("1986-01-01"), moment("1986-12-31"), {
        name: '1986-01-01',
        context: {}
    }),
];


//Timeline Viewmodel
function ViewModel() {
    this.title = "空氣品質大事紀";
    this.subtitle = "民國64年-105年";

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