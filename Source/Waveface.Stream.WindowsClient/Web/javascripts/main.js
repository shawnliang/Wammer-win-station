(function() {

  require(['config'], function(main) {
    return require(['router', 'wfwsocket', 'views/app', 'logger'], function(Router, Socket, AppView, Logger) {
      var root;
      root = this;
      root.Logger = Logger;
      root.Router = new Router;
      return Socket.init(function() {
        new AppView();
        return Backbone.history.start();
      });
    });
  });

}).call(this);
