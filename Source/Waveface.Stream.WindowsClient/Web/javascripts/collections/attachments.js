(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/attachment', 'localstorage', 'eventbundler'], function(_, Backbone, Attachment, LocalStorage, EventBundler) {
    var AttachmentCollection;
    AttachmentCollection = (function(_super) {

      __extends(AttachmentCollection, _super);

      function AttachmentCollection() {
        return AttachmentCollection.__super__.constructor.apply(this, arguments);
      }

      AttachmentCollection.prototype.model = Attachment;

      AttachmentCollection.prototype.schemaName = 'attachments';

      AttachmentCollection.prototype.dateGroup = [];

      AttachmentCollection.prototype.localStorage = new LocalStorage('attachments');

      AttachmentCollection.prototype.initialize = function() {
        dispatch.on("store:change:attachment:all", this.updateAttachment, this);
        return dispatch.on("more:attach:bylength", this.loadByLength, this);
      };

      AttachmentCollection.prototype.updateAttachment = function(data, ns) {
        var attachments, lastDate,
          _this = this;
        attachments = data.attachments;
        lastDate = false;
        if (attachments.length <= 0) {
          return false;
        }
        _.each(attachments, function(attachment, index) {
          var attachDate;
          attachDate = moment(attachment.timestamp).format('YYYY-MM-DD');
          if (index === 0) {
            lastDate = attachDate;
          }
          _this.dateGroup.push(attachDate);
          return _this.add(attachment);
        });
        this.dateGroup = _.uniq(this.dateGroup);
        if (ns !== "all") {
          return dispatch.trigger("render:change:attachment:" + ns, attachments);
        }
      };

      AttachmentCollection.prototype.callAttachments = function(post_id, pageSize, namespace, callback, context) {
        var memo, params;
        if (post_id == null) {
          post_id = false;
        }
        if (pageSize == null) {
          pageSize = 200;
        }
        if (namespace == null) {
          namespace = "all";
        }
        params = {
          page_size: pageSize,
          type: 1024
        };
        new EventBundler('GetAttachments', namespace);
        if (!!post_id) {
          params['post_id_array'] = post_id;
        }
        if (!!callback && !!context) {
          dispatch.off("store:change:attachment:" + namespace);
          dispatch.off("render:change:attachment:" + namespace);
          dispatch.on("render:change:attachment:" + namespace, callback, context);
          dispatch.on("store:change:attachment:" + namespace, this.updateAttachment, this);
        }
        memo = {
          namespace: namespace
        };
        return wfwsocket.sendMessage('getAttachments', params, memo);
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
    return new AttachmentCollection();
  });

}).call(this);
