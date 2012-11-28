(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

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
          return;
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

      function Bundler() {
        this.bind();
      }

      Bundler.prototype.bind = function() {
        this.clear();
        return Bundler.dispatch.on(this.eventName, this.callback);
      };

      Bundler.prototype.clear = function(event) {
        var eventName;
        eventName = event || this.eventName;
        Logger.log("Clear Dispatch: " + eventName);
        Bundler.dispatch.off(eventName);
        return Logger.log("Event Binded: " + eventName);
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
        nc_user_info = new LocalStorage('nc_user_info');
        nc_user_info.data = data.response;
        nc_user_info.save();
        return dispatch.trigger('user:info:save');
      };

      return GetUserInfo;

    })(Bundler);
    self.GetPosts = (function(_super) {

      __extends(GetPosts, _super);

      /*
              @TODO 
                 Proccess posts list from station
      */


      GetPosts.prototype.eventName = 'getPosts';

      GetPosts.prototype.postTarge = false;

      function GetPosts(postTarget) {
        this.postTarget = postTarget;
        this.callback = __bind(this.callback, this);

        this.bind(this.postTarget);
      }

      GetPosts.prototype.bind = function(postTarget) {
        var eventName;
        eventName = "" + this.eventName + ":" + postTarget;
        this.clear(eventName);
        Bundler.dispatch.on(eventName, this.callback);
        return Logger.log("Bundler event : " + this.eventName + ":" + postTarget + ", callback:" + this.callback);
      };

      GetPosts.prototype.callback = function(data) {
        return Bundler.dispatch.trigger("store:change:posts:" + this.postTarget, data.response, data.memo.namespace);
      };

      return GetPosts;

    })(Bundler);
    self.GetAttachments = (function(_super) {

      __extends(GetAttachments, _super);

      /*
              @TODO
                 Proccess attachments
                 use @attachmentTarget to assign current callback context
      */


      GetAttachments.prototype.eventName = 'getAttachments';

      GetAttachments.prototype.attachmentTarget = false;

      function GetAttachments(attachmentTarget) {
        this.attachmentTarget = attachmentTarget;
        this.callback = __bind(this.callback, this);

        this.bind(this.attachmentTarget);
      }

      GetAttachments.prototype.bind = function(attachmentTarget) {
        var eventName;
        eventName = "" + this.eventName + ":" + attachmentTarget;
        this.clear(eventName);
        Bundler.dispatch.on(eventName, this.callback);
        return Logger.log("Bundler event : " + this.eventName + ":" + attachmentTarget + ", callback:" + this.callback);
      };

      GetAttachments.prototype.callback = function(data) {
        return dispatch.trigger("store:change:attachment:" + this.attachmentTarget, data.response, data.memo.namespace);
      };

      return GetAttachments;

    })(Bundler);
    /*
            Return this file's main class
    */

    return EventBundler;
  });

}).call(this);
