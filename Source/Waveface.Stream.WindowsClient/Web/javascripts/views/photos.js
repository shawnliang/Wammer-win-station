(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/event_nav', 'views/attachment', 'text!templates/photos.html'], function(_, Backbone, M, EventNavView, AttachmentView, Template) {
    var PhotosView;
    return PhotosView = (function(_super) {

      __extends(PhotosView, _super);

      function PhotosView() {
        return PhotosView.__super__.constructor.apply(this, arguments);
      }

      PhotosView.prototype.className = 'photos';

      PhotosView.prototype.initialize = function() {
        this.collection.on('add', this.addOne, this);
        return this.collection.on('reset', this.render, this);
      };

      PhotosView.prototype.render = function() {
        this.renderMain();
        this.renderNav();
        this.addAll();
        return this;
      };

      PhotosView.prototype.renderMain = function() {
        this.$('#main').html(M.render(Template));
        return this;
      };

      PhotosView.prototype.renderNav = function() {
        var view;
        view = new EventNavView();
        this.$('#nav').html(view.render().el);
        return this;
      };

      PhotosView.prototype.addAll = function() {
        var _this = this;
        return this.collection.each(function(model) {
          var view;
          view = new AttachmentView({
            model: model
          });
          return _this.$("#photos").append(view.render().el);
        });
      };

      PhotosView.prototype.addOne = function(model) {
        var view;
        view = new AttachmentView({
          model: model
        });
        return this.$("#photos").append(view.render().el);
      };

      return PhotosView;

    })(Backbone.View);
  });

}).call(this);
