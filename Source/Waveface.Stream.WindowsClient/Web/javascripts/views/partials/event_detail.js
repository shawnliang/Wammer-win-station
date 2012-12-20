(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/partials/attachment', 'views/gallery', 'collections/attachments', 'text!templates/partials/event_detail.html', 'googlemaps'], function(_, Backbone, M, AttachmentView, GalleryView, Attachments, Template) {
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
          this.photoViews = [];
          this.renderLocalAttachments(this.model.get("attachment_id_array"));
          return this;
        } else {
          return false;
        }
      };

      EventView.prototype.renderAttachments = function(data, test) {
        var _this = this;
        this.photos = data;
        this.$(".attachments").empty();
        return _.each(data, function(attachment_data) {
          var attachment, view;
          if (!!(attachment = Attachments.get(attachment_data.id))) {
            view = new AttachmentView({
              model: attachment
            });
            _this.$(".attachments").append(view.render().el);
            return _this.photoViews.push(view);
          }
        });
      };

      EventView.prototype.renderLocalAttachments = function(_attachmments) {
        if (_attachmments.length <= 0) {
          return false;
        }
        this.$(".attachments").empty();
        return _.each(_attachmments, function(attachment_id) {
          var attachment, view;
          if (!!(attachment = Attachments.get(attachment_id))) {
            view = new AttachmentView({
              model: attachment
            });
            return this.$(".attachments").append(view.render().el);
          }
        });
      };

      EventView.prototype.renderMap = function() {
        var el, gps, latlng, mapOptions, marker;
        if (this.model.get('hasMap') === true && navigator.onLine === true) {
          el = this.$('#map_canvas').get(0);
          gps = this.model.get('gps');
          latlng = new google.maps.LatLng(gps.latitude, gps.longitude);
          mapOptions = {
            center: latlng,
            zoom: gps.zoom_level,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            scrollwheel: false
          };
          this.map = new google.maps.Map(el, mapOptions);
          return marker = new google.maps.Marker({
            position: latlng,
            map: this.map
          });
        }
      };

      EventView.prototype.loadMore = function() {
        return this.collection.loadMore();
      };

      EventView.prototype.startGallery = function(e) {
        var data;
        if (this.photos != null) {
          data = _(this.photos).map(function(attachment) {
            if ((attachment.meta_data.small.url != null) && (attachment.meta_data.medium.url != null)) {
              return {
                thumb: attachment['meta_data'].small.url,
                image: attachment['meta_data'].medium.url
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
