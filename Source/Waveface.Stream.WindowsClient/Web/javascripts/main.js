(function() {

  require(['config'], function(main) {
    return require(['router', 'wfwsocket', 'views/layouts/app', 'logger', 'collections/attachments', 'collections/documents', 'collections/wfcollections', 'collections/calendar', 'lib/jquery/jquery.blockUI', 'scrollTo'], function(Router, Socket, AppView, Logger, Attachments, Documents, WFCollections, Calendars) {
      var root;
      window.dialog = {};
      window.dialog.css = {
        border: 'none',
        padding: '15px',
        backgroundColor: '#000',
        opacity: 1,
        '-webkit-border-radius': '10px',
        '-moz-border-radius': '10px',
        'border-radius': '10px',
        color: '#fff',
        width: 'auto',
        left: '50%'
      };
      $.blockUI({
        css: window.dialog.css,
        message: $("<center><img src='images/loading.gif'/></center>")
      });
      root = this;
      root.Logger = Logger;
      root.Router = new Router;
      return Socket.init(function() {
        Backbone.history.start();
        new AppView();
        Attachments.callAttachments();
        Documents.callAttachments();
        WFCollections.callCollections();
        return Calendars.callCalendar();
      });
    });
  });

}).call(this);
