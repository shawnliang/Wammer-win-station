(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'text!templates/partials/document.html'], function(_, Backbone, M, Template) {
    var DocumentView;
    return DocumentView = (function(_super) {

      __extends(DocumentView, _super);

      function DocumentView() {
        return DocumentView.__super__.constructor.apply(this, arguments);
      }

      DocumentView.prototype.className = 'document';

      DocumentView.prototype.render = function() {
        if (!!this.model) {
          this.setOrientation();
          this.setSize();
          this.$el.html(M.render(Template, this.model.toJSON()));
        }
        return this;
      };

      DocumentView.prototype.setSize = function() {
        if (this.options.height) {
          this.$el.css({
            height: this.options.height
          });
        }
        if (this.options.width) {
          return this.$el.css({
            width: this.options.width
          });
        }
      };

      DocumentView.prototype.setOrientation = function() {
        return this.$el.addClass(this.model.get('orientation'));
      };

      return DocumentView;

    })(Backbone.View);
  });

}).call(this);
