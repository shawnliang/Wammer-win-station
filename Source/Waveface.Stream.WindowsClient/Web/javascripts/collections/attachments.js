(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/attachment', 'connector', 'localstorage'], function(_, Backbone, Attachment, LocalStorage) {
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

      AttachmentCollection.prototype.updateAttachment = function(data) {
        var attachments,
          _this = this;
        attachments = data.attachments;
        attachments.id = attachments.object_id;
        return _.each(attachments, function(attachment) {
          return _this.add(attachment);
        });
      };

      AttachmentCollection.prototype.callAttachments = function(pageSize) {
        var memo, params;
        if (pageSize == null) {
          pageSize = 50;
        }
        params = {
          page_size: pageSize
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
