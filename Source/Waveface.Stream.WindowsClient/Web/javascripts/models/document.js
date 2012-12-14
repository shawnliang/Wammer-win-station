(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'moment'], function(_, Backbone, Moment) {
    var DocumentModel;
    return DocumentModel = (function(_super) {

      __extends(DocumentModel, _super);

      function DocumentModel() {
        return DocumentModel.__super__.constructor.apply(this, arguments);
      }

      DocumentModel.prototype.initialize = function() {
        return this.setDate();
      };

      DocumentModel.prototype.setDate = function() {
        var attach_time;
        attach_time = this.get('timestamp');
        if (!attach_time) {
          return false;
        }
        return this.set('dateUri', Moment(attach_time).format('YYYY-MM-DD'));
      };

      return DocumentModel;

    })(Backbone.Model);
  });

}).call(this);
