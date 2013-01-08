(function() {

  require.config({
    shim: {
      underscore: {
        exports: "_"
      },
      jquery: {
        exports: "$"
      },
      jqueryui: {
        deps: ["jquery"]
      },
      scrollTo: {
        deps: ["jquery"]
      },
      backbone: {
        deps: ["underscore", "amplifystore", "jquery"],
        exports: "Backbone"
      },
      bootstrap: {
        deps: ["jquery"]
      },
      mousetrap: {
        exports: "Mousetrap"
      },
      'lib/jquery/fullcalendar': {
        deps: ["jquery"]
      },
      'lib/jquery/jquery.blockUI': {
        deps: ["jquery"]
      },
      'lib/galleria/galleria-1.2.8': {
        deps: ["jquery"],
        exports: "Galleria"
      },
      'lib/photobox': ["jquery"],
      'lib/d3': {
        exports: "d3"
      },
      'lib/lodash/underscore.string': {
        deps: ["underscore"]
      },
      logger: {
        deps: ["underscore", "jquery"]
      }
    },
    paths: {
      jquery: "lib/jquery/jquery",
      jqueryui: "lib/jquery/jquery-ui-1.9.2.custom",
      scrollTo: "lib/jquery/jquery.scrollTo.min",
      underscore: "lib/lodash/lodash",
      backbone: "lib/backbone/backbone",
      text: "lib/require/text",
      bootstrap: "lib/bootstrap/bootstrap",
      mustache: "lib/mustache",
      moment: "lib/moment",
      mousetrap: "lib/mousetrap",
      logger: "com/logger",
      eventbundler: "com/event",
      connector: "com/connector",
      templates: "../templates",
      amplifystore: "lib/backbone/amplify.store",
      localstorage: "lib/backbone/backbone.amplify",
      wfwsocket: "com/wfwsocket",
      googlemaps: "com/gmap",
      async: "lib/require/async",
      jasmine: "lib/jasmine/jasmine",
      jasminehtml: "lib/jasmine/jasmine-html",
      spinner: "lib/spin.min"
    },
    waitSeconds: 30
  });

}).call(this);
