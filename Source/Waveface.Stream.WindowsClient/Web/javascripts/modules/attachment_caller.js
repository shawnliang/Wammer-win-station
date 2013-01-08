(function() {
  var __indexOf = [].indexOf || function(item) { for (var i = 0, l = this.length; i < l; i++) { if (i in this && this[i] === item) return i; } return -1; };

  define(['eventbundler'], function(EventBundler) {
    var AttachmentCaller;
    return AttachmentCaller = (function() {

      function AttachmentCaller() {}

      AttachmentCaller.TYPE_ALL = 0;

      AttachmentCaller.TYPE_PHOTO = 1;

      AttachmentCaller.TYPE_MUSIC = 2;

      AttachmentCaller.TYPE_VIDEO = 4;

      AttachmentCaller.TYPE_DOC = 8;

      AttachmentCaller.prototype.type = AttachmentCaller.TYPE_PHOTO;

      AttachmentCaller.prototype.callAttachments = function(post_id, pageSize, namespace, callback, context) {
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
          type: this.type
        };
        new EventBundler('GetAttachments', namespace);
        if (!!post_id) {
          params['post_id_array'] = post_id;
        }
        if (!!callback && !!context) {
          dispatch.off("store:change:" + this.schemaName + ":" + namespace);
          dispatch.off("render:change:" + this.schemaName + ":" + namespace);
          dispatch.on("render:change:" + this.schemaName + ":" + namespace, callback, context);
          dispatch.on("store:change:" + this.schemaName + ":" + namespace, this.updateAttachment, this);
        }
        memo = {
          namespace: namespace,
          type: this.schemaName
        };
        return wfwsocket.sendMessage('getAttachments', params, memo);
      };

      AttachmentCaller.prototype.updateAttachment = function(data, ns) {
        var attachments, payload,
          _this = this;
        attachments = data.attachments;
        if (attachments.length <= 0) {
          return false;
        }
        _.each(attachments, function(attachment, index) {
          _this.setDateGroup(attachment, index);
          return _this.add(attachment);
        });
        this.pageNow = parseInt(this.length / this.loadSize);
        this.dateGroup = _.uniq(this.dateGroup);
        this.dateGroup.sort();
        this.dateGroup.reverse();
        payload = ns === "all" ? this.first().get("dateUri") : attachments;
        return dispatch.trigger("render:change:" + this.schemaName + ":" + ns, payload);
      };

      AttachmentCaller.prototype.setDateGroup = function(attachment, index) {
        var attachDate;
        attachDate = moment(attachment.timestamp).format('YYYY-MM-DD');
        if (__indexOf.call(this.dateGroup, attachDate) < 0) {
          return this.dateGroup.push(attachDate);
        }
      };

      AttachmentCaller.prototype.filterByDate = function(date) {
        if (!date) {
          return this;
        }
        return _(this.filter(function(model) {
          var attachDate;
          attachDate = model.get('dateUri');
          return attachDate === date;
        }));
      };

      AttachmentCaller.prototype.nextDate = function(date) {
        var dateIndex;
        dateIndex = _.indexOf(this.dateGroup, date);
        return this.dateGroup[dateIndex - 1];
      };

      AttachmentCaller.prototype.previousDate = function(date) {
        var dateIndex;
        dateIndex = _.indexOf(this.dateGroup, date);
        if (!this.dateGroup[dateIndex + 3]) {
          dispatch.trigger("more:" + this.schemaName + ":bylength");
        }
        return this.dateGroup[dateIndex + 1];
      };

      return AttachmentCaller;

    })();
  });

}).call(this);
