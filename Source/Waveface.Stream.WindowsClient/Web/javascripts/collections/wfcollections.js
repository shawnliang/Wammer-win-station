(function() {
  var __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  define(['underscore', 'backbone', 'models/wfcollection', 'localstorage', 'eventbundler'], function(_, Backbone, WFCollection, LocalStorage, EventBundler) {
    var WFCollectionCollection;
    WFCollectionCollection = (function(_super) {

      __extends(WFCollectionCollection, _super);

      function WFCollectionCollection() {
        return WFCollectionCollection.__super__.constructor.apply(this, arguments);
      }

      WFCollectionCollection.prototype.model = WFCollection;

      WFCollectionCollection.prototype.schemaName = 'collections';

      WFCollectionCollection.prototype.initialize = function() {
        return dispatch.on("store:change:" + this.schemaName + ":all", this.updateCollect, this);
      };

      WFCollectionCollection.prototype.comparator = function(model) {
        return -moment(model.get("timestamp")).unix();
      };

      WFCollectionCollection.prototype.updateCollect = function(data, ns) {
        var collects,
          _this = this;
        collects = data.collections;
        if (collects.length <= 0) {
          return false;
        }
        return _.each(collects, function(collect, index) {
          return _this.add(collect);
        });
      };

      WFCollectionCollection.prototype.callCollections = function(namespace) {
        var memo, pageNo, params, storage, viewData;
        if (namespace == null) {
          namespace = "all";
        }
        new EventBundler("GetCollections", namespace);
        storage = new LocalStorage("nc_collect_view_data");
        viewData = storage.data;
        if (viewData.pageNo != null) {
          pageNo = viewData.pageNo += 1;
        } else {
          pageNo = viewData.pageNo = 1;
        }
        storage.data = viewData;
        storage.save();
        memo = {
          namespace: namespace,
          type: this.schemaName
        };
        params = {
          page_no: pageNo,
          page_size: 30
        };
        return wfwsocket.sendMessage("getCollections", params, memo);
      };

      return WFCollectionCollection;

    })(Backbone.Collection);
    return new WFCollectionCollection;
  });

}).call(this);
