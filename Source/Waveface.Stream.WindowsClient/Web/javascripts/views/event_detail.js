(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/attachment', 'views/gallery', 'collections/attachments', 'text!templates/event_detail.html', 'googlemaps'], function(_, Backbone, M, AttachmentView, GalleryView, Attachments, Template) {
    var EventView;
    return EventView = (function(_super) {

      __extends(EventView, _super);

      function EventView() {
        return EventView.__super__.constructor.apply(this, arguments);
      }

      EventView.prototype.className = 'event-detail';

      EventView.prototype.events = {
        'click .attachment': 'startGallery'
      };

      EventView.prototype.render = function() {
        if (!!this.model) {
          this.$el.html(M.render(Template, this.model.toJSON()));
          this.$el.addClass('event-type-' + this.model.get('type'));
          Attachments.callAttachments([this.model.id], 100, this.model.id, this.renderAttachments, this);
          return this;
        } else {
          return false;
        }
      };

      EventView.prototype.renderAttachments = function(data) {
        var photoWidth,
          _this = this;
        this.photos = data;
        photoWidth = Math.floor(this.$('.attachments').width() * 0.20);
        _.each(data, function(attachment_data) {
          var attachment, view;
          if (!!(attachment = Attachments.get(attachment_data.id))) {
            view = new AttachmentView({
              model: attachment,
              width: photoWidth,
              height: photoWidth
            });
            return _this.$(".attachments").append(view.render().el);
          }
        });
        if (this.model.get('hasMap') === true) {
          return this.renderMap();
        } else {
          return this.$('#map_canvas').remove();
        }
      };

      EventView.prototype.renderMap = function() {
        var gps, latlng, map, mapOptions, marker;
        gps = this.model.get('gps');
        latlng = new google.maps.LatLng(gps.latitude, gps.longitude);
        mapOptions = {
          center: latlng,
          zoom: gps.zoom_level,
          mapTypeId: google.maps.MapTypeId.ROADMAP,
          scrollwheel: false
        };
        map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
        return marker = new google.maps.Marker({
          position: latlng,
          map: map
        });
      };

      EventView.prototype.loadMore = function() {
        return this.collection.loadMore();
      };

      EventView.prototype.startGallery = function(e) {
        var data;
        if (!!this.photos) {
          data = _(this.photos).map(function(attachment) {
            if ((attachment.image_meta.small.url != null) && (attachment.image_meta.medium.url != null)) {
              return {
                thumb: attachment['image_meta'].small.url,
                image: attachment['image_meta'].medium.url
              };
            }
          });
          return GalleryView.render(data, e);
        } else {
          return false;
        }
      };

      return EventView;

    })(Backbone.View);
  });

}).call(this);
