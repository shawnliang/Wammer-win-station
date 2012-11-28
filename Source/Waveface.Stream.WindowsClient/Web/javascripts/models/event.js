(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'moment'], function(_, Backbone, Moment) {
    var EventModel;
    return EventModel = (function(_super) {

      __extends(EventModel, _super);

      function EventModel() {
        return EventModel.__super__.constructor.apply(this, arguments);
      }

      EventModel.prototype.initialize = function() {
        this.setDate();
        this.setAttachmentSize();
        this.setCover();
        this.setSwitch();
        return this;
      };

      EventModel.prototype.setDate = function() {
        var event_time;
        event_time = this.get('timestamp');
        if (!event_time) {
          return false;
        }
        this.set('date', Moment(event_time).format('M/D hh:mm a'));
        this.set('time', Moment(event_time).format('ddd hh:mm a'));
        return this.set('dateUri', Moment(event_time).format('YYYY-MM-DD'));
      };

      EventModel.prototype.setAttachmentSize = function() {
        return this.set('attachmentSize', this.get('attachment_id_array').length);
      };

      EventModel.prototype.setCover = function() {
        return this.set('cover', _(this.get('summary_attachments')).first());
      };

      EventModel.prototype.setSwitch = function() {
        this.set('hasDescription', !!this.get('event_description'));
        this.set('hasPeople', !!this.get('people'));
        if (!!this.get('gps')) {
          this.set('hasLocation', !!this.get('gps').name);
          return this.set('hasMap', !!this.get('gps').zoom_level);
        } else {
          this.set('hasLocation', false);
          return this.set('hasMap', false);
        }
      };

      return EventModel;

    })(Backbone.Model);
  });

}).call(this);
