(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['jquery', 'backbone', 'mustache', 'text!templates/event_menu.html'], function($, Backbone, M, Template) {
    var EventView;
    return EventView = (function(_super) {

      __extends(EventView, _super);

      function EventView() {
        return EventView.__super__.constructor.apply(this, arguments);
      }

      EventView.prototype.id = 'event-nav';

      EventView.prototype.events = {
        'click .nav-date': 'navDate'
      };

      EventView.prototype.initialize = function() {
        return dispatch.on('navDate', this.navDate, this);
      };

      EventView.prototype.render = function() {
        this.$el.html(M.render(Template, {}));
        return this;
      };

      EventView.prototype.navDate = function(e) {
        if (e != null) {
          e.preventDefault();
        }
        return dispatch.trigger('viewEventDate');
      };

      return EventView;

    })(Backbone.View);
  });

}).call(this);
