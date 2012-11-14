(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/event', 'connector', 'localstorage'], function(_, Backbone, Event, LocalStorage) {
    var EventCollection;
    EventCollection = (function(_super) {

      __extends(EventCollection, _super);

      function EventCollection() {
        return EventCollection.__super__.constructor.apply(this, arguments);
      }

      EventCollection.prototype.model = Event;

      EventCollection.prototype.schemaName = 'posts';

      EventCollection.prototype.localStorage = new LocalStorage('events');

      EventCollection.prototype.initialize = function() {
        return dispatch.on("store:change:posts", this.updatePost, this);
      };

      EventCollection.prototype.updatePost = function(data) {
        var posts,
          _this = this;
        posts = data.posts;
        return _.each(posts, function(post) {
          post.id = post.object_id;
          return _this.add(post);
        });
      };

      EventCollection.prototype.callPosts = function() {
        return wfwsocket.sendMessage('getPosts');
      };

      return EventCollection;

    })(Backbone.Collection);
    return new EventCollection();
  });

}).call(this);
