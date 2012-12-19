(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/partials/collection_sum', 'views/partials/collection_detail', 'text!templates/collections.html'], function(_, Backbone, M, CollectionSumView, CollectionDetailView, Template) {
    var CollectionsView;
    CollectionsView = (function(_super) {

      __extends(CollectionsView, _super);

      function CollectionsView() {
        return CollectionsView.__super__.constructor.apply(this, arguments);
      }

      CollectionsView.prototype.id = 'collections';

      CollectionsView.prototype.events = {
        'click .collection': 'renderDetail',
        'click .collection-detail': 'renderGrid'
      };

      CollectionsView.prototype.initialize = function() {};

      CollectionsView.prototype.render = function() {
        this.$el.html(M.render(Template));
        this.delegateEvents();
        this.renderGrid();
        return this;
      };

      CollectionsView.prototype.renderGrid = function() {
        var num, view, _i, _results;
        this.$('.content-detail').hide();
        this.$('.content-grid').show();
        this.$('.collection-grid').empty();
        _results = [];
        for (num = _i = 1; _i <= 20; num = ++_i) {
          view = new CollectionSumView;
          _results.push(this.$('.collection-grid').append(view.render().el));
        }
        return _results;
      };

      CollectionsView.prototype.renderDetail = function() {
        var view;
        this.$('.content-detail').empty().show();
        this.$('.content-grid').hide();
        view = new CollectionDetailView;
        return this.$('.content-detail').append(view.render().el);
      };

      return CollectionsView;

    })(Backbone.View);
    return new CollectionsView();
  });

}).call(this);
