(function() {

  define(["common"], function(Common) {
    var Logger, root;
    root = window;
    Logger = {};
    /*
            Logger will replace the console method , if the window.console is no exist ,
            if the Logger.sendToServer is true , it'll send log package through socket connection
    */

    Logger = (function() {

      Logger.WARNING = "warn";

      Logger.LOG = "log";

      Logger.ERROR = "error";

      Logger.prototype.origin = window.Logger;

      Logger.prototype.sendToServer = false;

      Logger.prototype.logState = true;

      function Logger() {
        /*
                        Check the console exist. if not , use Server side Logger
        */
        this.setSendToServer(Common.ServerLog);
        this.setLogState(Common.Log);
      }

      Logger.prototype.noconflic = function() {
        root.Logger = this.origin;
        return this;
      };

      Logger.prototype.setConnector = function(connector) {
        this.connector = connector;
      };

      Logger.prototype.setSendToServer = function(sendToServer) {
        this.sendToServer = sendToServer;
      };

      Logger.prototype.setLogState = function(logState) {
        this.logState = logState;
      };

      Logger.prototype.send = function(type, data) {
        var content, tmp;
        if (!this.logState) {
          return;
        }
        if (!this.sendToServer) {
          tmp = eval("console." + type + "(data)");
          return;
        }
        if (!this.connector) {
          return;
        }
        content = {
          command: "setLog",
          params: {
            data: data,
            type: type
          }
        };
        return this.connector.send(JSON.stringify(content));
      };

      Logger.prototype.log = function(object) {
        return this.send(Logger.LOG, object);
      };

      Logger.prototype.warn = function(object) {
        return this.send(Logger.WARNING, object);
      };

      Logger.prototype.error = function(object) {
        return this.send(Logger.ERROR, object);
      };

      return Logger;

    })();
    Logger = root.Logger = new Logger();
    return Logger;
  });

}).call(this);
