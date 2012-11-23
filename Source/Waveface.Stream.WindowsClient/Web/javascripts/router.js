(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'collections/events', 'views/events', 'collections/attachments', 'views/photos', 'views/calendar', 'localstorage'], function(_, Backbone, Events, EventsView, Attachments, PhotosView, CalendarView, Storage) {
    var AppRouter, root;
    root = this;
    return AppRouter = (function(_super) {

      __extends(AppRouter, _super);

      function AppRouter() {
        return AppRouter.__super__.constructor.apply(this, arguments);
      }

      AppRouter.prototype.routes = {
        '': 'actionEvents',
        'events': 'actionEvents',
        'events/:date': 'actionEvents',
        'events/:date/:id': 'actionEvents',
        'photos': 'actionPhotos',
        'photos/:date': 'actionPhotos',
        'calendar': 'actionCalendar'
      };

      AppRouter.prototype.initialize = function() {
        dispatch.on('all', function(e) {
          return Logger.log("Event trigger : " + e);
        });
        this.on('all', function(e) {
          return Logger.log("Router Event trigger : " + e);
        });
        return this.main = $("#main");
      };

      AppRouter.prototype.actionEvents = function(date, id) {
        var last, state;
        if (Events.length === 0) {
          Events.callPosts();
        }
        state = new Storage('nc_view_state:events');
        if (!date && !id && (last = Events.first())) {
          date = moment(last.get("timestamp")).format("YYYY-MM-DD");
        }
        if (!id && !date && state.data.id) {
          id = state.data.id;
        }
        this.main.empty().append(EventsView.render(date, id).el);
        state.data = {
          date: date,
          id: id
        };
        return state.save();
      };

      AppRouter.prototype.actionPhotos = function(date) {
        var today;
        if (Attachments.length === 0) {
          Attachments.callAttachments();
        }
        today = moment().format("YYYY-MM-DD");
        date = date || today;
        return this.main.empty().append(PhotosView.render(date).el);
      };

      AppRouter.prototype.actionCalendar = function() {
        this.main.empty().append(CalendarView.render().el);
        return CalendarView.renderCalendar();
      };

      return AppRouter;

    })(Backbone.Router);
  });

}).call(this);
