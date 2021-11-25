define([
    "dojo/_base/declare",
    "epi/dependency",
    "epi/routes",
    "epi/shell/store/JsonRest",
    "epi/shell/store/Throttle",
    "epi/_Module",
    "advanced-cms-approval-reviews/commandsProvider",
    "advanced-cms-approval-reviews/onPageEditingInitializer",
    "advanced-cms-approval-reviews/approveChangesInitializer",
    "advanced-cms-approval-reviews/rejectChangesInitializer",
    "advanced-cms-approval-reviews/notificationsInitializer"
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
    rejectChangesInitializer,
    notificationsInitializer
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
            registry.add("approvaladvancedreview",
                new Throttle(
                    new JsonRest({
                        preventCache: true,
                        target: this._getRestPath("approvaladvancedreview")//,
                        //idProperty: "contentLink"
                    })
                )
            );

            var commandsProvider = new CommandsProvider();

            var commandregistry = dependency.resolve("epi.globalcommandregistry");
            var area = "epi.cms.globalToolbar";
            commandregistry.registerProvider(area, commandsProvider);

            var language = this._settings.language || "";
            var avatarUrl = this._settings.avatarUrl || "";
            onPageEditingInitializer.initialize(options, language, avatarUrl);
            approveChangesInitializer.initialize();
            rejectChangesInitializer.initialize();
            notificationsInitializer.initialize();
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "advanced-cms-approval-reviews", storeName: name });
        }
    });
});
