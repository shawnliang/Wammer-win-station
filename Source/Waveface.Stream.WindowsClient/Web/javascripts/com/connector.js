(function() {

  define(['eventbundler', 'backbone'], function(EventBundler, Backbone) {
    var Connector;
    Connector = (function() {

      function Connector() {}

      /*
                  Setting connector enviroment in connector initialize.
      */


      Connector.sendMessage = function(command, data) {
        return wfwsocket.sendMessage(command, data);
      };

      Connector.end = function() {
        return Logger.log('Connector end');
      };

      return Connector;

    }).call(this);
    return Connector;
  });

}).call(this);
