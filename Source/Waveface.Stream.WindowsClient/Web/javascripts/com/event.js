(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; },
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  define(['jquery', 'underscore', 'backbone', 'localstorage'], function($, _, Backbone, LocalStorage) {
    /*
            Setting Backbone events dispatcher.
            It's a Global event dispatch
    */

    var Bundler, EventBundler, FetchPostByFilter, GetAttachments, GetPosts, GetSessionToken, WebSocketOpen, that;
    window.dispatch = _.extend(Backbone.Events);
    that = this;
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
          Logger.log("event bind:" + events);
          eventInstance = new that.bundleClass[events](args);
          return;
        }
        _.each(events, function(event) {
          Logger.log("event bind:" + event + "(" + args + ")");
          return eventInstance = new that.bundleClass[event](args);
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
        return Bundler.dispatch.on(this.eventName, this.callback);
      };

      return Bundler;

    })();
    WebSocketOpen = (function(_super) {

      __extends(WebSocketOpen, _super);

      function WebSocketOpen() {
        return WebSocketOpen.__super__.constructor.apply(this, arguments);
      }

      WebSocketOpen.prototype.eventName = 'WebSocketOpen';

      WebSocketOpen.prototype.callback = function(data) {
        return Logger.log("" + (new Date()) + " Web Socket Connection is open in " + (data.getUri()) + " from " + (data.getName()));
      };

      return WebSocketOpen;

    })(Bundler);
    GetSessionToken = (function(_super) {

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
    FetchPostByFilter = (function(_super) {

      __extends(FetchPostByFilter, _super);

      function FetchPostByFilter() {
        return FetchPostByFilter.__super__.constructor.apply(this, arguments);
      }

      FetchPostByFilter.prototype.eventName = '/v2/posts/fetchByFilter';

      FetchPostByFilter.prototype.callback = function(data) {
        return EventBundler.trigger('store:change:posts', data.response);
      };

      return FetchPostByFilter;

    })(Bundler);
    GetPosts = (function(_super) {

      __extends(GetPosts, _super);

      function GetPosts() {
        return GetPosts.__super__.constructor.apply(this, arguments);
      }

      /*
              @TODO 
                 Proccess posts list from station
      */


      GetPosts.prototype.eventName = 'getPosts';

      GetPosts.prototype.callback = function(data) {
        return EventBundler.trigger('store:change:posts', data.response);
      };

      return GetPosts;

    })(Bundler);
    GetAttachments = (function(_super) {

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
        Bundler.dispatch.on("" + this.eventName + ":" + attachmentTarget, this.callback);
        return Logger.log("Bundler event : " + this.eventName + ":" + attachmentTarget + ", callback:" + this.callback);
      };

      GetAttachments.prototype.callback = function(data) {
        return EventBundler.trigger("store:change:attachment:" + this.attachmentTarget, data.response);
      };

      return GetAttachments;

    })(Bundler);
    that.bundleClass = {
      "GetSessionToken": GetSessionToken,
      "GetPosts": GetPosts,
      "WebSocketOpen": WebSocketOpen,
      "FetchPostByFilter": FetchPostByFilter,
      "GetAttachments": GetAttachments
    };
    /*
            Return this file's main class
    */

    return EventBundler;
  });

}).call(this);
