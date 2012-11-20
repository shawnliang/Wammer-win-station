(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/attachment', 'collections/attachments', 'lib/galleria/galleria-1.2.8', 'text!templates/photos.html'], function(_, Backbone, M, AttachmentView, Photos, Galleria, Template) {
    var PhotosView;
    PhotosView = (function(_super) {

      __extends(PhotosView, _super);

      function PhotosView() {
        return PhotosView.__super__.constructor.apply(this, arguments);
      }

      PhotosView.prototype.id = 'photos';

      PhotosView.prototype.collection = Photos;

      PhotosView.prototype.events = {
        'click .more': 'loadMore',
        'click .attachment': 'initGallery'
      };

      PhotosView.prototype.initialize = function() {
        this.collection.on('add', this.addOne, this);
        return this.collection.on('reset', this.render, this);
      };

      PhotosView.prototype.render = function(date) {
        var _this = this;
        this.$el.html(M.render(Template));
        this.collection.filterByDate(date).each(function(model) {
          return _this.addOne(model);
        });
        this.delegateEvents();
        return this;
      };

      PhotosView.prototype.addOne = function(model) {
        var photoWidth, view;
        photoWidth = Math.floor(($('#main').width() - 20) * 0.125);
        view = new AttachmentView({
          model: model,
          width: photoWidth,
          height: photoWidth
        });
        return this.$(".grid").append(view.render().el);
      };

      PhotosView.prototype.initGallery = function(e) {
        var data, options;
        data = this.collection.map(function(model) {
          return {
            thumb: model.get('image_meta').small.url,
            image: model.get('image_meta').medium.url
          };
        });
        options = {
          dataSource: data,
          debug: false,
          show: $(e.currentTarget).index(),
          extend: function(options) {
            this.addElement('close');
            this.appendChild('container', 'close');
            return this.$('close').html('&times');
          }
        };
        $('body').append('<div id="gallery"></div>');
        $('#gallery').galleria(options).on('click', '.galleria-close', function(e) {
          return $(e.delegateTarget).remove();
        });
        return Router.navigate('slide');
      };

      PhotosView.prototype.loadMore = function() {
        return this.collection.loadMore();
      };

      return PhotosView;

    })(Backbone.View);
    return new PhotosView({
      collection: Photos
    });
  });

}).call(this);
