(function() {

  define([], function() {
    var BaseEnv;
    return BaseEnv = (function() {

      BaseEnv.prototype.envName = null;

      BaseEnv.prototype.uri = null;

      function BaseEnv() {}

      BaseEnv.prototype.getUri = function() {
        return this.uri;
      };

      BaseEnv.prototype.setUri = function(uri) {
        this.uri = uri;
      };

      BaseEnv.prototype.getName = function() {
        return this.envName;
      };

      return BaseEnv;

    })();
  });

}).call(this);
