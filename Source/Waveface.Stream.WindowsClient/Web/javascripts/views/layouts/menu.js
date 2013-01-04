(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'text!templates/layouts/menu.html', 'collections/events', 'com/user', 'lib/lodash/underscore.string'], function(_, Backbone, M, Template, Events, User) {
    var MenuView;
    return MenuView = (function(_super) {

      __extends(MenuView, _super);

      function MenuView() {
        return MenuView.__super__.constructor.apply(this, arguments);
      }

      MenuView.prototype.user = User;

      MenuView.prototype.initialize = function() {
        dispatch.on("user:info:save", this.renderUserInfo, this);
        Router.on('route:actionEvents', this.menuEvents, this);
        Router.on('route:actionPhotos', this.menuPhotos, this);
        Router.on('route:actionDocs', this.menuDocs, this);
        Router.on('route:actionCalendar', this.menuCalendar, this);
        Router.on('route:actionCollection', this.menuCollection, this);
        return Router.on('route:actionReading', this.menuReading, this);
      };

      MenuView.prototype.render = function() {
        var data;
        data = {
          username: _.str.truncate(User.get("nickname", 12))
        };
        this.$el.html(M.render(Template, data));
        this.renderUserInfo();
        return this;
      };

      MenuView.prototype.menuEvents = function(e) {
        return this.highlight('.nav-to-events');
      };

      MenuView.prototype.menuPhotos = function(e) {
        return this.highlight('.nav-to-photos');
      };

      MenuView.prototype.menuDocs = function(e) {
        return this.highlight('.nav-to-docs');
      };

      MenuView.prototype.menuReading = function() {
        return this.highlight('.nav-to-reading');
      };

      MenuView.prototype.menuCalendar = function(e) {
        return this.highlight('.nav-to-calendar');
      };

      MenuView.prototype.menuCollection = function(e) {
        return this.highlight('.nav-to-collection');
      };

      MenuView.prototype.highlight = function(className) {
        console.log("hightlight: " + className);
        this.$('li').removeClass('active');
        return this.$(className).addClass('active');
      };

      MenuView.prototype.renderUserInfo = function() {
        var data;
        data = {
          username: _.str.truncate(User.get("nickname"), 12)
        };
        return this.$('#username').html(data.username);
      };

      return MenuView;

    })(Backbone.View);
  });

}).call(this);
