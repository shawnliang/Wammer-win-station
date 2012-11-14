(function() {

  require(['config'], function(main) {
    return require(['bootstrap', 'logger', 'wfwsocket'], function(Bootstrap) {});
  });

}).call(this);
