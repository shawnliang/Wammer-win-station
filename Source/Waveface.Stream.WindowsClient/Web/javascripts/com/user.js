(function() {

  define(['localstorage', 'eventbundler'], function(Storage) {
    var User;
    User = (function() {

      User.prototype.userData = false;

      User.prototype.storageKey = "nc_user_info";

      function User() {
        dispatch.on('user:info:save', this.fetchData, this);
      }

      User.prototype.get = function(_property) {
        if (!this.userData) {
          this.fetchData();
        }
        return this.userData[_property];
      };

      User.prototype.fetchData = function(data) {
        var storage;
        storage = new Storage(this.storageKey);
        if (!!data) {
          storage.data = this.userData = data.response;
          storage.save();
        } else {
          this.userData = storage.data;
        }
        return this.userData;
      };

      return User;

    })();
    /*
    */

    return new User;
  });

}).call(this);
