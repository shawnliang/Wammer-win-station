(function() {

  require(['config'], function(main) {
    return require(['logger', 'jasmine', 'jasminehtml', 'wfwsocket', 'collections/events'], function(Logger, Jasmine, JasmineHtml, Events) {
      var htmlReporter, jasmineEnv, root, wsOpen;
      root = window;
      wsOpen = function() {
        console.log("wsOpen");
        describe("WebSocket open", function() {
          return it("WebSocket 接口開啟", function() {
            expect(root.wfwsocket.status).not.toBe(false);
            return expect(root.wfwsocket.status.code).toBe(1);
          });
        });
        describe("Event 集合", function() {
          it("Events Collection to be defined", function() {
            return expect(Events.length).toBeDefined();
          });
          return it("Add one event model", function() {});
        });
        return jasmineEnv.execute();
      };
      dispatch.on("socket:open", wsOpen, this);
      describe("WebScoket test", function() {
        it("WebSocket 是全域變數(wfwsocket)", function() {
          return expect(root.wfwsocket != null).toBe(true);
        });
        return it("WebSocket 建立連線", function() {
          return wfwsocket.init(function() {});
        });
      });
      jasmineEnv = jasmine.getEnv();
      htmlReporter = new jasmine.HtmlReporter;
      jasmineEnv.addReporter(htmlReporter);
      return jasmineEnv.execute();
    });
  });

}).call(this);
