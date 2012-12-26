(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['jquery', 'underscore', 'backbone', 'localstorage'], function($, _, Backbone, LocalStorage) {
    /*
            Setting Backbone events dispatcher.
            It's a Global event dispatch
    */

    var Bundler, EventBundler, self;
    window.dispatch = _.extend(Backbone.Events);
    self = {};
    EventBundler = (function() {

      EventBundler.dispatch = window.dispatch;

      EventBundler.container = EventBundler.dispatch;

      function EventBundler(events, args) {
        if (args == null) {
          args = false;
        }
        this.register(events, args);
      }

      EventBundler.bind = function(eventname, callback) {
        return EventBundler.container.on(eventname, callback);
      };

      EventBundler.trigger = function(eventname, data) {
        return EventBundler.container.trigger(eventname, data);
      };

      EventBundler.prototype.register = function(events, args) {
        var eventInstance;
        if (args == null) {
          args = false;
        }
        if (!Array.isArray(events)) {
          eventInstance = new self[events](args);
          return true;
        }
        _.each(events, function(event) {
          return eventInstance = new self[event](args);
        });
        return true;
      };

      return EventBundler;

    }).call(this);
    Bundler = (function() {

      Bundler.dispatch = window.dispatch;

      function Bundler(target) {
        this.bind(target);
      }

      Bundler.prototype.bind = function(target) {
        if (target) {
          this.eventName = "" + this.eventName + ":" + target;
        }
        this.clear();
        return Bundler.dispatch.on(this.eventName, this.callback);
      };

      Bundler.prototype.clear = function(event) {
        var eventName;
        eventName = event || this.eventName;
        Logger.log("Clear Dispatch: " + eventName);
        Bundler.dispatch.off(eventName);
        return Logger.log("Bundler event : " + eventName + ", callback:" + this.callback);
      };

      Bundler.prototype.callback = function(data, callback, context) {
        if (callback == null) {
          callback = null;
        }
        if (context == null) {
          context = null;
        }
        Bundler.dispatch.trigger("store:change:" + data.memo.type + ":" + data.memo.namespace, data.response, data.memo.namespace);
        if (callback != null) {
          context = context || this;
          return callback.call(context);
        }
      };

      return Bundler;

    })();
    self.WebSocketOpen = (function(_super) {

      __extends(WebSocketOpen, _super);

      function WebSocketOpen() {
        return WebSocketOpen.__super__.constructor.apply(this, arguments);
      }

      WebSocketOpen.prototype.eventName = 'WebSocketOpen';

      WebSocketOpen.prototype.callback = function(data) {
        Logger.log("" + (new Date()) + " Web Socket Connection is open in " + (data.getUri()) + " from " + (data.getName()));
        wfwsocket.status = {
          code: 1,
          msg: "online"
        };
        Bundler.dispatch.trigger("socket:open");
        return wfwsocket.sendMessage("getUserInfo");
      };

      return WebSocketOpen;

    })(Bundler);
    self.GetSessionToken = (function(_super) {

      __extends(GetSessionToken, _super);

      function GetSessionToken() {
        return GetSessionToken.__super__.constructor.apply(this, arguments);
      }

      GetSessionToken.prototype.eventName = 'getSessionToken';

      GetSessionToken.prototype.callback = function(data) {
        var nc_session_token;
        nc_session_token = new LocalStorage('nc_session_token');
        nc_session_token.data = data.response.sessionToken;
        return nc_session_token.save();
      };

      return GetSessionToken;

    })(Bundler);
    self.GetUserInfo = (function(_super) {

      __extends(GetUserInfo, _super);

      function GetUserInfo() {
        return GetUserInfo.__super__.constructor.apply(this, arguments);
      }

      GetUserInfo.prototype.eventName = 'getUserInfo';

      GetUserInfo.prototype.callback = function(data) {
        var nc_user_info;
        console.log("callback getUserInfo");
        nc_user_info = new LocalStorage('nc_user_info');
        nc_user_info.data = data.response;
        nc_user_info.save();
        return Bundler.dispatch.trigger("user:info:save", data);
      };

      return GetUserInfo;

    })(Bundler);
    self.GetPosts = (function(_super) {

      __extends(GetPosts, _super);

      function GetPosts() {
        return GetPosts.__super__.constructor.apply(this, arguments);
      }

      GetPosts.prototype.eventName = 'getPosts';

      return GetPosts;

    })(Bundler);
    self.GetAttachments = (function(_super) {

      __extends(GetAttachments, _super);

      function GetAttachments() {
        return GetAttachments.__super__.constructor.apply(this, arguments);
      }

      GetAttachments.prototype.eventName = 'getAttachments';

      return GetAttachments;

    })(Bundler);
    self.GetCollections = (function(_super) {

      __extends(GetCollections, _super);

      function GetCollections() {
        return GetCollections.__super__.constructor.apply(this, arguments);
      }

      GetCollections.prototype.eventName = 'getCollections';

      return GetCollections;

    })(Bundler);
    self.GetCalendar = (function(_super) {

      __extends(GetCalendar, _super);

      function GetCalendar() {
        return GetCalendar.__super__.constructor.apply(this, arguments);
      }

      GetCalendar.prototype.eventName = 'getCalendarEntries';

      return GetCalendar;

    })(Bundler);
    self.SubscribeEvent = (function(_super) {

      __extends(SubscribeEvent, _super);

      SubscribeEvent.prototype.eventName = 'subscribeEvent';

      function SubscribeEvent(target) {
        this.target = target;
        this.bind(this.target);
      }

      SubscribeEvent.prototype.bind = function(target) {
        var eventName;
        eventName = "" + this.eventName + ":" + target;
        this.clear(eventName);
        return Bundler.dispatch.on(eventName, this.callback, this);
      };

      SubscribeEvent.prototype.callback = function(data) {
        var event_id;
        if (data.memo.event_id && (event_id = data.memo.event_id)) {
          return Bundler.dispatch.trigger("subscribe:change:" + event_id + ":" + this.target, data.response, data.memo.namespace);
        } else {
          return Bundler.dispatch.trigger("subscribe:change:" + this.target, data.response, data.memo.namespace);
        }
      };

      return SubscribeEvent;

    })(Bundler);
    return EventBundler;
  });

}).call(this);
