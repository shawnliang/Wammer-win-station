(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'views/layouts/menu', 'lib/galleria/galleria-1.2.8'], function(_, Backbone, MenuView, Galleria) {
    var AppView;
    return AppView = (function(_super) {

      __extends(AppView, _super);

      function AppView() {
        return AppView.__super__.constructor.apply(this, arguments);
      }

      AppView.prototype.el = "#ncApp";

      AppView.prototype.initialize = function() {
        this.renderMenu();
        return Galleria.loadTheme('javascripts/lib/galleria/themes/classic/galleria.classic.js');
      };

      AppView.prototype.renderMenu = function() {
        return this.$('#menu').empty().append((new MenuView).render().el);
      };

      return AppView;

    })(Backbone.View);
  });

}).call(this);
