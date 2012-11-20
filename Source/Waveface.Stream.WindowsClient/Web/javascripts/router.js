(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'collections/events', 'views/events', 'views/photos', 'views/calendar'], function(_, Backbone, Events, EventsView, PhotosView, CalendarView) {
    var AppRouter, root;
    root = this;
    AppRouter = (function(_super) {

      __extends(AppRouter, _super);

      function AppRouter() {
        return AppRouter.__super__.constructor.apply(this, arguments);
      }

      AppRouter.prototype.routes = {
        '': 'actionEvents',
        'events': 'actionEvents',
        'events/:date': 'actionEvents',
        'photos': 'actionPhotos',
        'photos/:date': 'actionPhotos',
        'calendar': 'actionCalendar'
      };

      AppRouter.prototype.initialize = function() {
        this.on('all', function(trigger, args) {
          return Logger.log("Event trigger : " + trigger);
        });
        return this.main = $("#main");
      };

      AppRouter.prototype.actionEvents = function(date) {
        var today;
        today = moment().format("YYYY-MM-DD");
        return this.main.empty().append(EventsView.render(date).el);
      };

      AppRouter.prototype.actionPhotos = function(date) {
        var today;
        today = moment().format("YYYY-MM-DD");
        return this.main.empty().append(PhotosView.render(date).el);
      };

      AppRouter.prototype.actionCalendar = function() {
        return this.main.empty().append(CalendarView.render().el);
      };

      return AppRouter;

    })(Backbone.Router);
    return root.Router = new AppRouter();
  });

}).call(this);
