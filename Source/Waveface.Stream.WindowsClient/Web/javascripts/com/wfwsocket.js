(function() {

  define(['common', 'eventbundler', 'views/app'], function(Common, EventBundler, AppView) {
    /*
        Registry wfwsocket to global window
    */

    var WfWSocket, previousConnector, root, wfwsocket;
    root = window;
    previousConnector = root.wfwsocket;
    WfWSocket = (function() {

      function WfWSocket() {}

      WfWSocket.URI = '127.0.0.1';

      WfWSocket.PORT = '1337';

      WfWSocket.connection = false;

      WfWSocket.statTrigger = true;

      WfWSocket.statShowReceivedData = false;

      WfWSocket.statSandbox = false;

      WfWSocket.init = function() {
        new EventBundler(['GetSessionToken', 'WebSocketOpen'], null);
        WfWSocket.connection = new WebSocket("ws://" + WfWSocket.URI + ":" + WfWSocket.PORT + "/");
        WfWSocket.connection.onopen = function() {
          return WfWSocket.handleOpen(Common.Env);
        };
        WfWSocket.connection.onolose = WfWSocket.end;
        WfWSocket.connection.onmessage = WfWSocket.handleMessage;
      };

      WfWSocket.handleOpen = function(env) {
        EventBundler.trigger('WebSocketOpen', env);
        return new AppView();
      };

      /*
              # WfWSocket.handleMessage( MessageEvent message )
              # When websocket receive message , this method will trigger the resp command's
              # event. If the {@statSandbox} is yes(and true or on) command will not to
              # trigger anythings, and the console'll log MessageEvent's {data}
      */


      WfWSocket.handleMessage = function(message) {
        var data, triggerStr;
        if (WfWSocket.statShowReceivedData || WfWSocket.statSandbox) {
          Logger.log("ws received message: " + message.data);
        }
        data = JSON.parse(message.data);
        Logger.log("ws received command: " + data.command);
        if (!!data.memo) {
          Logger.log("ws received memo: " + (JSON.stringify(data.memo)));
        }
        if (!!data.memo) {
          triggerStr = [data.command];
          if (!!data.memo.namespace) {
            triggerStr.push(":" + data.memo.namespace);
          }
          if (!!data.memo.page) {
            triggerStr.push(":" + data.memo.page);
          }
          triggerStr = triggerStr.join("");
          Logger.log("Event trigger : " + triggerStr);
          if (WfWSocket.statTrigger && !WfWSocket.statSandbox) {
            return EventBundler.trigger(triggerStr, data);
          }
        } else {
          if (WfWSocket.statTrigger && !WfWSocket.statSandbox) {
            return EventBundler.trigger(data.command, data);
          }
        }
      };

      WfWSocket.handleError = function(error) {
        return Logger.log("Something is wrong: " + error);
      };

      WfWSocket.sendMessage = function(command, data, memo) {
        var sendData, sendJSON;
        if (memo == null) {
          memo = false;
        }
        sendData = {
          command: command,
          params: data,
          memo: memo
        };
        sendJSON = JSON.stringify(sendData);
        Logger.log("send msg is " + sendJSON);
        return WfWSocket.connection.send(sendJSON);
      };

      WfWSocket.end = function() {
        return Logger.log('Connector end');
      };

      return WfWSocket;

    }).call(this);
    /*
            TODO:Override Backbone.sync to use websocket
    */

    Backbone.sync = function(method, model, options) {
      return options.success(true);
    };
    /*
        # WfWSocket.noconflic()
        # Recovery the origin window.wfwsocket
        # And return self.
    */

    WfWSocket.noconflic = function() {
      root.wfwocket = previousConnector;
      return this;
    };
    wfwsocket = root.wfwsocket = WfWSocket;
    wfwsocket.init();
  });

}).call(this);
