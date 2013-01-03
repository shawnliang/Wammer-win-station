(function() {
  var __slice = [].slice;

  define([], function() {
    return Object.defineProperty(Object.prototype, "mixin", {
      value: function() {
        var Klasses,
          _this = this;
        Klasses = 1 <= arguments.length ? __slice.call(arguments, 0) : [];
        return Klasses.forEach(function(Klass) {
          var key, value, _ref;
          for (key in Klass) {
            value = Klass[key];
            _this[key] = value;
          }
          _ref = Klass.prototype;
          for (key in _ref) {
            value = _ref[key];
            _this.prototype[key] = value;
          }
          return _this;
        });
      },
      enumerable: false
    });
  });

}).call(this);
