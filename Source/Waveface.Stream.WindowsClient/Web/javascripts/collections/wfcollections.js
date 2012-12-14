(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/wfcollection'], function(_, Backbone, WFCollection) {
    var WFCollectionCollection;
    return WFCollectionCollection = (function(_super) {

      __extends(WFCollectionCollection, _super);

      function WFCollectionCollection() {
        return WFCollectionCollection.__super__.constructor.apply(this, arguments);
      }

      WFCollectionCollection.prototype.model = WFCollection;

      WFCollectionCollection.prototype.callCollections = function() {};

      return WFCollectionCollection;

    })(Backbone.Collection);
  });

}).call(this);
