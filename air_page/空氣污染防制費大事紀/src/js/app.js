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

    new Event("開徵第一階段空污費依燃料油實際使用量徵收", moment("1995-01-01"), moment("1995-12-31"), {
        name: '1995-01',
        context: {}
    }),
    new Event("開徵第二階段空污費依實際污染排放量徵收、開徵氮氧化物空污費、公告硫氧化物與氮氧化物排放係數及控制效率", moment("1998-01-01"), moment("1998-12-31"), {
        name: '1998-01',
        context: {}
    }),
    new Event("調整氮氧化物收費費率反應環境空氣品質之污染現況", moment("2001-01-01"), moment("2001-12-31"), {
        name: '2001-01',
        context: {}
    }),
    new Event("修正公告硫氧化物與氮氧化物空污費收費費率、依燃油種類、成分、數量、指定公告物質修正空污費費率", moment("2004-01-01"), moment("2004-12-31"), {
        name: '2004-01',
        context: {}
    }),
    new Event("修正公告硫氧化物與氮氧化物排放係數及控制效率、公告揮發性有機物排放係數及控制效率、開徵揮發性有機物", moment("2007-01-01"), moment("2007-12-31"), {
        name: '2007-01',
        context: {}
    }),
    new Event("公告揮發性有機物質量平衡計畫規定", moment("2010-01-01"), moment("2010-12-31"), {
        name: '2010-01',
        context: {}
    }),
    new Event("公告修正VOCs空污費之排放係數、公告修正VOCs空污費之排放係數", moment("2012-01-01"), moment("2012-12-31"), {
        name: '2012-01',
        context: {}
    }),
    new Event("新增電子化繳費代收服務及繳費單線上列印功能，增加業者繳費便利性", moment("2014-01-01"), moment("2014-12-31"), {
        name: '2014-01',
        context: {}
    }),
    new Event("訂定發布淘汰二行程機車及新購電動二輪車補助辦法", moment("2015-01-01"), moment(), {
        name: '',
        context: {}
    }),
];

//right events
var rightEvents = [

    new Event("落實污染者付費擴大空污費污染徵收項目、依照工程類別開徵營建工程空污費（依照實際排量徵收）", moment("1997-01-01"), moment("1997-12-31"), {
        name: '1997-01',
        context: {}
    }),
    new Event("修正空污費排放量計算要點與收費辦法、修正公告硫氧化物與氮氧化物排放係數及控制效率、配合空污法修正內容修正空污費相關徵收條文內容", moment("1999-01-01"), moment("1999-12-31"), {
        name: '1999-01',
        context: {}
    }),
    new Event("公告「高級柴油依其銷售量，向銷售者或進口者徵收空氣污染防制費」", moment("2002-01-01"), moment("2002-12-31"), {
        name: '',
        context: {}
    }),
    new Event("修正公告硫氧化物與氮氧化物排放及揮發性有機物空污費徵收費率", moment("2006-01-01"), moment("2006-12-31"), {
        name: '2006-01',
        context: {}
    }),
    new Event("修正公告空污費費率、公告揮發性有機物自廠係數建置作業要點", moment("2009-01-01"), moment("2009-12-31"), {
        name: '2009-01',
        context: {}
    }),
    new Event("公告修正固定污染源空氣污染防制費收費費率優惠係數計量方式規定", moment("2011-01-01"), moment("2011-12-31"), {
        name: '2011-01',
        context: {}
    }),
    new Event("修正排放量計量規定", moment("2013-01-01"), moment("2013-12-31"), {
        name: '2013-01',
        context: {}
    }),
    new Event("修正固定污染源空氣污染防制費收費費率之適用零費率範圍、修正固定污染源空氣污染防制費收費費率之適用零費率範圍、「訂定發布機動車輛停車怠速管理辦法」", moment("2012-01-01"), moment("2012-12-31"), {
        name: '2012',
        context: {}
    }),
    new Event("推動VOCs簡化申報表單及新版系統之開放、公告修正FLARE之SOx、NOx空污費排放係數，以符合實際排放現況", moment("2014-01-01"), moment("2014-12-31"), {
        name: '2014',
        context: {}
    }),
    new Event("實施柴油汽車黑煙排放不透光率檢測方法及程序", moment("2015-01-01"), moment(), {
        name: '',
        context: {}
    }),
];


//Timeline Viewmodel
function ViewModel() {
    this.title = "空氣污染防制費大事紀";
    this.subtitle = "民國84年-103年";

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