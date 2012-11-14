(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'views/attachment', 'models/attachment', 'text!templates/event_detail.html', 'lib/jquery/jquery.colorbox'], function(_, Backbone, M, attachmentView, attachmentModel, template) {
    var EventView;
    return EventView = (function(_super) {

      __extends(EventView, _super);

      function EventView() {
        return EventView.__super__.constructor.apply(this, arguments);
      }

      EventView.prototype.className = 'event-detail';

      EventView.prototype.initialize = function() {};

      EventView.prototype.render = function() {
        this.$el.addClass('event-type-' + this.model.get('type'));
        this.$el.html(M.render(template, this.model.toJSON()));
        this.renderAttachments();
        return this;
      };

      EventView.prototype.renderAttachments = function() {
        var _this = this;
        _.each(this.model.get('attachments'), function(attachment) {
          var view;
          view = new attachmentView({
            model: new attachmentModel(attachment)
          });
          return _this.$(".attachments").append(view.render().el);
        });
        return this.$('.attachment a').colorbox({
          rel: 'gallery',
          photo: true
        });
      };

      return EventView;

    })(Backbone.View);
  });

}).call(this);
