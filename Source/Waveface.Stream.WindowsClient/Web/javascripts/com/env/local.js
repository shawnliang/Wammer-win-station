(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['com/env/base'], function(BaseEnv) {
    var LocalEnv;
    return LocalEnv = (function(_super) {

      __extends(LocalEnv, _super);

      LocalEnv.prototype.uri = "ws://127.0.0.1:1337";

      function LocalEnv() {
        this.envName = "Local";
      }

      return LocalEnv;

    })(BaseEnv);
  });

}).call(this);
