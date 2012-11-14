(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/event_sum', 'views/event_detail', 'views/event_nav', 'text!templates/events.html'], function(_, Backbone, M, EventSumView, EventDetailView, EventNavView, Template) {
    var EventsView;
    return EventsView = (function(_super) {

      __extends(EventsView, _super);

      function EventsView() {
        return EventsView.__super__.constructor.apply(this, arguments);
      }

      EventsView.prototype.className = 'events';

      EventsView.prototype.initialize = function() {
        this.collection.on('add', this.addOne, this);
        return this.collection.on('reset', this.render, this);
      };

      EventsView.prototype.render = function() {
        this.renderMain();
        this.renderNav();
        this.addAll();
        return this;
      };

      EventsView.prototype.renderMain = function() {
        this.$('#main').html(M.render(Template, {}));
        return this;
      };

      EventsView.prototype.renderNav = function() {
        var view;
        view = new EventNavView();
        this.$('#nav').html(view.render().el);
        return this;
      };

      EventsView.prototype.addAll = function() {
        var _this = this;
        if (this.collection.length > 1) {
          this.viewEventDetail(this.collection.first());
        }
        return this.collection.each(function(model) {
          return _this.addOne(model);
        });
      };

      EventsView.prototype.addOne = function(event) {
        var view;
        view = new EventSumView({
          model: event
        });
        view.on('viewDetail', this.viewEventDetail, this);
        this.$('#events').append(view.render().el);
        if (this.collection.length === 1) {
          return this.viewEventDetail(event);
        }
      };

      EventsView.prototype.viewEventDetail = function(event) {
        var view;
        view = new EventDetailView({
          model: event
        });
        return this.$('#event-detail').empty().append(view.render().el);
      };

      return EventsView;

    })(Backbone.View);
  });

}).call(this);
