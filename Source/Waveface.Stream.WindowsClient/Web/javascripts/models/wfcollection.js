(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone'], function(_, Backbone) {
    var WFCollectionModel;
    return WFCollectionModel = (function(_super) {

      __extends(WFCollectionModel, _super);

      function WFCollectionModel() {
        return WFCollectionModel.__super__.constructor.apply(this, arguments);
      }

      WFCollectionModel.prototype.initialize = function() {
        return true;
      };

      return WFCollectionModel;

    })(Backbone.Model);
  });

}).call(this);
