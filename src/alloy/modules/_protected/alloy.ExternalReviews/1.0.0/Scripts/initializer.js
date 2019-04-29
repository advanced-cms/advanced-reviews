define([
    "dojo/_base/declare",
    "epi/routes",
    "epi/shell/store/JsonRest",
    "epi/shell/store/Throttle",
    "epi/_Module"
], function (
    declare,
    routes,
    JsonRest,
    Throttle,
    _Module
) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            var registry = this.resolveDependency("epi.storeregistry");

            //Register store
            registry.add("externalreviews",
                new Throttle(
                    new JsonRest({
                        target: this._getRestPath("externalreviewstore")//,
                        //idProperty: "contentLink"
                    })
                )
            );
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "alloy.externalreviews", storeName: name });
        }
    });
});
