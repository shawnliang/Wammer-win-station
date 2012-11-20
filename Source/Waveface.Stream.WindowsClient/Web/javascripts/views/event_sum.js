(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/attachment', 'models/attachment', 'text!templates/event_sum.html'], function(_, Backbone, M, attachmentView, attachmentModel, template) {
    var EventView;
    return EventView = (function(_super) {

      __extends(EventView, _super);

      function EventView() {
        return EventView.__super__.constructor.apply(this, arguments);
      }

      EventView.prototype.className = 'event';

      EventView.prototype.initialize = function() {};

      EventView.prototype.events = function() {
        return {
          'click': 'viewDetail'
        };
      };

      EventView.prototype.render = function() {
        this.$el.html(M.render(template, this.model.toJSON()));
        return this;
      };

      EventView.prototype.viewDetail = function() {
        return this.trigger('viewDetail', this.model);
      };

      return EventView;

    })(Backbone.View);
  });

}).call(this);
