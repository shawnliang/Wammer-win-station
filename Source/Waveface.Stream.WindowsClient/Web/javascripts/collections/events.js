(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/event', 'localstorage', 'eventbundler', 'com/subscriber'], function(_, Backbone, Event, LocalStorage, EventBundler, Subscriber) {
    var EventCollection, POST_ADDED, POST_UPDATE;
    POST_ADDED = Subscriber.POST_ADDED;
    POST_UPDATE = Subscriber.POST_UPDATE;
    EventCollection = (function(_super) {

      __extends(EventCollection, _super);

      function EventCollection() {
        return EventCollection.__super__.constructor.apply(this, arguments);
      }

      EventCollection.prototype.model = Event;

      EventCollection.prototype.schemaName = 'posts';

      EventCollection.prototype.dateGroup = [];

      EventCollection.prototype.localStorage = new LocalStorage('events');

      EventCollection.prototype.initialize = function() {
        dispatch.on("store:change:posts:all", this.updatePost, this);
        dispatch.on("subscribe:change:" + POST_ADDED + ":new:event", this.updatePost, this);
        dispatch.on("subscribe:change:" + POST_UPDATE + ":update:event", this.updatePost, this);
        return dispatch.on("more:posts", this.loadMore, this);
      };

      EventCollection.prototype.comparator = function(model) {
        return -moment(model.get("timestamp")).unix();
      };

      EventCollection.prototype.updatePost = function(data, ns) {
        var eventViewState, fragment, lastDate, posts,
          _this = this;
        posts = data.posts;
        lastDate = false;
        if (posts.length <= 0) {
          return false;
        }
        _.each(posts, function(post, index) {
          var postDate;
          postDate = moment(post.timestamp).format('YYYY-MM-DD');
          if (index === 0) {
            lastDate = postDate;
          }
          _this.dateGroup.push(postDate);
          return _this.add(post);
        });
        this.dateGroup = _.uniq(this.dateGroup);
        this.dateGroup.sort();
        this.dateGroup.reverse();
        eventViewState = new LocalStorage('nc_view_state:events');
        if (eventViewState.data.date != null) {
          if (eventViewState.data.id != null) {
            fragment = "events/" + eventViewState.data.date + "/" + eventViewState.data.id;
          } else {
            fragment = "events/" + eventViewState.data.date;
          }
        } else if (ns === "all") {
          fragment = "events/" + lastDate;
        }
        if (fragment != null) {
          if (!window.currentView || window.currentView === "events") {
            return Backbone.history.navigate(fragment, {
              trigger: true
            });
          }
        }
      };

      EventCollection.prototype.loadMore = function() {
        return this.callPosts("more", this.updatePost, this);
      };

      EventCollection.prototype.callPosts = function(namespace, callback, context) {
        var eventStore, memo, pageNo, params, viewData;
        if (namespace == null) {
          namespace = "all";
        }
        new EventBundler('GetPosts', namespace);
        if (!!callback && !!context) {
          dispatch.off("store:change:posts:" + namespace);
          dispatch.on("store:change:posts:" + namespace, callback, context);
        }
        eventStore = new LocalStorage("nc_events_view_data");
        viewData = eventStore.data;
        if (!!viewData.pageNo) {
          pageNo = viewData.pageNo += 1;
        } else {
          pageNo = viewData.pageNo = 1;
        }
        eventStore.data = viewData;
        eventStore.save();
        memo = {
          namespace: namespace,
          type: this.schemaName
        };
        params = {
          page_no: pageNo,
          page_size: 30
        };
        return wfwsocket.sendMessage('getPosts', params, memo);
      };

      EventCollection.prototype.subscribe = function(namespace, callback, context, event_id) {
        var memo, params;
        if (namespace == null) {
          namespace = "new:event";
        }
        if (event_id == null) {
          event_id = Subscriber.POST_ADDED;
        }
        new EventBundler("SubscribeEvent", namespace);
        params = {
          event_id: event_id
        };
        memo = {
          namespace: namespace,
          event_id: event_id
        };
        return wfwsocket.sendMessage("subscribeEvent", params, memo);
      };

      EventCollection.prototype.filterByDate = function(date) {
        if (!date) {
          return this;
        }
        return _(this.filter(function(model) {
          var modelDate;
          modelDate = model.get('dateUri');
          return modelDate === date;
        }));
      };

      EventCollection.prototype.nextDate = function(date) {
        var dateIndex;
        dateIndex = _.indexOf(this.dateGroup, date);
        return this.dateGroup[dateIndex - 1];
      };

      EventCollection.prototype.previousDate = function(date, trigger) {
        var dateIndex;
        if (trigger == null) {
          trigger = true;
        }
        dateIndex = _.indexOf(this.dateGroup, date);
        if (trigger === true) {
          if (!this.dateGroup[dateIndex + 3]) {
            dispatch.trigger("more:posts");
          }
        }
        return this.dateGroup[dateIndex + 1];
      };

      EventCollection.prototype.nextEvent = function(id) {
        var currentIndex;
        currentIndex = this.indexOf(this.get(id));
        return this.at(currentIndex - 1);
      };

      EventCollection.prototype.previousEvent = function(id) {
        var currentIndex;
        currentIndex = this.indexOf(this.get(id));
        return this.at(currentIndex + 1);
      };

      return EventCollection;

    })(Backbone.Collection);
    return new EventCollection();
  });

}).call(this);
