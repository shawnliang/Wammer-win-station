(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/document', 'modules/attachment_caller', 'global'], function(_, Backbone, Document, AttachmentCaller) {
    var DocumentCollection;
    DocumentCollection = (function(_super) {

      __extends(DocumentCollection, _super);

      function DocumentCollection() {
        return DocumentCollection.__super__.constructor.apply(this, arguments);
      }

      DocumentCollection.mixin(AttachmentCaller);

      DocumentCollection.prototype.type = DocumentCollection.TYPE_DOC;

      DocumentCollection.prototype.model = Document;

      DocumentCollection.prototype.schemaName = 'docs';

      DocumentCollection.prototype.dateGroup = [];

      DocumentCollection.prototype.initialize = function() {
        return dispatch.on("store:change:" + this.schemaName + ":all", this.updateAttachment, this);
      };

      return DocumentCollection;

    })(Backbone.Collection);
    return new DocumentCollection;
  });

}).call(this);
