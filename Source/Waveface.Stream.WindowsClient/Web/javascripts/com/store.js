(function() {
  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  define(['underscore', 'backbone', 'lib/backbone/localstorage'], function(_, Backbone, LocalStorage) {
    var Store;
    return Store = (function() {

      function Store() {
        this.sync = __bind(this.sync, this);

      }

      Store.prototype.sync = function(method, model, options) {
        var resp, schemaName;
        resp = false;
        schemaName = this.getSchemaName(model);
        switch (method) {
          case "create":
            Logger.log("create was not happen");
            break;
          case "read":
            resp = model.id ? this.find(schema, model.id) : this.findAll(schema);
            if (!resp) {
              return options.error("Not found.");
            }
            break;
          case "destroy":
            resp = true;
        }
        if (resp) {
          return options.success(resp);
        } else {
          return options.error("Unknow error.");
        }
      };

      Store.prototype.find = function(schema, id) {
        return this.data[schema] && this.data[schema][id];
      };

      Store.prototype.findAll = function(schema) {
        return _.values(this.data[schema] || []);
      };

      Store.prototype.getSchemaName = function(model) {
        if (model.schemaName) {
          return model.schemaName;
        } else if (model.collection && model.collection.schemaName) {
          return model.collection.schemaName;
        }
      };

      return Store;

    })();
  });

}).call(this);
