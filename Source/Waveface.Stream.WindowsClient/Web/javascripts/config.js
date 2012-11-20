(function() {

  require.config({
    shim: {
      underscore: {
        exports: "_"
      },
      jquery: {
        exports: "$"
      },
      backbone: {
        deps: ["underscore", "amplifystore", "jquery"],
        exports: "Backbone"
      },
      bootstrap: {
        deps: ["jquery"]
      },
      'lib/jquery/fullcalendar': ["jquery"],
      'lib/jquery/jquery.colorbox': ["jquery"],
      'lib/galleria/galleria-1.2.8': {
        deps: ["jquery"],
        exports: "Galleria"
      },
      logger: {
        deps: ["underscore", "jquery"]
      }
    },
    paths: {
      jquery: "lib/jquery/jquery",
      underscore: "lib/lodash/lodash",
      backbone: "lib/backbone/backbone",
      text: "lib/require/text",
      bootstrap: "lib/bootstrap/bootstrap",
      mustache: "lib/mustache",
      moment: "lib/moment",
      logger: "com/logger",
      eventbundler: "com/event",
      connector: "com/connector",
      templates: "../templates",
      amplifystore: "lib/backbone/amplify.store",
      localstorage: "lib/backbone/backbone.amplify",
      wfwsocket: "com/wfwsocket"
    }
  });

}).call(this);
