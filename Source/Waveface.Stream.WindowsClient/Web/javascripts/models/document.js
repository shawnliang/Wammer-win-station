(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone'], function(_, Backbone) {
    var DocumentModel;
    return DocumentModel = (function(_super) {

      __extends(DocumentModel, _super);

      function DocumentModel() {
        return DocumentModel.__super__.constructor.apply(this, arguments);
      }

      DocumentModel.prototype.initialize = function() {
        return true;
      };

      return DocumentModel;

    })(Backbone.Model);
  });

}).call(this);
