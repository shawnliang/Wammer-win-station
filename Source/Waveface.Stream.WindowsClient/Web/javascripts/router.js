(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'moment', 'collections/events', 'collections/attachments', 'collections/documents', 'collections/calendar', 'views/events', 'views/photos', 'views/documents', 'views/calendar', 'views/collections', 'localstorage', 'com/subscriber'], function(_, Backbone, Moment, Events, Attachments, Documents, Calendars, EventsView, PhotosView, DocsView, CalendarView, CollectionView, Storage, Subscriber) {
    var AppRouter, root;
    root = this;
    return AppRouter = (function(_super) {

      __extends(AppRouter, _super);

      function AppRouter() {
        return AppRouter.__super__.constructor.apply(this, arguments);
      }

      AppRouter.prototype.routes = {
        '': 'actionEvents',
        '/': 'actionEvents',
        'events': 'actionEvents',
        'events/': 'actionEvents',
        'events/:date': 'actionEvents',
        'events/:date/': 'actionEvents',
        'events/:date/:id': 'actionEvents',
        'photos': 'actionPhotos',
        'photos/': 'actionPhotos',
        'photos/:date': 'actionPhotos',
        'documents': 'actionDocs',
        'documents/': 'actionDocs',
        'documents/:date': 'actionDocs',
        'reading': 'actionReading',
        'calendar': 'actionCalendar',
        'calendar/:year': 'actionCalendar',
        'collection': 'actionCollection'
      };

      AppRouter.prototype.initialize = function() {
        Storage.reset(['nc_events_view_data', 'nc_photos_view_data', 'nc_view_state:events', 'nc_view_state:photos']);
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
        state = new Storage('nc_view_state:events');
        if (Events.length === 0) {
          Events.callPosts();
        }
        if (!date && !id) {
          if (state.data.id != null) {
            id = state.data.id;
          } else if ((last = Events.first())) {
            date = last.get('dateUri');
          }
        }
        this.main.empty().append(EventsView.render(date, id).el);
        if (id != null) {
          state.data = {
            id: id
          };
          state.save();
        }
        Events.subscribe();
        Events.subscribe("update:event", false, false, Subscriber.E_POS_UPD);
      };

      AppRouter.prototype.actionPhotos = function(date) {
        var last, viewState;
        viewState = new Storage('nc_view_state:photos');
        if (Attachments.length === 0) {
          Attachments.callAttachments();
        }
        if (!date) {
          if (viewState.data.date != null) {
            date = viewState.data.date;
          } else if ((last = Attachments.first())) {
            date = last.get('dateUri');
          }
        }
        this.main.empty().append(PhotosView.render(date).el);
        viewState.data = {
          date: date
        };
        return viewState.save();
      };

      AppRouter.prototype.actionDocs = function(date) {
        var last, viewState;
        viewState = new Storage('nc_view_state:docs');
        if (Documents.length === 0) {
          Documents.callAttachments();
        }
        if (!date) {
          if (viewState.data.date != null) {
            date = viewState.data.date;
          } else if ((last = Documents.first())) {
            date = last.get('dateUri');
          }
        }
        this.main.empty().append(DocsView.render(date).el);
        viewState.data = {
          date: date
        };
        return viewState.save();
      };

      AppRouter.prototype.actionReading = function() {
        return true;
      };

      AppRouter.prototype.actionCalendar = function(year) {
        year = year || Moment().format('YYYY');
        this.main.empty().append(CalendarView.render(year).el);
        return CalendarView.renderCalendar();
      };

      AppRouter.prototype.actionCollection = function() {
        return this.main.empty().append(CollectionView.render().el);
      };

      return AppRouter;

    })(Backbone.Router);
  });

}).call(this);
