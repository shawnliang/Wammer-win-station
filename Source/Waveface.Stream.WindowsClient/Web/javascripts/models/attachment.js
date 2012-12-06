(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'moment'], function(_, Backbone, Moment) {
    var AttachmentModel;
    return AttachmentModel = (function(_super) {

      __extends(AttachmentModel, _super);

      function AttachmentModel() {
        return AttachmentModel.__super__.constructor.apply(this, arguments);
      }

      AttachmentModel.prototype.initialize = function() {
        this.setOrientation();
        return this.setDate();
      };

      AttachmentModel.prototype.setOrientation = function() {
        var image_height, image_meta, image_width, _ref;
        image_meta = this.get('image_meta');
        if (!image_meta) {
          return false;
        }
        _ref = this.getImageSize(image_meta), image_height = _ref[0], image_width = _ref[1];
        if (image_height > image_width) {
          return this.set('orientation', 'potrait');
        } else {
          return this.set('orientation', 'landscape');
        }
      };

      AttachmentModel.prototype.setDate = function() {
        var attach_time;
        attach_time = this.get('timestamp');
        if (!attach_time) {
          return false;
        }
        return this.set('dateUri', Moment(attach_time).format('YYYY-MM-DD'));
      };

      AttachmentModel.prototype.getImageSize = function(image_meta) {
        if ((image_meta.small != null) && (image_meta.small.height != null) && (image_meta.small.width != null)) {
          return [image_meta.small.height, image_meta.small.width];
        }
        if ((image_meta.middle != null) && (image_meta.middle.height != null) && (image_meta.middle.width != null)) {
          return [image_meta.middle.height, image_meta.middle.width];
        }
        if ((image_meta.large != null) && (image_meta.large.height != null) && (image_meta.large.width != null)) {
          return [image_meta.large.height, image_meta.large.width];
        }
        return [0, 0];
      };

      return AttachmentModel;

    })(Backbone.Model);
  });

}).call(this);
