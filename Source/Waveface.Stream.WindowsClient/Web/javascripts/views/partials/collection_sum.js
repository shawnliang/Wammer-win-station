(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'text!templates/partials/collection_sum.html'], function(_, Backbone, M, Template) {
    var CollectionView;
    return CollectionView = (function(_super) {

      __extends(CollectionView, _super);

      function CollectionView() {
        return CollectionView.__super__.constructor.apply(this, arguments);
      }

      CollectionView.prototype.className = 'collection';

      CollectionView.prototype.events = {
        'click': 'viewDetail'
      };

      CollectionView.prototype.initialize = function() {};

      CollectionView.prototype.render = function() {
        this.$el.html(M.render(Template, this.model.toJSON()));
        return this;
      };

      CollectionView.prototype.viewDetail = function() {
        return this.options.parent.trigger('viewDetail', this.model);
      };

      return CollectionView;

    })(Backbone.View);
  });

}).call(this);