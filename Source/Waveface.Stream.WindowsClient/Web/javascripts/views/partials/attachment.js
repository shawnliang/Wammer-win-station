(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'text!templates/partials/attachment.html'], function(_, Backbone, M, Template) {
    var EventView;
    return EventView = (function(_super) {

      __extends(EventView, _super);

      function EventView() {
        return EventView.__super__.constructor.apply(this, arguments);
      }

      EventView.prototype.className = 'attachment';

      EventView.prototype.render = function() {
        if (!!this.model) {
          this.$el.html(M.render(Template, this.model.toJSON()));
          this.setOrientation();
        }
        if (this.options.height) {
          this.$el.css({
            height: this.options.height
          });
        }
        if (this.options.width) {
          this.$el.css({
            width: this.options.width
          });
        }
        return this;
      };

      EventView.prototype.setOrientation = function() {
        return this.$('.frame').addClass(this.model.get('orientation'));
      };

      return EventView;

    })(Backbone.View);
  });

}).call(this);
