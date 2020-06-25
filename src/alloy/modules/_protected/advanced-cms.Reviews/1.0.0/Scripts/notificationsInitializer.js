define([
    "dojo/topic",
    "dojo/when",
    "epi/dependency",
    "epi/shell/StickyViewSelector",
    "epi-cms/notification/viewmodels/NotificationListViewModel",
    "epi/shell/TypeDescriptorManager"
], function (
    topic,
    when,
    dependency,
    StickyViewSelector,
    NotificationListViewModel,
    TypeDescriptorManager
) {
    function initialize() {

        // get available views for content type
        function getAvailableViews(dataType) {
            var availableViews = TypeDescriptorManager.getAndConcatenateValues(dataType, "availableViews") || [],
                disabledViews = TypeDescriptorManager.getValue(dataType, "disabledViews") || [];

            var filteredViews = availableViews.filter(function (availableView) {
                return disabledViews.every(function (disabledView) {
                    return availableView.key !== disabledView;
                });
            });
            return filteredViews.map(function (x) { return x.key; });
        }


        //
        // override original Notification item click and when external review is detected,
        // then automatically turn on reviews mode
        //
        var original = NotificationListViewModel.prototype.gotoSelectedNotificationOrigin;
        NotificationListViewModel.prototype.gotoSelectedNotificationOrigin = function () {
            if (this.selectedNotification.content.indexOf("external-review'")) {
                // subscribe to context changed event once, and then turn on project mode
                var handle = topic.subscribe("/epi/shell/context/changed", function () {
                    handle.remove();
                    handle = null;

                    // wait one second until OPE is loaded and then turn on reviews
                    setTimeout(function () {
                        topic.publish("reviews:force-review-mode");
                    }.bind(this), 1000);
                });

                setTimeout(function () {
                    if (handle) {
                        handle.remove();
                    }
                }, 2000);

                // parse contentLink from URL parameter
                var id = this.selectedNotification.link.substring(this.selectedNotification.link.indexOf(":///") + 4);

                var registry = dependency.resolve("epi.storeregistry");
                var store = registry.get("epi.cms.contentdata");

                // get content and check if OPE is in available views
                when(store.get(id)).then(function (contentData) {
                    var availableViews = getAvailableViews(contentData.typeIdentifier);
                    if (availableViews.indexOf("onpageedit") === -1) {
                        original.apply(this, arguments);
                    }

                    // use StickyViewSelector, to force loading OPE
                    var typeIdentifier = contentData.typeIdentifier;
                    var stickyViewSelector = new StickyViewSelector();
                    stickyViewSelector.save(true, typeIdentifier, "onpageedit");

                    // call original method to load new context
                    original.apply(this, arguments);
                }.bind(this));
            } else {
                original.apply(this, arguments);
            }
        };

        NotificationListViewModel.prototype.gotoSelectedNotificationOrigin.nom = "gotoSelectedNotificationOrigin";
    }

    return {
        initialize: initialize
    };
});
