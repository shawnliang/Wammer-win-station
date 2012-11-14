(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['com/env/base'], function(BaseEnv) {
    var WinEnv;
    return WinEnv = (function(_super) {

      __extends(WinEnv, _super);

      WinEnv.prototype.uri = "ws://192.168.1.97:1337";

      function WinEnv() {
        this.envName = 'Windows';
        console.log('Windows env init');
      }

      return WinEnv;

    })(BaseEnv);
  });

}).call(this);
