(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'collections/events', 'collections/attachments', 'views/menu', 'views/events', 'views/photos', 'views/calendar', 'text!templates/app.html', 'eventbundler', 'localstorage'], function(_, Backbone, M, Events, Attachments, MenuView, EventsView, PhotosView, CalendarView, Template, EventBundler, Store) {
    var AppView;
    return AppView = (function(_super) {

      __extends(AppView, _super);

      function AppView() {
        return AppView.__super__.constructor.apply(this, arguments);
      }

      AppView.prototype.el = "#ncApp";

      AppView.prototype.initialize = function() {
        new EventBundler('GetPosts');
        new EventBundler('GetAttachments', 'all');
        Events.callPosts();
        Attachments.callAttachments();
        this.render();
        dispatch.on('all', function(e) {
          return Logger.log("Event trigger : " + e);
        });
        dispatch.on('viewEvents', this.renderEvents, this);
        dispatch.on('viewCalendar', this.renderCalendar, this);
        dispatch.on('viewPhotos', this.renderPhotos, this);
        return dispatch.trigger('menuEvents');
      };

      AppView.prototype.render = function() {
        return this.renderMenu();
      };

      AppView.prototype.renderMenu = function() {
        var view;
        view = new MenuView();
        return this.$('#menu').html(view.render().el);
      };

      AppView.prototype.renderEvents = function() {
        var view;
        view = new EventsView({
          el: '#client',
          collection: Events
        });
        return view.render();
      };

      AppView.prototype.renderPhotos = function() {
        var view;
        view = new PhotosView({
          el: '#client',
          collection: Attachments
        });
        return view.render();
      };

      AppView.prototype.renderCalendar = function() {
        var view;
        view = new CalendarView({
          el: '#client',
          collection: Events
        });
        return view.render();
      };

      return AppView;

    })(Backbone.View);
  });

}).call(this);
