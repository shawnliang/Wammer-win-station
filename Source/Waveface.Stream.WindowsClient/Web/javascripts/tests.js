(function() {

  require(['config'], function(main) {
    return require(['logger', 'jasmine', 'jasminehtml', 'wfwsocket', 'collections/events', 'models/event', 'text!tests/mocks/posts.json'], function(Logger, Jasmine, JasmineHtml, WFWSocket, Events, EventModel, mockPosts) {
      var excuteTest, htmlReporter, jasmineEnv, root;
      root = window;
      jasmineEnv = jasmine.getEnv();
      htmlReporter = new jasmine.HtmlReporter;
      jasmineEnv.addReporter(htmlReporter);
      (function() {
        var wsOpen;
        wsOpen = function() {
          console.log("wsOpen");
          describe("WebSocket open", function() {
            return it("WebSocket 接口開啟", function() {
              expect(root.wfwsocket.status).not.toBe(false);
              return expect(root.wfwsocket.status.code).toBe(1);
            });
          });
          return jasmineEnv.execute();
        };
        dispatch.on("socket:open", wsOpen, this);
        describe("WebScoket test", function() {
          it("WebSocket 是全域變數(wfwsocket)", function() {
            return expect(root.wfwsocket != null).toBe(true);
          });
          return it("WebSocket 建立連線", function() {
            return wfwsocket.init(function() {
              return true;
            });
          });
        });
        /*
                    #
        */

        describe("Event 集合", function() {
          it("Events Collection is 0", function() {
            return expect(Events.length).toBe(0);
          });
          it("Add one event model", function() {
            Events.add(new EventModel(JSON.parse(mockPosts)));
            return expect(Events.length).toBe(1);
          });
          it("Check Event Date Format", function() {
            var model;
            model = Events.at(0);
            expect(model).toBeDefined();
            expect(model.get("dateUri")).toBeDefined();
            return expect(model.get("dateUri")).toBe("2012-11-08");
          });
          return it("Check Event DateGroup exist", function() {
            return expect(Events.dateGroup).toBeDefined();
          });
        });
        return jasmineEnv.execute();
      }).call(window);
      return excuteTest = function() {};
    });
  });

}).call(this);
