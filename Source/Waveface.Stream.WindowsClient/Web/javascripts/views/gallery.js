(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['backbone', 'mustache', 'mousetrap'], function(Backbone, M, Mousetrap) {
    var GalleryView;
    GalleryView = (function(_super) {

      __extends(GalleryView, _super);

      function GalleryView() {
        return GalleryView.__super__.constructor.apply(this, arguments);
      }

      GalleryView.prototype.initialize = function() {};

      GalleryView.prototype.render = function(dataSource, e, start) {
        var options,
          _this = this;
        if (!(start != null)) {
          start = $(e.currentTarget).index();
        }
        this.history = Backbone.history.fragment;
        options = {
          dataSource: dataSource,
          debug: false,
          height: $('#main').height(),
          show: start,
          extend: function(options) {
            this.addElement('close');
            this.appendChild('container', 'close');
            return this.$('close').html('&times');
          }
        };
        $('body').append('<div id="gallery"></div>');
        $('#gallery').galleria(options).on('click', '.galleria-close', function() {
          return _this.close.call(_this);
        });
        Mousetrap.bind('esc', function() {
          _this.close.call(_this);
          return false;
        });
        return Router.navigate(Backbone.history.fragment + '/gallery');
      };

      GalleryView.prototype.close = function() {
        $('#gallery').remove();
        return Router.navigate(this.history);
      };

      return GalleryView;

    })(Backbone.View);
    return new GalleryView;
  });

}).call(this);
