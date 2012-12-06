(function() {

  define(['com/env/local'], function(Enviroment) {
    var Common;
    Common = {};
    Common.Env = new Enviroment;
    Common.ServerLog = false;
    Common.Log = true;
    return Common;
  });

}).call(this);
