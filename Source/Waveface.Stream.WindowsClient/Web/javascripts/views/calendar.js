(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'collections/events', 'collections/attachments', 'collections/calendar', 'text!templates/calendar.html', 'lib/d3', 'moment'], function(_, Backbone, M, Events, Photos, Calendars, Template, d3, Moment) {
    var CalendarView;
    CalendarView = (function(_super) {

      __extends(CalendarView, _super);

      function CalendarView() {
        return CalendarView.__super__.constructor.apply(this, arguments);
      }

      CalendarView.prototype.id = 'calendar';

      CalendarView.prototype.eventsCollection = Events;

      CalendarView.prototype.photosCollection = Photos;

      CalendarView.prototype.events = {
        'click #calendar-control .events': 'renderEvents',
        'click #calendar-control .photos': 'renderPhotos',
        'click #previousDateBtn': 'previousYear',
        'click #nextDateBtn': 'nextYear'
      };

      CalendarView.prototype.render = function(year) {
        this.year = year;
        this.photoDays = this.fetchData(this.year);
        this.$el.html(M.render(Template, {
          year: this.year
        }));
        Mousetrap.reset();
        this.delegateEvents();
        return this;
      };

      CalendarView.prototype.nextYear = function() {
        this.year++;
        return Backbone.history.navigate("calendar/" + this.year, {
          trigger: true
        });
      };

      CalendarView.prototype.previousYear = function() {
        this.year -= 1;
        return Backbone.history.navigate("calendar/" + this.year, {
          trigger: true
        });
      };

      CalendarView.prototype.renderCalendar = function() {
        return this.renderTable();
      };

      CalendarView.prototype.renderTable = function() {
        this.dateRow = d3.select('.month tbody').selectAll('tr').data(this.photoDays).enter().append('tr');
        this.dateRow.append('th').text(function(d) {
          return d.month_i18n;
        });
        this.dateCell = this.dateRow.selectAll('td').data(function(d) {
          return d.day;
        }).enter().append('td').attr('class', function(d) {
          var _ref;
          if ((_ref = moment(d.date, 'YYYY-M-D').format('d')) === '0' || _ref === '6') {
            return 'holiday';
          }
        }).text(function(d, i) {
          if (i < moment(d.date, 'YYYY-M').daysInMonth()) {
            return i + 1;
          }
        }).append("div").attr('class', 'dateCell').append("svg").attr('width', '100').attr('height', '100');
        this.dateLink = this.dateCell.append('a');
        this.rects = this.dateLink.append("rect").attr("width", '100%').attr("x", '0').attr("y", '100');
        this.renderEvents();
        this.dateRow.append('td');
        return this;
      };

      CalendarView.prototype.renderPhotos = function() {
        var _this = this;
        this.$('#calendar-control .btn').removeClass("active");
        this.$('#calendar-control .photos').addClass("active");
        this.dateLink.attr("xlink:href", function(d) {
          return '#photos/' + Moment(d.date, "YYYY-M-D").format('YYYY-MM-DD');
        });
        return this.rects.transition().duration(500).attr("class", "photo-rect").attr("height", function(d) {
          return "" + (_this.photoBarHeight(d.photo_count)) + "%";
        }).attr("y", function(d) {
          return "" + (100 - _this.photoBarHeight(d.photo_count)) + "%";
        });
      };

      CalendarView.prototype.renderEvents = function() {
        var _this = this;
        this.$('#calendar-control .btn').removeClass("active");
        this.$('#calendar-control .events').addClass("active");
        this.dateLink.attr("xlink:href", function(d) {
          return '#events/' + Moment(d.date, "YYYY-M-D").format('YYYY-MM-DD');
        });
        return this.rects.transition().duration(500).attr("class", "event-rect").attr("height", function(d) {
          return "" + (_this.eventBarHeight(d.event_count)) + "%";
        }).attr("y", function(d) {
          return "" + (100 - _this.eventBarHeight(d.event_count)) + "%";
        });
      };

      CalendarView.prototype.eventBarHeight = function(event_count) {
        var h;
        if (event_count === 0) {
          return 0;
        }
        h = event_count * 10;
        if (h > 100) {
          h = 100;
        }
        return h;
      };

      CalendarView.prototype.photoBarHeight = function(photo_count) {
        var h;
        if (photo_count === 0) {
          return 0;
        }
        h = photo_count + 10;
        if (h > 100) {
          h = 100;
        }
        return h;
      };

      CalendarView.prototype.fetchData = function(year) {
        var data, date, i, m, _i, _j;
        data = [];
        for (m = _i = 0; _i <= 11; m = ++_i) {
          data[m] = {
            month: ("" + year + "-") + (m + 1),
            month_i18n: moment().month(m).format('MMM'),
            day: []
          };
          for (i = _j = 0; _j <= 30; i = ++_j) {
            date = "" + year + "-" + (m + 1) + "-" + (i + 1);
            data[m]['day'][i] = {
              date: date,
              photo_count: Calendars.photosByDate(date),
              event_count: Calendars.eventsByDate(date)
            };
          }
        }
        return data;
      };

      return CalendarView;

    })(Backbone.View);
    return new CalendarView({
      collection: Events
    });
  });

}).call(this);
