(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'views/layouts/day_view', 'mustache', 'views/partials/attachment', 'views/gallery', 'collections/attachments', 'text!templates/photos.html'], function(_, DayView, M, AttachmentView, GalleryView, Attachments, Template) {
    var PhotosView;
    PhotosView = (function(_super) {

      __extends(PhotosView, _super);

      function PhotosView() {
        return PhotosView.__super__.constructor.apply(this, arguments);
      }

      PhotosView.prototype.id = 'photos';

      PhotosView.prototype.initialize = function() {};

      PhotosView.prototype.render = function(date) {
        var data,
          _this = this;
        dispatch.on("render:change:attachment:all", this.render, this);
        this.mode = 'photo';
        this.date = date;
        this.setDates();
        data = {
          currentDate: this.currentDate,
          nextDate: this.nextDate,
          previousDate: this.previousDate
        };
        this.$el.html(M.render(Template, data));
        this.collection.filterByDate(date).each(function(model) {
          return _this.addOne(model);
        });
        this.delegateEvents(_.extend(this.events, {
          'click .attachment': this.initGallery
        }));
        this.setHeaderStyle();
        this.setKey();
        return this;
      };

      PhotosView.prototype.addOne = function(model) {
        var photoWidth, view;
        photoWidth = Math.floor(($('#main').width() - 80) * 0.125);
        view = new AttachmentView({
          model: model,
          width: photoWidth,
          height: photoWidth
        });
        return this.$(".photos").append(view.render().el);
      };

      PhotosView.prototype.loadMore = function() {
        return this.collection.loadMore();
      };

      PhotosView.prototype.initGallery = function(e) {
        var data;
        if (!!this.collection) {
          data = this.collection.filterByDate(this.date).map(function(model) {
            return {
              thumb: model.get('image_meta').small.url,
              image: model.get('image_meta').medium.url
            };
          });
          return GalleryView.render(data, e);
        } else {
          return false;
        }
      };

      PhotosView.prototype.setKey = function() {
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

      PhotosView.prototype.calendarSwitch = function() {
        var calendarOptions, photoDays,
          _this = this;
        if (this.mode === 'calendar') {
          return this.render(this.date);
        } else if (this.mode === 'photo') {
          photoDays = this.collection.map(function(model) {
            return {
              start: model.get('dateUri'),
              title: 'Photo'
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

      return PhotosView;

    })(DayView);
    return new PhotosView({
      collection: Attachments
    });
  });

}).call(this);
