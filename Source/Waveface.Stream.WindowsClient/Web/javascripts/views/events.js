(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'mustache', 'mousetrap', 'views/event_sum', 'views/event_detail', 'collections/events', 'text!templates/events.html'], function(_, Backbone, M, Mousetrap, EventSumView, EventDetailView, Events, Template) {
    var EventsView;
    EventsView = (function(_super) {

      __extends(EventsView, _super);

      function EventsView() {
        return EventsView.__super__.constructor.apply(this, arguments);
      }

      EventsView.prototype.id = 'events';

      EventsView.prototype.collection = Events;

      EventsView.prototype.events = {
        'click #nextDate': 'nextPage',
        'click #previousDate': 'previousPage'
      };

      EventsView.prototype.render = function(date, id) {
        var data, event,
          _this = this;
        if (!(date != null)) {
          Logger.log('ERROR: NO DATE');
          return false;
        }
        if (id != null) {
          event = this.collection.get(id);
          if (event != null) {
            date = event.get('dateUri');
          }
        }
        this.date = date;
        this.setDates();
        data = {
          currentDate: this.currentDate,
          nextDate: this.nextDate,
          previousDate: this.previousDate
        };
        this.$el.html(M.render(Template, data));
        this.$el.on('click', '#event-list .event', function(e) {
          $(e.delegateTarget).find('.event').removeClass('selected');
          return $(this).addClass('selected');
        });
        this.collection.filterByDate(date).each(function(model, index) {
          return _this.addOne(model, index);
        });
        this.delegateEvents();
        this.setKey();
        this.setHeaderStyle();
        if (event != null) {
          this.viewEventDetail(event);
        }
        return this;
      };

      EventsView.prototype.addOne = function(event, index) {
        var view;
        view = new EventSumView({
          model: event
        });
        view.on('viewDetail', this.viewEventDetail, this);
        this.$('#event-list').append(view.render().el);
        if (index === 0) {
          this.viewEventDetail(event);
          return view.$el.addClass('selected');
        }
      };

      EventsView.prototype.viewEventDetail = function(event) {
        if (!event) {
          return false;
        }
        if (this.detailView != null) {
          Logger.log('KILL Zombie');
          this.detailView.remove();
          this.detailView.model.off();
        }
        this.detailView = new EventDetailView({
          model: event
        });
        this.$('#event-detail').empty().append(this.detailView.render().el);
        this.id = event.id;
        this.date = event.get("dateUri");
        return Router.navigate("events/" + this.date + "/" + event.id);
      };

      EventsView.prototype.nextPage = function() {
        if (this.nextDate) {
          return Router.navigate("events/" + this.nextDateUri, {
            trigger: true
          });
        }
      };

      EventsView.prototype.previousPage = function() {
        if (this.previousDate) {
          return Router.navigate("events/" + this.previousDateUri, {
            trigger: true
          });
        }
      };

      EventsView.prototype.nextEvent = function() {
        var nextModel;
        nextModel = this.collection.nextEvent(this.id);
        if (!nextModel) {
          return false;
        }
        if (nextModel.get("dateUri") !== this.date) {
          return this.nextPage();
        } else {
          return this.viewEventDetail(nextModel);
        }
      };

      EventsView.prototype.previousEvent = function() {
        var previousModel;
        previousModel = this.collection.previousEvent(this.id);
        if (!previousModel) {
          return false;
        }
        if (previousModel.get("dateUri") !== this.date) {
          return this.previousPage();
        } else {
          return this.viewEventDetail(previousModel);
        }
      };

      EventsView.prototype.hasNextPage = function() {
        if (this.date != null) {
          return !!this.collection.next(this.date);
        } else {
          return false;
        }
      };

      EventsView.prototype.setDates = function() {
        this.currentDate = moment(this.date).format('MMMM DD, YYYY');
        if (this.collection.previous(this.date)) {
          this.previousDateUri = this.collection.previous(this.date);
          this.previousDate = moment(this.previousDateUri).format('MMMM DD');
        } else {
          this.previousDateUri = null;
          this.previousDate = null;
        }
        if (this.collection.next(this.date)) {
          this.nextDateUri = this.collection.next(this.date);
          return this.nextDate = moment(this.nextDateUri).format('MMMM DD');
        } else {
          this.nextDateUri = null;
          return this.nextDate = null;
        }
      };

      EventsView.prototype.setHeaderStyle = function() {
        if (this.hasNextPage()) {
          return this.$('#nextDate').removeClass('disabled');
        } else {
          return this.$('#nextDate').addClass('disabled');
        }
      };

      EventsView.prototype.setKey = function(event) {
        var _this = this;
        Mousetrap.bind('right', function() {
          _this.nextPage();
          return false;
        });
        Mousetrap.bind('left', function() {
          _this.previousPage();
          return false;
        });
        Mousetrap.bind('up', function() {
          _this.nextEvent();
          return false;
        });
        return Mousetrap.bind('down', function() {
          _this.previousEvent();
          return false;
        });
      };

      return EventsView;

    })(Backbone.View);
    return new EventsView({
      collection: Events
    });
  });

}).call(this);
