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

      EventView.prototype.photoViews = [];

      EventView.prototype.render = function() {
        if (!!this.model) {
          this.$el.html(M.render(Template, this.model.toJSON()));
          this.$el.addClass('event-type-' + this.model.get('type'));
          if (!this.model.detailFetchStatus) {
            Attachments.callAttachments([this.model.id], 100, this.model.id, this.renderAttachments, this);
            this.model.detailFetchStatus = true;
            $.unblockUI();
          } else {
            this.renderLocalAttachments(this.model.get("attachment_id_array"));
            $.unblockUI();
          }
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
        _.each(data, function(attachment_data) {
          var attachment, view;
          if (!!(attachment = Attachments.get(attachment_data.id))) {
            view = new AttachmentView({
              model: attachment
            });
            _this.$(".attachments").append(view.render().el);
            return _this.photoViews.push(view);
          }
        });
        this.delegateEvents();
        return $.unblockUI();
      };

      EventView.prototype.renderLocalAttachments = function(_attachments) {
        var _this = this;
        this.photos = _attachments;
        if ((_attachments != null) && _attachments.length <= 0) {
          return false;
        }
        this.$(".attachments").empty();
        _.each(_attachments, function(attachment_id) {
          var attachment, view;
          if (!!(attachment = Attachments.get(attachment_id))) {
            view = new AttachmentView({
              model: attachment
            });
            _this.$(".attachments").append(view.render().el);
            return _this.photoViews.push(view);
          }
        });
        return this.delegateEvents();
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
            var meta_data, photo;
            if (attachment.id != null) {
              photo = Attachments.get(attachment.id);
            } else if (Attachments.get(attachment)) {
              photo = Attachments.get(attachment);
            }
            meta_data = photo.get("meta_data");
            if (meta_data && (meta_data.small.url != null) && (meta_data.medium.url != null)) {
              return {
                thumb: meta_data.small.url,
                image: meta_data.medium.url
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
