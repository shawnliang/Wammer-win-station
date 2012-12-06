(function() {

  require(['config'], function(main) {
    return require(['router', 'wfwsocket', 'views/layouts/app', 'logger', 'collections/attachments'], function(Router, Socket, AppView, Logger, Attachments) {
      var root;
      root = this;
      root.Logger = Logger;
      root.Router = new Router;
      return Socket.init(function() {
        new AppView();
        Backbone.history.start();
        return Attachments.callAttachments();
      });
    });
  });

}).call(this);
