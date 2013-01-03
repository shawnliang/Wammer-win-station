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

      DocumentModel.prototype.baseRatio = 1;

      DocumentModel.prototype.potraitRatio = 2 / 3;

      DocumentModel.prototype.landscapeRatio = 3 / 2;

      DocumentModel.prototype.initialize = function() {
        this.setDate();
        this.setTruncatedName();
        return this.setOrientation();
      };

      DocumentModel.prototype.setDate = function() {
        var attach_time;
        attach_time = this.get('timestamp');
        if (!attach_time) {
          return false;
        }
        return this.set('dateUri', Moment(attach_time).format('YYYY-MM-DD'));
      };

      DocumentModel.prototype.setTruncatedName = function() {
        return this.set('truncated_name', _.str.truncate(this.get('file_name'), 32));
      };

      DocumentModel.prototype.setOrientation = function() {
        var frameRatio, meta_data, ratio;
        meta_data = this.get('meta_data');
        ratio = meta_data.width / meta_data.height;
        if (ratio < this.baseRatio) {
          this.set('orientation', 'potrait');
          frameRatio = this.potraitRatio;
        } else {
          this.set('orientation', 'landscape');
          frameRatio = this.landscapeRatio;
        }
        if (ratio >= frameRatio) {
          return this.set('fill', 'height');
        } else {
          return this.set('fill', 'width');
        }
      };

      return DocumentModel;

    })(Backbone.Model);
  });

}).call(this);
