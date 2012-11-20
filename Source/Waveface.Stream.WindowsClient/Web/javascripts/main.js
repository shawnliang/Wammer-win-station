(function() {

  require(['config'], function(main) {
    return require(['backbone', 'bootstrap', 'logger', 'router', 'wfwsocket'], function(Backbone) {});
  });

}).call(this);
