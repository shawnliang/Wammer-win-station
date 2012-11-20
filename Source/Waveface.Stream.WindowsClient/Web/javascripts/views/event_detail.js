(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/attachment', 'collections/attachments', 'text!templates/event_detail.html'], function(_, Backbone, M, attachmentView, Attachments, Template) {
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

      EventView.prototype.initialize = function() {};

      EventView.prototype.render = function() {
        if (!!this.model) {
          this.$el.addClass('event-type-' + this.model.get('type'));
          this.$el.html(M.render(Template, this.model.toJSON()));
          Attachments.callAttachments([this.model.id], 100, this.model.id, this.renderAttachments, this);
          return this;
        }
        return false;
      };

      EventView.prototype.renderAttachments = function(data) {
        var photoWidth,
          _this = this;
        photoWidth = Math.floor(($('#event-detail').width() - 40 + 20) * 0.20);
        if (!!data) {
          Attachments.updateAttachment(data);
        }
        dispatch.off("store:change:attachment:" + this.model.id);
        return _.each(this.model.get('attachment_id_array'), function(attachmentId) {
          var view;
          if (!!Attachments.get(attachmentId)) {
            view = new attachmentView({
              model: Attachments.get(attachmentId),
              width: photoWidth,
              height: photoWidth
            });
            return _this.$(".attachments").append(view.render().el);
          }
        });
      };

      EventView.prototype.startGallery = function(e) {
        var data, options;
        data = _(this.model.get('summary_attachments')).map(function(attachment) {
          return {
            thumb: attachment['image_meta'].small.url,
            image: attachment['image_meta'].medium.url
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

      return EventView;

    })(Backbone.View);
  });

}).call(this);
