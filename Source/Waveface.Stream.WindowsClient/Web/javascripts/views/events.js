(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/event_sum', 'views/event_detail', 'views/event_menu', 'collections/events', 'text!templates/events.html'], function(_, Backbone, M, EventSumView, EventDetailView, EventMenuView, Events, Template) {
    var EventsView;
    EventsView = (function(_super) {

      __extends(EventsView, _super);

      function EventsView() {
        return EventsView.__super__.constructor.apply(this, arguments);
      }

      EventsView.prototype.id = 'events';

      EventsView.prototype.initialize = function() {
        this.collection.on('add', this.addOne, this);
        return this.collection.on('reset', this.render, this);
      };

      EventsView.prototype.render = function(date) {
        var _this = this;
        this.$el.html(M.render(Template, {}));
        this.collection.filterByDate(date).each(function(model, index) {
          return _this.addOne(model, index);
        });
        this.delegateEvents();
        return this;
      };

      EventsView.prototype.renderNav = function() {
        var view;
        view = new EventMenuView();
        $('#nav').html(view.render().el);
        return this;
      };

      EventsView.prototype.addOne = function(event, index) {
        var view;
        view = new EventSumView({
          model: event
        });
        view.on('viewDetail', this.viewEventDetail, this);
        this.$('#event-list').append(view.render().el);
        if (index === 0 || this.collection.length === 1) {
          return this.viewEventDetail(event);
        }
      };

      EventsView.prototype.viewEventDetail = function(event) {
        if (!event) {
          return false;
        }
        if (this.detailView != null) {
          Logger.log('KILL Zombie');
          this.detailView.remove();
          this.detailView.model.off();
        }
        this.detailView = new EventDetailView({
          model: event
        });
        return this.$('#event-detail').empty().append(this.detailView.render().el);
      };

      return EventsView;

    })(Backbone.View);
    return new EventsView({
      collection: Events
    });
  });

}).call(this);
