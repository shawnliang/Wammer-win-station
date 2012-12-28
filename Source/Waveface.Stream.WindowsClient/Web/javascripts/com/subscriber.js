(function() {

  define([], function() {
    var Subscriber;
    Subscriber = (function() {

      function Subscriber() {}

      Subscriber.prototype.E_ALL = 0;

      Subscriber.prototype.E_USR_CHG = 1;

      Subscriber.prototype.E_POS_ADD = 10;

      Subscriber.prototype.E_POS_UPD = 11;

      Subscriber.prototype.E_ATC_DLD = 20;

      Subscriber.prototype.E_COL_ADD = 30;

      Subscriber.prototype.E_COL_UPD = 31;

      Subscriber.prototype.POST_ADDED = 10;

      Subscriber.prototype.POST_UPDATE = 11;

      return Subscriber;

    })();
    return new Subscriber;
  });

}).call(this);
