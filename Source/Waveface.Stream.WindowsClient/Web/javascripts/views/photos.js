(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'views/layouts/day_view', 'mustache', 'views/partials/attachment', 'views/partials/photo_grid', 'views/gallery', 'collections/attachments', 'text!templates/photos.html', 'bootstrap', 'jqueryui'], function(_, DayView, M, AttachmentView, PhotoGridView, GalleryView, Attachments, Template) {
    var PhotosView;
    PhotosView = (function(_super) {

      __extends(PhotosView, _super);

      function PhotosView() {
        return PhotosView.__super__.constructor.apply(this, arguments);
      }

      PhotosView.prototype.id = 'photos';

      PhotosView.prototype.initialize = function() {
        return this.selectedPhotoId = [];
      };

      PhotosView.prototype.render = function(date) {
        var data, photoData;
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
        photoData = this.collection.filterByDate(date);
        this.renderPhotoGrid(photoData);
        this.delegateEvents(_.extend(this.events, {
          'dblclick .frame': this.initGallery,
          'click .action-new-collection': this.newCollection,
          'click .action-add-collection': this.addToCollection,
          'click .action-delete-photo': this.deletePhoto,
          'click .start-edit': this.startEdit,
          'click .done-edit': this.doneEdit
        }));
        this.initSelectable();
        this.setHeaderStyle();
        this.setKey();
        return this;
      };

      PhotosView.prototype.renderPhotoGrid = function(photoData) {
        var view;
        if (photoData.size() < 1) {
          return this;
        }
        view = new PhotoGridView();
        if (view.render(photoData)) {
          this.$(".photos").append(view.el);
        }
        return this.renderPhotoGrid(view.photoData);
      };

      PhotosView.prototype.addOne = function(model) {
        var view;
        view = new AttachmentView({
          model: model
        });
        return this.$(".photos").append(view.render().el);
      };

      PhotosView.prototype.loadMore = function() {
        return this.collection.loadMore();
      };

      PhotosView.prototype.initGallery = function(e) {
        var data, start;
        if (!!this.collection) {
          start = this.$('.frame').index(e.currentTarget);
          data = this.collection.filterByDate(this.date).map(function(model) {
            return {
              thumb: model.get('image_meta').small.url,
              image: model.get('image_meta').medium.url
            };
          });
          return GalleryView.render(data, e, start);
        } else {
          return false;
        }
      };

      PhotosView.prototype.startEdit = function() {
        this.$('.start-edit-buttons').hide();
        this.$('.edit-buttons').show();
        return this.$selectable.selectable('enable');
      };

      PhotosView.prototype.doneEdit = function() {
        this.$('.start-edit-buttons').show();
        this.$('.edit-buttons').hide();
        this.$selectable.selectable('disable');
        return this.$selectable.find('.ui-selected').removeClass('ui-selected');
      };

      PhotosView.prototype.initSelectable = function() {
        var _this = this;
        return this.$selectable = this.$('.photo-grid').selectable({
          appendTo: '#client',
          filter: '.frame',
          disabled: 'true',
          selected: function(e, ui) {
            return _this.selectedPhotoId.push(_this.$(ui.selected).data('id'));
          },
          unselected: function(e, ui) {
            return _this.selectedPhotoId = _.without(_this.selectedPhotoId, _this.$(ui.unselected).data('id'));
          },
          start: function() {
            return _this.$('.btn-group').removeClass('open').find('.btn').blur();
          }
        });
      };

      PhotosView.prototype.newCollection = function(e) {
        e.preventDefault();
        if (this.selectedPhotoId.length > 0) {
          console.log(10000, 'CREATE NEW COLLECTION');
          console.log(10001, 'ADD THESE TO THE NEW COLECTION!!!', this.selectedPhotoId);
          return console.log(10002, 'GO TO THE COLLECTION PAGE');
        }
      };

      PhotosView.prototype.addToCollection = function(e) {
        e.preventDefault();
        if (this.selectedPhotoId.length > 0) {
          return console.log(10000, 'ADD THESE TO COLECTION!!!', this.selectedPhotoId);
        }
      };

      PhotosView.prototype.deletePhoto = function(e) {
        e.preventDefault();
        if (this.selectedPhotoId.length > 0) {
          return console.log(10000, 'DELETE THESE PHOTOS!!!', this.selectedPhotoId);
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
              title: 'Photo',
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

      return PhotosView;

    })(DayView);
    return new PhotosView({
      collection: Attachments
    });
  });

}).call(this);
