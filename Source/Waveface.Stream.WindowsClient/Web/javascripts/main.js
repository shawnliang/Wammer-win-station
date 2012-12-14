(function() {

  require(['config'], function(main) {
    return require(['router', 'wfwsocket', 'views/layouts/app', 'logger', 'collections/attachments', 'collections/documents'], function(Router, Socket, AppView, Logger, Attachments, Documents) {
      var root;
      root = this;
      root.Logger = Logger;
      root.Router = new Router;
      return Socket.init(function() {
        new AppView();
        Backbone.history.start();
        Attachments.callAttachments();
        return Documents.callAttachments();
      });
    });
  });

}).call(this);
