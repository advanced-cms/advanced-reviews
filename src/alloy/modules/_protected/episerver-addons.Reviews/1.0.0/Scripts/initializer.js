define([
    "dojo/_base/declare",
    "epi/dependency",
    "epi/_Module",
    "alloy-review/commandsProvider",
    "alloy-review/onPageEditingInitializer"
], function (
    declare,
    dependency,
    _Module,
    CommandsProvider,
    onPageEditingInitializer

) {
    return declare([_Module], {
        initialize: function () {
            this.inherited(arguments);

            var registry = this.resolveDependency("epi.storeregistry");
/*
            //Register store
            registry.add("alloy.favouriteContentStore",
                new Throttle(
                    new JsonRest({
                        target: this._getRestPath("favouriteContentStore"),
                        idProperty: "contentLink"
                    })
                )
            );*/

            var commandsProvider = new CommandsProvider();

            var commandregistry = dependency.resolve("epi.globalcommandregistry");
            var area = "epi.cms.globalToolbar";
            commandregistry.registerProvider(area, commandsProvider);

            onPageEditingInitializer.initialize();
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "App", storeName: name });
        }
    });
});
