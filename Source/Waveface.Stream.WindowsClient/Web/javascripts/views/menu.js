(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['backbone', 'mustache', 'text!templates/menu.html', 'collections/events', 'com/user'], function(Backbone, M, Template, Events, User) {
    var MenuView;
    MenuView = (function(_super) {

      __extends(MenuView, _super);

      function MenuView() {
        return MenuView.__super__.constructor.apply(this, arguments);
      }

      MenuView.prototype.render = function() {
        Router.on('route:actionEvents', this.menuEvents, this);
        Router.on('route:actionPhotos', this.menuPhotos, this);
        Router.on('route:actionCalendar', this.menuCalendar, this);
        this.$el.html(M.render(Template, {
          username: User.get("nickname")
        }));
        return this;
      };

      MenuView.prototype.menuEvents = function(e) {
        return this.highlight('.nav-to-events');
      };

      MenuView.prototype.menuPhotos = function(e) {
        return this.highlight('.nav-to-photos');
      };

      MenuView.prototype.menuCalendar = function(e) {
        return this.highlight('.nav-to-calendar');
      };

      MenuView.prototype.highlight = function(className) {
        this.$('li').removeClass('active');
        return this.$(className).addClass('active');
      };

      return MenuView;

    })(Backbone.View);
    return new MenuView;
  });

}).call(this);
