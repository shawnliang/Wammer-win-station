(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'text!templates/photos.html', 'text!templates/photos/grid_LLL.html', 'text!templates/photos/grid_LPP.html', 'text!templates/photos/grid_PLP.html', 'text!templates/photos/grid_PPL.html', 'text!templates/photos/grid_LLPP.html', 'text!templates/photos/grid_PPLL.html', 'text!templates/photos/grid_6S.html', 'text!templates/photos/grid_2L4S.html', 'text!templates/photos/grid_4S2L.html'], function(_, Backbone, M, Template, GridLLLTemplate, GridLPPTemplate, GridPLPTemplate, GridPPLTemplate, GridLLPPTemplate, GridPPLLTemplate, Grid6STemplate, Grid2L4STemplate, Grid4S2LTemplate) {
    var PhotoGridView;
    return PhotoGridView = (function(_super) {

      __extends(PhotoGridView, _super);

      function PhotoGridView() {
        return PhotoGridView.__super__.constructor.apply(this, arguments);
      }

      PhotoGridView.prototype.initialize = function() {};

      PhotoGridView.prototype.render = function(photoData) {
        var $grid_el, coin, gridSize, gridTemplate, html, templateData;
        this.photoData = photoData;
        if (photoData.size() < 1) {
          return false;
        }
        coin = _.random(1, 3);
        if (coin !== 1 && photoData.size() > 6) {
          gridSize = _.random(3, 6);
          gridTemplate = this.photoTemplateByOrientation(photoData, gridSize);
        } else {
          gridSize = 6;
          gridTemplate = Grid6STemplate;
        }
        if (gridTemplate) {
          templateData = photoData.first(gridSize).map(function(model) {
            return model.toJSON();
          });
          html = M.render(gridTemplate, {
            photos: templateData
          });
          $grid_el = $(html).appendTo(this.el);
          $grid_el.find('.frame').each(function(index, el) {
            if (templateData[index] != null) {
              if (templateData[index].ratio > $(this).data('ratio')) {
                return $(this).addClass('fill-height');
              } else {
                return $(this).addClass('fill-width');
              }
            }
          });
          this.photoData = _(photoData.rest(gridSize));
          return this;
        } else {
          return false;
        }
      };

      PhotoGridView.prototype.photoTemplateByOrientation = function(photoData, gridSize) {
        var coin, orientations;
        orientations = photoData.first(gridSize).map(function(model) {
          return model.get('orientation');
        }).join(' ');
        switch (gridSize) {
          case 3:
            if (orientations === 'landscape landscape landscape') {
              return GridLLLTemplate;
            }
            if (orientations === 'landscape portrait portrait') {
              return GridLPPTemplate;
            }
            if (orientations === 'portrait landscape portrait') {
              return GridPLPTemplate;
            }
            if (orientations === 'portrait portrait landscape') {
              return GridPPLTemplate;
            }
            break;
          case 4:
            coin = _.random(1, 2);
            if (coin === 1) {
              return GridPPLLTemplate;
            } else {
              return GridLLPPTemplate;
            }
            break;
          case 6:
            coin = _.random(1, 2);
            if (coin === 1) {
              return Grid2L4STemplate;
            } else {
              return Grid4S2LTemplate;
            }
            break;
          default:
            return false;
        }
        return false;
      };

      PhotoGridView.prototype.tweakPhotoOrientation = function(photoData, baseRatio) {
        var photo, _i, _len;
        if (baseRatio == null) {
          baseRatio = 1;
        }
        if (photoData == null) {
          return false;
        }
        for (_i = 0, _len = photoData.length; _i < _len; _i++) {
          photo = photoData[_i];
          photo.setOrientation(baseRatio);
        }
        return photoData;
      };

      return PhotoGridView;

    })(Backbone.View);
  });

}).call(this);
