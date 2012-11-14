(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/events', 'views/eventDetail', 'text!templates/myDay.html'], function(_, Backbone, M, EventView, EventDetailView, template) {
    var MyDayView;
    return MyDayView = (function(_super) {

      __extends(MyDayView, _super);

      function MyDayView() {
        return MyDayView.__super__.constructor.apply(this, arguments);
      }

      MyDayView.prototype.className = 'myDay';

      MyDayView.prototype.initialize = function() {
        console.log('init MyDayView');
        this.collection.on('add', this.addOne, this);
        return this.collection.on('reset', this.render, this);
      };

      MyDayView.prototype.render = function() {
        this.$el.html(M.render(template, {}));
        return this;
      };

      MyDayView.prototype.addOne = function(event) {
        var view;
        view = new EventView({
          model: event
        });
        view.on('viewDetail', this.viewEventDetail, this);
        return this.$('#events').append(view.render().el);
      };

      MyDayView.prototype.viewEventDetail = function(event) {
        var view;
        view = new EventDetailView({
          model: event
        });
        return this.$('#event-detail').empty().append(view.render().el);
      };

      return MyDayView;

    })(Backbone.View);
  });

}).call(this);
