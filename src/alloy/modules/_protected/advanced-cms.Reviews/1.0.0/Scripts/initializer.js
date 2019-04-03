define([
    "dojo/_base/declare",
    "epi/dependency",
    "epi/routes",
    "epi/shell/store/JsonRest",
    "epi/shell/store/Throttle",
    "epi/_Module",
    "alloy-review/commandsProvider",
    "alloy-review/onPageEditingInitializer"
], function (
    declare,
    dependency,
    routes,
    JsonRest,
    Throttle,
    _Module,
    CommandsProvider,
    onPageEditingInitializer

) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            var registry = this.resolveDependency("epi.storeregistry");

            //Register store
            registry.add("approvaladvancedreview",
                new Throttle(
                    new JsonRest({
                        target: this._getRestPath("approvaladvancedreview")//,
                        //idProperty: "contentLink"
                    })
                )
            );

            var commandsProvider = new CommandsProvider();

            var commandregistry = dependency.resolve("epi.globalcommandregistry");
            var area = "epi.cms.globalToolbar";
            commandregistry.registerProvider(area, commandsProvider);

            onPageEditingInitializer.initialize();
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "alloy.reviews", storeName: name });
        }
    });
});
