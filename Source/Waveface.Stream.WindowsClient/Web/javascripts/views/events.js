(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'views/layouts/day_view', 'mustache', 'mousetrap', 'views/event_sum', 'views/event_detail', 'collections/events', 'models/event', 'text!templates/events.html', 'localstorage', 'com/subscriber'], function(_, DayView, M, Mousetrap, EventSumView, EventDetailView, Events, EventModel, Template, Storage, Subscriber) {
    var EventsView, POST_ADDED, POST_UPDATE;
    POST_ADDED = Subscriber.POST_ADDED;
    POST_UPDATE = Subscriber.POST_UPDATE;
    EventsView = (function(_super) {

      __extends(EventsView, _super);

      function EventsView() {
        return EventsView.__super__.constructor.apply(this, arguments);
      }

      EventsView.prototype.id = 'events';

      EventsView.prototype.eventView = [];

      EventsView.prototype.initialize = function() {
        dispatch.on("subscribe:change:" + POST_ADDED + ":new:event", this.updateEventMenu, this);
        return dispatch.on("subscribe:change:" + POST_UPDATE + ":update:event", this.checkEventUpdate, this);
      };

      EventsView.prototype.render = function(date, id) {
        var data, event,
          _this = this;
        this.mode = 'event';
        this.date = date;
        if (id != null) {
          event = this.collection.get(id);
          if (event != null) {
            this.currentEvent = event;
            this.date = this.currentEvent.get("dateUri");
          }
        }
        this.setDates();
        data = {
          currentDate: this.currentDate,
          nextDate: this.nextDate,
          previousDate: this.previousDate
        };
        this.$el.html(M.render(Template, data));
        this.collection.filterByDate(this.date).each(function(model, index) {
          return _this.addOne(model, index);
        });
        this.delegateEvents();
        this.setHeaderStyle();
        this.setKey();
        if (event != null) {
          this.eventSelect(event);
        }
        return this;
      };

      EventsView.prototype.addOne = function(event, index, prepend) {
        if (prepend == null) {
          prepend = false;
        }
        this.eventView[event.id] = new EventSumView({
          model: event
        });
        this.eventView[event.id].on('eventSelect', this.eventSelect, this);
        if (prepend) {
          this.$('#events-left').prepend(this.eventView[event.id].render().el);
        } else {
          this.$('#events-left').append(this.eventView[event.id].render().el);
        }
        if (index === 0) {
          return this.eventSelect(event);
        }
      };

      EventsView.prototype.updateEventMenu = function(data, ns) {
        var posts,
          _this = this;
        posts = data.posts;
        if (posts.length <= 0) {
          return false;
        }
        if (ns === "new:event") {
          _.each(posts, function(post) {
            var event;
            event = new EventModel(post);
            if (event.get("dateUri") === _this.date) {
              return _this.addOne(event, 1, true);
            } else {
              return _this.render(_this.date, _this.currentEvent.get("id"));
            }
          });
        }
        return true;
      };

      EventsView.prototype.eventSelect = function(event) {
        if (event) {
          this.currentEvent = event;
          return this.renderCurrentEvent();
        } else {
          return false;
        }
      };

      EventsView.prototype.renderCurrentEvent = function() {
        var self, viewState;
        if (!this.currentEvent) {
          return false;
        }
        if (this.eventDetailView != null) {
          this.eventDetailView.remove();
          this.eventDetailView.model.off();
        }
        this.eventDetailView = new EventDetailView({
          model: this.currentEvent
        });
        self = this;
        this.$('#events-right').fadeOut('fast', function() {
          return $(this).empty().append(self.eventDetailView.render().el).fadeIn('fast', function() {
            if (self.eventDetailView.map != null) {
              return google.maps.event.trigger(self.eventDetailView.map, 'resize');
            }
          });
        });
        this.$('.event').removeClass('selected');
        this.eventView[this.currentEvent.id].$el.addClass('selected');
        viewState = new Storage('nc_view_state:events');
        viewState.data = {
          date: this.date,
          id: this.currentEvent.id
        };
        viewState.save();
        return Router.navigate("events/" + this.date + "/" + this.currentEvent.id);
      };

      EventsView.prototype.checkEventUpdate = function(data) {
        var respEvents,
          _this = this;
        if (!this.currentEvent) {
          return false;
        }
        respEvents = data.posts;
        return _.each(respEvents, function(event) {
          if (_this.currentEvent.id === event.id) {
            return _this.renderCurrentEvent();
          }
        });
      };

      EventsView.prototype.nextEvent = function() {
        var nextModel;
        nextModel = this.collection.nextEvent(this.currentEvent.id);
        if (!nextModel) {
          return false;
        }
        if (nextModel.get("dateUri") === this.date) {
          return this.eventSelect(nextModel);
        }
      };

      EventsView.prototype.previousEvent = function() {
        var previousModel;
        previousModel = this.collection.previousEvent(this.currentEvent.id);
        if (!previousModel) {
          return false;
        }
        if (previousModel.get("dateUri") === this.date) {
          return this.eventSelect(previousModel);
        }
      };

      EventsView.prototype.calendarSwitch = function() {
        var calendarOptions, eventDays,
          _this = this;
        if (this.mode === 'calendar') {
          return this.render(this.date);
        } else if (this.mode === 'event') {
          eventDays = this.collection.map(function(model) {
            return {
              start: model.get('dateUri'),
              title: 'Events',
              allDay: true,
              backgroundColor: '#C85E52',
              borderColor: '#C85E52'
            };
          });
          eventDays = _.uniq(eventDays, false, function(day) {
            return day.start;
          });
          calendarOptions = {
            events: eventDays,
            year: this.currentYear,
            month: this.currentMonth,
            header: false,
            height: $('#main').height() - 100,
            eventClick: function(EventObj) {
              return Router.navigate('events/' + moment(EventObj.start).format('YYYY-MM-DD'), {
                trigger: true
              });
            },
            viewDisplay: function(view) {
              return _this.updateHeaderDate(view.title);
            }
          };
          return this.renderCalendar(calendarOptions);
        }
      };

      EventsView.prototype.setKey = function() {
        var _this = this;
        Mousetrap.reset();
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
        Mousetrap.bind('down', function() {
          _this.previousEvent();
          return false;
        });
        return Mousetrap.bind('c', function() {
          _this.calendarSwitch();
          return false;
        });
      };

      return EventsView;

    })(DayView);
    return new EventsView({
      collection: Events
    });
  });

}).call(this);
