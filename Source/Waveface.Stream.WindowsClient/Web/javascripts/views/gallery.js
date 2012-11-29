(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['backbone', 'mustache'], function(Backbone, M, Template, Events, User) {
    var GalleryView;
    GalleryView = (function(_super) {

      __extends(GalleryView, _super);

      function GalleryView() {
        return GalleryView.__super__.constructor.apply(this, arguments);
      }

      GalleryView.prototype.initialize = function() {};

      GalleryView.prototype.render = function(dataSource, e) {
        var options,
          _this = this;
        this.history = Backbone.history.fragment;
        options = {
          dataSource: dataSource,
          debug: false,
          height: $('#main').height(),
          show: $(e.currentTarget).index(),
          extend: function(options) {
            this.addElement('close');
            this.appendChild('container', 'close');
            return this.$('close').html('&times');
          }
        };
        $('body').append('<div id="gallery"></div>');
        $('#gallery').galleria(options).on('click', '.galleria-close', function(e) {
          $(e.delegateTarget).remove();
          return Router.navigate(_this.history);
        });
        return Router.navigate(Backbone.history.fragment + '/gallery');
      };

      return GalleryView;

    })(Backbone.View);
    return new GalleryView;
  });

}).call(this);
