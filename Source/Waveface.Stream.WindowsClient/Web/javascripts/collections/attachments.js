(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/attachment', 'localstorage', 'com/subscriber', 'modules/attachment_caller', 'global'], function(_, Backbone, Photo, LocalStorage, Subscriber, AttachmentCaller) {
    var AttachmentCollection;
    AttachmentCollection = (function(_super) {

      __extends(AttachmentCollection, _super);

      function AttachmentCollection() {
        return AttachmentCollection.__super__.constructor.apply(this, arguments);
      }

      AttachmentCollection.mixin(AttachmentCaller);

      AttachmentCollection.prototype.type = AttachmentCollection.TYPE_PHOTO;

      AttachmentCollection.prototype.model = Photo;

      AttachmentCollection.prototype.schemaName = 'photo';

      AttachmentCollection.prototype.dateGroup = [];

      AttachmentCollection.prototype.localStorage = new LocalStorage('attachments');

      AttachmentCollection.prototype.initialize = function() {
        return dispatch.on("store:change:" + this.schemaName + ":all", this.updateAttachment, this);
      };

      AttachmentCollection.prototype.filterByDate = function(date) {
        if (!date) {
          return this;
        }
        return _(this.filter(function(model) {
          var photoDate;
          photoDate = model.get('dateUri');
          return photoDate === date;
        }));
      };

      AttachmentCollection.prototype.nextDate = function(date) {
        var dateIndex;
        dateIndex = _.indexOf(this.dateGroup, date);
        return this.dateGroup[dateIndex - 1];
      };

      AttachmentCollection.prototype.previousDate = function(date) {
        var dateIndex;
        dateIndex = _.indexOf(this.dateGroup, date);
        if (!this.dateGroup[dateIndex + 3]) {
          dispatch.trigger("more:attach:bylength");
        }
        return this.dateGroup[dateIndex + 1];
      };

      AttachmentCollection.prototype.loadMore = function(pageSize, pageNo) {
        var memo, params, photosViewStore, viewInfo;
        if (pageSize == null) {
          pageSize = 100;
        }
        if (pageNo == null) {
          pageNo = 2;
        }
        photosViewStore = new LocalStorage('nc_photos_view_data');
        viewInfo = photosViewStore.data;
        if (!!viewInfo.pageSize && !!viewInfo.pageNo) {
          pageSize = viewInfo.pageSize;
          pageNo = viewInfo.pageNo += 1;
          photosViewStore.data = viewInfo;
        } else {
          viewInfo = photosViewStore.data = {
            pageSize: pageSize,
            pageNo: pageNo
          };
        }
        photosViewStore.save();
        params = {
          page_size: pageSize,
          page_no: pageNo
        };
        memo = {
          namespace: 'all'
        };
        return wfwsocket.sendMessage('getAttachments', params, memo);
      };

      return AttachmentCollection;

    })(Backbone.Collection);
    ({
      loadByLength: function(pageSize) {
        var localAttachsNum, memo, pageNow, params;
        if (pageSize == null) {
          pageSize = 200;
        }
        localAttachsNum = this.length;
        pageNow = parseInt(localAttachsNum / pageSize);
        params = {
          page_size: pageSize,
          page_no: pageNow + 1
        };
        memo = {
          namespace: 'all'
        };
        return wfwsocket.sendMessage('getAttachments', params, memo);
      }
    });
    return new AttachmentCollection;
  });

}).call(this);
