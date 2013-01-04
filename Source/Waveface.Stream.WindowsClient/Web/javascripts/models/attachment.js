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
        this.set("image_meta", this.get("meta_data"));
        if ((this.get("image_meta")).small_previews != null) {
          this.transformSchema();
        }
        this.setOrientation();
        return this.setDate();
      };

      AttachmentModel.prototype.transformSchema = function() {
        var originData, result;
        originData = this.get("image_meta");
        originData.small = _(originData.small_previews).first();
        originData.medium = _(originData.medium_previews).first() || _(originData.small_previews).first();
        originData.large = _(originData.large_previews).first() || _(originData.medium_previews).first();
        return this.set("image_meta", result = originData);
      };

      AttachmentModel.prototype.setOrientation = function(baseRatio) {
        var image_height, image_meta, image_width, ratio, _ref;
        if (baseRatio == null) {
          baseRatio = 1;
        }
        image_meta = this.get("image_meta");
        if (!image_meta) {
          return false;
        }
        _ref = this.getImageSize(image_meta), image_height = _ref[0], image_width = _ref[1];
        ratio = image_width / image_height;
        this.set('ratio', ratio);
        if (ratio < baseRatio) {
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
