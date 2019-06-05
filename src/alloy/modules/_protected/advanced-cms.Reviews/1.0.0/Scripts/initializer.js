define([
    "dojo/_base/declare",
    "epi/dependency",
    "epi/routes",
    "epi/shell/store/JsonRest",
    "epi/shell/store/Throttle",
    "epi/_Module",
    "episerver-addons-review/commandsProvider",
    "episerver-addons-review/onPageEditingInitializer",
    "episerver-addons-review/approveChangesInitializer",
    "episerver-addons-review/rejectChangesInitializer"
], function (
    declare,
    dependency,
    routes,
    JsonRest,
    Throttle,
    _Module,
    CommandsProvider,
    onPageEditingInitializer,
    approveChangesInitializer,
    rejectChangesInitializer
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
            registry.add("approvallanguage",
                new Throttle(
                    new JsonRest({
                        target: this._getRestPath("approvallanguage")//,
                        //idProperty: "contentLink"
                    })
                )
            );

            var commandsProvider = new CommandsProvider();

            var commandregistry = dependency.resolve("epi.globalcommandregistry");
            var area = "epi.cms.globalToolbar";
            commandregistry.registerProvider(area, commandsProvider);

            onPageEditingInitializer.initialize();
            approveChangesInitializer.initialize();
            rejectChangesInitializer.initialize();
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "episerver-addons.reviews", storeName: name });
        }
    });
});
