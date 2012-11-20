(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'text!templates/event_calendar.html', 'lib/jquery/fullcalendar'], function(_, Backbone, M, Template) {
    var CalendarView;
    CalendarView = (function(_super) {

      __extends(CalendarView, _super);

      function CalendarView() {
        return CalendarView.__super__.constructor.apply(this, arguments);
      }

      CalendarView.prototype.id = 'event-calendar';

      CalendarView.prototype.render = function() {
        this.renderMain();
        this.renderCalendar();
        return this;
      };

      CalendarView.prototype.renderMain = function() {
        this.$el.html(M.render(Template));
        return this;
      };

      CalendarView.prototype.renderCalendar = function() {
        var calendarOptions;
        calendarOptions = {
          events: [],
          dayClick: function() {
            dispatch.trigger('viewEventDate');
            return dispatch.trigger('navDate');
          }
        };
        this.$('#calendar').fullCalendar(calendarOptions);
        return this;
      };

      return CalendarView;

    })(Backbone.View);
    return new CalendarView;
  });

}).call(this);
