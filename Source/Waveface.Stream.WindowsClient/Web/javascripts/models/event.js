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

      EventModel.prototype.detailFetchStatus = false;

      EventModel.prototype.initialize = function() {
        this.setDate();
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

      EventModel.prototype.setCover = function() {
        var cover, summary_attachments;
        summary_attachments = this.get("summary_attachments");
        if (!(summary_attachments != null)) {
          return false;
        }
        cover = _(summary_attachments).first();
        if (!(cover != null)) {
          return false;
        }
        cover.image_meta = cover.meta_data;
        return this.set('cover', cover);
      };

      EventModel.prototype.setSwitch = function() {
        this.set('hasDescription', !!this.get('event_description'));
        this.set('hasPeople', !!this.get('people'));
        this.set('hasTags', !!this.get('tags'));
        if (this.get('attachment_count') > 0) {
          this.set('hasPhoto', true);
        }
        if (this.get('location')) {
          this.set('gps', this.get('location'));
        }
        if (!!this.get('gps') && this.get('gps').latitude !== 0.0 && this.get('gps').longitude !== 0.0) {
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
