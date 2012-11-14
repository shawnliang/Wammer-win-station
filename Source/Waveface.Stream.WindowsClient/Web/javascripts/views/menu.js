(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['backbone', 'mustache', 'text!templates/menu.html'], function(Backbone, M, Template) {
    var EventView;
    return EventView = (function(_super) {

      __extends(EventView, _super);

      function EventView() {
        return EventView.__super__.constructor.apply(this, arguments);
      }

      EventView.prototype.events = {
        'click .nav-to-events': 'menuEvents',
        'click .nav-to-photos': 'menuPhotos',
        'click .nav-to-calendar': 'menuCalendar'
      };

      EventView.prototype.initialize = function() {
        dispatch.on('menuEvents', this.menuEvents, this);
        dispatch.on('menuPhotos', this.menuPhotos, this);
        return dispatch.on('menuCalendar', this.menuCalendar, this);
      };

      EventView.prototype.render = function() {
        this.$el.html(M.render(Template, {}));
        return this;
      };

      EventView.prototype.highlight = function(e, className) {
        if (e != null) {
          e.preventDefault();
        }
        this.$('li').removeClass('active');
        return this.$(className).addClass('active');
      };

      EventView.prototype.menuEvents = function(e) {
        this.highlight(e, '.nav-to-events');
        return dispatch.trigger('viewEvents');
      };

      EventView.prototype.menuPhotos = function(e) {
        this.highlight(e, '.nav-to-photos');
        dispatch.trigger('callAttachments');
        return dispatch.trigger('viewPhotos');
      };

      EventView.prototype.menuCalendar = function(e) {
        this.highlight(e, '.nav-to-calendar');
        return dispatch.trigger('viewCalendar');
      };

      return EventView;

    })(Backbone.View);
  });

}).call(this);
