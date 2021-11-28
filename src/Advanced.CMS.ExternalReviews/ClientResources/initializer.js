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

            var options = this._settings.options || {};
            if (!options.isEnabled) {
                return;
            }

            var registry = this.resolveDependency("epi.storeregistry");

            //Register store
            registry.add("externalreviews",
                new Throttle(
                    new JsonRest({
                        preventCache: true,
                        target: this._getRestPath("externalreviewstore")//,
                        //idProperty: "contentLink"
                    })
                )
            );
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "advanced-cms-external-reviews", storeName: name });
        }
    });
});
