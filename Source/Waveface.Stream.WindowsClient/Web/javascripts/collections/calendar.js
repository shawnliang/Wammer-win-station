(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/calendar', 'localstorage', 'eventbundler', 'moment'], function(_, Backbone, Calendar, LocalStorage, EventBundler, Moment) {
    var CalendarCollection;
    CalendarCollection = (function(_super) {

      __extends(CalendarCollection, _super);

      function CalendarCollection() {
        return CalendarCollection.__super__.constructor.apply(this, arguments);
      }

      CalendarCollection.prototype.model = Calendar;

      CalendarCollection.prototype.schemaName = 'calendars';

      CalendarCollection.prototype.localStorage = new LocalStorage('calendars');

      CalendarCollection.prototype.initialize = function() {
        return dispatch.on("store:change:" + this.schemaName + ":all", this.updateCalendar, this);
      };

      CalendarCollection.prototype.updateCalendar = function(data, ns) {
        var calendars,
          _this = this;
        calendars = data.calendar_entries;
        _.each(calendars, function(calendar) {
          return _this.add(calendar);
        });
        return dispatch.trigger("render:change:" + this.schemaName + ":" + ns, data.calendars);
      };

      CalendarCollection.prototype.callCalendar = function(ns) {
        var memo, params;
        if (ns == null) {
          ns = "all";
        }
        new EventBundler("GetCalendar", ns);
        memo = {
          namespace: ns,
          type: this.schemaName
        };
        params = {
          group_by: 0,
          page_size: 100
        };
        return wfwsocket.sendMessage("getCalendarEntries", params, memo);
      };

      CalendarCollection.prototype.photosByDate = function(date) {
        var attachment_count, data;
        if (!date) {
          return 0;
        }
        date = Moment(date, "YYYY-M-D").format('YYYY-MM-DD');
        data = this.get(date);
        if (data) {
          attachment_count = data.get("attachment_count");
          return attachment_count;
        }
        return 0;
      };

      CalendarCollection.prototype.eventsByDate = function(date) {
        var data, post_count;
        if (!date) {
          return 0;
        }
        date = Moment(date, "YYYY-M-D").format('YYYY-MM-DD');
        data = this.get(date);
        if (data) {
          post_count = data.get("post_count");
          return post_count;
        }
        return 0;
      };

      return CalendarCollection;

    })(Backbone.Collection);
    return new CalendarCollection();
  });

}).call(this);
