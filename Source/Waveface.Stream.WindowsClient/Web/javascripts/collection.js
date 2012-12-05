(function() {

  require(["config"], function(main) {
    return require(["jquery", "connector", "common", "bootstrap"], function($, Connector, Common) {
      return $("#send_data").on("click", function() {
        return Connector.send(JSON.stringify({
          "getSessionToken": {}
        }));
      });
    });
  });

}).call(this);
