(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone'], function(_, Backbone) {
    var AttachmentModel;
    return AttachmentModel = (function(_super) {

      __extends(AttachmentModel, _super);

      function AttachmentModel() {
        return AttachmentModel.__super__.constructor.apply(this, arguments);
      }

      AttachmentModel.prototype.initialize = function() {
        return this.setOrientation();
      };

      AttachmentModel.prototype.setOrientation = function() {
        var image_meta;
        image_meta = this.get('image_meta');
        if (!image_meta) {
          return false;
        }
        if (image_meta.height > image_meta.width) {
          return this.set('orientation', 'potrait');
        } else {
          return this.set('orientation', 'landscape');
        }
      };

      return AttachmentModel;

    })(Backbone.Model);
  });

}).call(this);
