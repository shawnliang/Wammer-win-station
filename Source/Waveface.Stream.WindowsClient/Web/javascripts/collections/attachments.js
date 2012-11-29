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

      AttachmentCollection.prototype.localStorage = new LocalStorage('attachments');

      AttachmentCollection.prototype.initialize = function() {
        return dispatch.on("store:change:attachment:all", this.updateAttachment, this);
      };

      AttachmentCollection.prototype.updateAttachment = function(data, ns) {
        var attachments,
          _this = this;
        attachments = data.attachments;
        _.each(attachments, function(attachment) {
          return _this.add(attachment);
        });
        return dispatch.trigger("render:change:attachment:" + ns, data.attachments);
      };

      AttachmentCollection.prototype.callAttachments = function(post_id, pageSize, namespace, callback, context) {
        var memo, params, photosViewStore;
        if (post_id == null) {
          post_id = false;
        }
        if (pageSize == null) {
          pageSize = 100;
        }
        if (namespace == null) {
          namespace = "all";
        }
        params = {
          page_size: pageSize,
          type: 1024
        };
        new EventBundler('GetAttachments', namespace);
        photosViewStore = new LocalStorage('nc_photos_view_data');
        photosViewStore.reset();
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
          return true;
        }));
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
    return new AttachmentCollection();
  });

}).call(this);
