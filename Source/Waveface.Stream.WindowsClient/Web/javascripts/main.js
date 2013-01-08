(function() {

  require(['config'], function(main) {
    return require(['underscore', 'backbone', 'router', 'wfwsocket', 'views/layouts/app', 'logger', 'collections/events', 'collections/attachments', 'collections/documents', 'collections/wfcollections', 'collections/calendar', 'spinner', 'lib/jquery/jquery.blockUI', 'lib/lodash/underscore.string'], function(_, Backbone, Router, Socket, AppView, Logger, Events, Attachments, Documents, WFCollections, Calendars, Spinner) {
      var dialog, root, spinner;
      _.mixin(_.str.exports());
      dialog = {};
      dialog.css = {
        border: 'none',
        padding: '15px',
        backgroundColor: 'none',
        opacity: 1,
        color: '#fff',
        width: 'auto',
        left: '50%'
      };
      spinner = new Spinner({
        length: 19,
        radius: 32
      }).spin();
      $.blockUI({
        css: dialog.css,
        message: spinner.el
      });
      root = this;
      root.Logger = Logger;
      root.Router = new Router;
      return Socket.init(function() {
        var currentView;
        Backbone.history.start();
        new AppView();
        root.currentView = currentView = _(_.words(Backbone.history.fragment, "/")).first();
        if (currentView !== "events") {
          Events.callPosts();
        }
        if (currentView !== "photos") {
          Attachments.callAttachments();
        }
        Documents.callAttachments();
        WFCollections.callCollections();
        return Calendars.callCalendar();
      });
    });
  });

}).call(this);
