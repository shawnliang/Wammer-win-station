(function() {

  define(['common', 'eventbundler'], function(Common, EventBundler) {
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

      WfWSocket.wantTrigger = true;

      WfWSocket.wantShowReceivedData = false;

      WfWSocket.inSandbox = false;

      WfWSocket.status = false;

      WfWSocket.init = function(onopen) {
        if (root.connection) {
          console.log(root.connection);
          WfWSocket.connection = root.connection;
          return WfWSocket;
        }
        new EventBundler(['WebSocketOpen', 'GetUserInfo']);
        root.connection = WfWSocket.connection = new WebSocket("ws://" + WfWSocket.URI + ":" + WfWSocket.PORT + "/");
        WfWSocket.connection.onopen = function() {
          WfWSocket.handleOpen(Common.Env);
          if (typeof onopen === 'function') {
            return onopen();
          }
        };
        WfWSocket.connection.onclose = WfWSocket.end;
        WfWSocket.connection.onmessage = WfWSocket.handleMessage;
        return WfWSocket;
      };

      WfWSocket.handleOpen = function(env) {
        return EventBundler.trigger('WebSocketOpen', env);
      };

      /*
              # WfWSocket.handleMessage( MessageEvent message )
              # When websocket receive message , this method will trigger the resp command's
              # event. If the {@statSandbox} is yes(and true or on) command will not to
              # trigger anythings, and the console'll log MessageEvent's {data}
      */


      WfWSocket.handleMessage = function(message) {
        var data, triggerStr;
        if (WfWSocket.wantShowReceivedData || WfWSocket.inSandbox) {
          Logger.log("ws received message: " + message.data);
        }
        data = JSON.parse(message.data);
        Logger.log("ws received command: " + data.command);
        if (!!data.memo) {
          Logger.log("ws received memo: " + (JSON.stringify(data.memo)));
        }
        if (data.memo != null) {
          triggerStr = [data.command];
          if (!!data.memo.namespace) {
            triggerStr.push(":" + data.memo.namespace);
          }
          if (!!data.memo.page) {
            triggerStr.push(":" + data.memo.page);
          }
          triggerStr = triggerStr.join("");
          if (WfWSocket.wantTrigger && !WfWSocket.inSandbox) {
            return EventBundler.trigger(triggerStr, data);
          }
        } else {
          if (WfWSocket.wantTrigger && !WfWSocket.inSandbox) {
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
        Logger.log('Connector end');
        return this.status = {
          code: 0,
          msg: "offline"
        };
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
      root.wfwsocket = previousConnector;
      return this;
    };
    return wfwsocket = root.wfwsocket = WfWSocket;
  });

}).call(this);
