(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'views/layouts/day_view', 'mustache', 'views/partials/document', 'collections/documents', 'text!templates/documents.html'], function(_, DayView, M, DocumentView, Documents, Template) {
    var DocumentsView;
    DocumentsView = (function(_super) {

      __extends(DocumentsView, _super);

      function DocumentsView() {
        return DocumentsView.__super__.constructor.apply(this, arguments);
      }

      DocumentsView.prototype.id = 'documents';

      DocumentsView.prototype.initialize = function() {};

      DocumentsView.prototype.render = function(date) {
        var data, docsData,
          _this = this;
        this.mode = 'photo';
        this.date = date;
        this.setDates();
        data = {
          currentDate: this.currentDate,
          nextDate: this.nextDate,
          previousDate: this.previousDate
        };
        this.$el.html(M.render(Template, data));
        docsData = this.collection.filterByDate(date);
        docsData.each(function(doc) {
          return _this.addOne(doc);
        });
        this.delegateEvents(_.extend(this.events, {}));
        this.setHeaderStyle();
        this.setKey();
        return this;
      };

      DocumentsView.prototype.addOne = function(model) {
        var photoHeight, photoWidth, view;
        photoWidth = Math.floor(($('#main').width() - 80) * 0.25);
        photoHeight = Math.floor(photoWidth * 1.3);
        view = new DocumentView({
          model: model,
          width: photoWidth,
          height: photoHeight
        });
        return this.$(".docs").append(view.render().el);
      };

      DocumentsView.prototype.loadMore = function() {
        return this.collection.loadMore();
      };

      DocumentsView.prototype.setKey = function() {
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
        return Mousetrap.bind('c', function() {
          _this.calendarSwitch();
          return false;
        });
      };

      DocumentsView.prototype.calendarSwitch = function() {
        var calendarOptions, photoDays,
          _this = this;
        if (this.mode === 'calendar') {
          return this.render(this.date);
        } else if (this.mode === 'photo') {
          photoDays = this.collection.map(function(model) {
            return {
              start: model.get('dateUri'),
              title: 'Document',
              allDay: true,
              backgroundColor: '#6E93AA',
              borderColor: '#6E93AA'
            };
          });
          photoDays = _.uniq(photoDays, false, function(day) {
            return day.start;
          });
          calendarOptions = {
            events: photoDays,
            year: this.currentYear,
            month: this.currentMonth,
            header: false,
            height: $('#main').height() - 100,
            eventClick: function(EventObj) {
              return Router.navigate('photos/' + moment(EventObj.start).format('YYYY-MM-DD'), {
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

      return DocumentsView;

    })(DayView);
    return new DocumentsView({
      collection: Documents
    });
  });

}).call(this);
