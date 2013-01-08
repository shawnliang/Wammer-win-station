(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['jquery', 'backbone', 'mustache', 'views/partials/attachment', 'views/gallery', 'collections/attachments', 'text!templates/layouts/day_view_calendar.html', 'lib/jquery/jquery.blockUI', 'lib/jquery/fullcalendar'], function($, Backbone, M, AttachmentView, GalleryView, Attachments, CalendarTemplate) {
    var DayView;
    return DayView = (function(_super) {

      __extends(DayView, _super);

      function DayView() {
        return DayView.__super__.constructor.apply(this, arguments);
      }

      DayView.prototype.tagName = 'section';

      DayView.prototype.mode = "calendar";

      DayView.prototype.events = {
        'click #nextDateBtn': 'nextPage',
        'click .nextDate': 'nextPage',
        'click #previousDateBtn': 'previousPage',
        'click .previousDate': 'previousPage',
        'click .header-date': 'calendarSwitch',
        'click #nextMonthBtn': 'nextMonth',
        'click #previousMonthBtn': 'previousMonth'
      };

      DayView.prototype.nextPage = function() {
        if (this.nextDateUri) {
          return Router.navigate("" + this.id + "/" + this.nextDateUri, {
            trigger: true
          });
        }
      };

      DayView.prototype.previousPage = function() {
        if (this.previousDateUri = this.collection.previousDate(this.date)) {
          return Router.navigate("" + this.id + "/" + this.previousDateUri, {
            trigger: true
          });
        }
      };

      DayView.prototype.setDates = function(date) {
        var nextDateUri, previousDateUri;
        this.date = date;
        this.currentDate = moment(this.date).format('MMMM DD, YYYY');
        this.currentYear = moment(this.date).format('YYYY');
        this.currentMonth = moment(this.date).format('M') - 1;
        this.currentYearMonth = moment(this.date).format('MMMM YYYY');
        previousDateUri = this.collection.previousDate(this.date, false);
        this.previousDateUri = previousDateUri ? previousDateUri : null;
        this.previousDate = previousDateUri ? moment(previousDateUri).format('MMMM DD') : null;
        nextDateUri = this.collection.nextDate(this.date);
        this.nextDateUri = nextDateUri ? nextDateUri : null;
        return this.nextDate = nextDateUri ? moment(nextDateUri).format('MMMM DD') : null;
      };

      DayView.prototype.setHeaderStyle = function() {
        this.nextDateUri && this.$('#nextDateBtn').removeClass('disabled') || this.$('#nextDateBtn').addClass('disabled');
        return this.previousDateUri && this.$('#previousDateBtn').removeClass('disabled') || this.$('#previousDateBtn').addClass('disabled');
      };

      DayView.prototype.renderCalendar = function(calendarOptions) {
        var data,
          _this = this;
        data = {
          currentDate: this.currentYearMonth
        };
        this.$el.html(M.render(CalendarTemplate, data));
        this.$calendar = this.$('#calendar').fullCalendar(calendarOptions);
        Mousetrap.reset();
        Mousetrap.bind("right", function() {
          _this.nextMonth();
          return false;
        });
        Mousetrap.bind("left", function() {
          _this.previousMonth();
          return false;
        });
        return Mousetrap.bind("c", function() {
          _this.calendarSwitch();
          return false;
        });
      };

      DayView.prototype.setKey = function() {
        var _this = this;
        Mousetrap.reset();
        Mousetrap.bind('right', function() {
          _this.nextPage();
          return false;
        });
        Mousetrap.bind('left', function() {
          _this.previousPage();
          return false;
        });
        return Mousetrap.bind('c', function() {
          _this.calendarSwitch();
          return false;
        });
      };

      DayView.prototype.updateHeaderDate = function(date) {
        if (date != null) {
          return this.$('.header-date').text(date);
        }
      };

      DayView.prototype.nextMonth = function() {
        return this.$calendar.fullCalendar("next");
      };

      DayView.prototype.previousMonth = function() {
        return this.$calendar.fullCalendar("prev");
      };

      return DayView;

    })(Backbone.Marionette.View);
  });

}).call(this);
