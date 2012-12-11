(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'collections/events', 'collections/attachments', 'text!templates/calendar.html', 'lib/jquery/fullcalendar'], function(_, Backbone, M, Events, Photos, Template) {
    var CalendarView;
    CalendarView = (function(_super) {

      __extends(CalendarView, _super);

      function CalendarView() {
        return CalendarView.__super__.constructor.apply(this, arguments);
      }

      CalendarView.prototype.id = 'event-calendar';

      CalendarView.prototype.eventsCollection = Events;

      CalendarView.prototype.photosCollection = Photos;

      CalendarView.prototype.initialize = function() {};

      CalendarView.prototype.render = function() {
        this.eventDays = this.eventsCollection.map(function(model) {
          return {
            start: moment(model.get('timestamp')).format('YYYY-MM-DD'),
            title: 'Events',
            allDay: true,
            backgroundColor: '#C85E52',
            borderColor: '#C85E52'
          };
        });
        console.log(this.eventDays);
        this.eventDays = _.uniq(this.eventDays, false, function(day) {
          return day.start;
        });
        this.photoDays = this.photosCollection.map(function(model) {
          return {
            start: model.get('dateUri'),
            title: 'Photos',
            allDay: true,
            backgroundColor: '#6E93AA',
            borderColor: '#6E93AA'
          };
        });
        this.photoDays = _.uniq(this.photoDays, false, function(day) {
          return day.start;
        });
        this.$el.html(M.render(Template));
        return this;
      };

      CalendarView.prototype.renderCalendar = function() {
        var calendarOptions,
          _this = this;
        calendarOptions = {
          height: $('#main').height() - 40,
          events: this.eventDays.concat(this.photoDays),
          eventClick: function(EventObj) {
            return Router.navigate('events/' + moment(EventObj.start).format('YYYY-MM-DD'), {
              trigger: true
            });
          }
        };
        this.$calendar = this.$('#calendar').fullCalendar(calendarOptions);
        Mousetrap.reset();
        Mousetrap.bind('right', function() {
          _this.nextMonth();
          return false;
        });
        Mousetrap.bind('left', function() {
          _this.previousMonth();
          return false;
        });
        Mousetrap.bind('c', function() {
          _this.calendarSwitch();
          return false;
        });
        return this;
      };

      CalendarView.prototype.nextMonth = function() {
        return this.$calendar.fullCalendar('next');
      };

      CalendarView.prototype.previousMonth = function() {
        return this.$calendar.fullCalendar('prev');
      };

      return CalendarView;

    })(Backbone.View);
    return new CalendarView({
      collection: Events
    });
  });

}).call(this);
