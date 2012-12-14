(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'collections/events', 'collections/attachments', 'text!templates/calendar_d3.html', 'lib/d3'], function(_, Backbone, M, Events, Photos, Template, d3) {
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
        'click #calendar-control .photos': 'renderPhotos'
      };

      CalendarView.prototype.initialize = function() {
        return this.photoDays = this.testData();
      };

      CalendarView.prototype.render = function() {
        this.$el.html(M.render(Template));
        Mousetrap.reset();
        this.delegateEvents();
        return this;
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
          return '#photos/' + d.date;
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
          return '#events/' + d.date;
        });
        return this.rects.transition().duration(500).attr("class", "event-rect").attr("height", function(d) {
          return "" + (_this.eventBarHeight(d.event_count)) + "%";
        }).attr("y", function(d) {
          return "" + (100 - _this.eventBarHeight(d.event_count)) + "%";
        });
      };

      CalendarView.prototype.eventBarHeight = function(event_count) {
        var h, r;
        if (event_count === 0) {
          return 0;
        }
        h = event_count * 10 + 10;
        if (h > 100) {
          r = 100;
        }
        return h;
      };

      CalendarView.prototype.photoBarHeight = function(photo_count) {
        var h, r;
        if (photo_count === 0) {
          return 0;
        }
        h = photo_count + 10;
        if (h > 100) {
          r = 100;
        }
        return h;
      };

      CalendarView.prototype.testData = function() {
        var data, i, m, _i, _j;
        data = [];
        for (m = _i = 0; _i <= 11; m = ++_i) {
          data[m] = {
            month: '2012-' + (m + 1),
            month_i18n: moment().month(m).format('MMM'),
            day: []
          };
          for (i = _j = 0; _j <= 30; i = ++_j) {
            data[m]['day'][i] = {
              date: "2012-" + (m + 1) + "-" + (i + 1),
              photo_count: _.random(0, 50),
              event_count: _.random(0, 5)
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
