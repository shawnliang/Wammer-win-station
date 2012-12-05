(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone'], function(_, Backbone) {
    var CalendarModel;
    return CalendarModel = (function(_super) {

      __extends(CalendarModel, _super);

      function CalendarModel() {
        return CalendarModel.__super__.constructor.apply(this, arguments);
      }

      CalendarModel.prototype.initialize = function() {};

      return CalendarModel;

    })(Backbone.Model);
  });

}).call(this);
