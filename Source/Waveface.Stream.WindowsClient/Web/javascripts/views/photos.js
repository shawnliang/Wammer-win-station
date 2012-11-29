(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/attachment', 'views/gallery', 'collections/attachments', 'lib/galleria/galleria-1.2.8', 'text!templates/photos.html'], function(_, Backbone, M, AttachmentView, GalleryView, Attachments, Galleria, Template) {
    var PhotosView;
    PhotosView = (function(_super) {

      __extends(PhotosView, _super);

      function PhotosView() {
        return PhotosView.__super__.constructor.apply(this, arguments);
      }

      PhotosView.prototype.id = 'photos';

      PhotosView.prototype.tagName = 'section';

      PhotosView.prototype.events = {
        'click .more': 'loadMore',
        'click .attachment': 'initGallery'
      };

      PhotosView.prototype.initialize = function() {
        return dispatch.on("render:change:attachment:all", this.render, this);
      };

      PhotosView.prototype.render = function(date) {
        var data,
          _this = this;
        data = {
          date: moment(date).format('LL')
        };
        this.$el.html(M.render(Template, data));
        this.collection.filterByDate(date).each(function(model) {
          return _this.addOne(model);
        });
        this.delegateEvents();
        return this;
      };

      PhotosView.prototype.addOne = function(model) {
        var photoWidth, view;
        photoWidth = Math.floor(($('#main').width() - 60) * 0.125);
        view = new AttachmentView({
          model: model,
          width: photoWidth,
          height: photoWidth
        });
        return this.$(".grid").append(view.render().el);
      };

      PhotosView.prototype.loadMore = function() {
        return this.collection.loadMore();
      };

      PhotosView.prototype.initGallery = function(e) {
        var data;
        if (!!this.collection) {
          data = this.collection.map(function(model) {
            return {
              thumb: model.get('image_meta').small.url,
              image: model.get('image_meta').medium.url
            };
          });
          return GalleryView.render(data, e);
        } else {
          return false;
        }
      };

      return PhotosView;

    })(Backbone.View);
    return new PhotosView({
      collection: Attachments
    });
  });

}).call(this);
