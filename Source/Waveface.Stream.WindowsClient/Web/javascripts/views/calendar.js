(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'collections/events', 'text!templates/calendar.html', 'lib/jquery/fullcalendar'], function(_, Backbone, M, Events, Template) {
    var CalendarView;
    CalendarView = (function(_super) {

      __extends(CalendarView, _super);

      function CalendarView() {
        return CalendarView.__super__.constructor.apply(this, arguments);
      }

      CalendarView.prototype.id = 'event-calendar';

      CalendarView.prototype.initialize = function() {};

      CalendarView.prototype.render = function() {
        this.events = this.collection.map(function(model) {
          var data;
          data = {
            start: moment(model.get('timestamp')).format('YYYY-MM-DD'),
            title: 'test'
          };
          return data;
        });
        this.$el.html(M.render(Template));
        return this;
      };

      CalendarView.prototype.renderCalendar = function() {
        var calendarOptions;
        calendarOptions = {
          events: this.events,
          eventClick: function(EventObj) {
            return Router.navigate('events/' + moment(EventObj.start).format('YYYY-MM-DD'), {
              trigger: true
            });
          }
        };
        this.$('#calendar').fullCalendar(calendarOptions);
        return this;
      };

      return CalendarView;

    })(Backbone.View);
    return new CalendarView({
      collection: Events
    });
  });

}).call(this);
