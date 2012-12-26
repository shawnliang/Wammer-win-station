(function() {

  define(['async!https://maps.googleapis.com/maps/api/js?key=AIzaSyCAo25juPfKZGZ08gdgVPr_ZKSPNjxA6aw&sensor=false'], function(gmaps) {
    if (window.navigator.online) {
      return window.google.maps;
    }
    return {};
  });

}).call(this);
