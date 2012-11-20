(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'collections/events', 'collections/attachments', 'views/menu', 'eventbundler'], function(_, Backbone, Events, Attachments, MenuView, EventBundler) {
    var AppView;
    return AppView = (function(_super) {

      __extends(AppView, _super);

      function AppView() {
        return AppView.__super__.constructor.apply(this, arguments);
      }

      AppView.prototype.el = "#ncApp";

      AppView.prototype.initialize = function() {
        new EventBundler('GetPosts');
        Events.callPosts();
        Attachments.callAttachments();
        this.render();
        dispatch.on('all', function(e) {
          return Logger.log("Event trigger : " + e);
        });
        return Galleria.loadTheme('javascripts/lib/galleria/themes/classic/galleria.classic.js');
      };

      AppView.prototype.render = function() {
        return this.renderMenu();
      };

      AppView.prototype.renderMenu = function() {
        return this.$('#menu').empty().append(MenuView.render().el);
      };

      return AppView;

    })(Backbone.View);
  });

}).call(this);
