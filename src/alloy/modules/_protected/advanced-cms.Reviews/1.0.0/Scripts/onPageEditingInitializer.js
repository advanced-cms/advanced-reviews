define([
    "dojo/topic",
    "dojo/dom-class",
    "epi-cms/contentediting/OnPageEditing",
    "alloy-review/ReviewWidget"
], function (
    topic,
    domClass,
    OnPageEditing,
    ReviewWidget) {

    var reviewWidget = null;

    function toggleReviewOverlay(value) {
        if (reviewWidget) {
            domClass.toggle(reviewWidget.domNode, "dijitHidden", !value);
        }
    }

    function initialize() {
        topic.subscribe("toggle:reviews", function (toggle, language) {
            if (!reviewWidget) {
                var div = document.createElement("div");
                var iframe = document.getElementsByName("sitePreview")[0];
                reviewWidget = new ReviewWidget({ iframe: iframe, language: language });
                reviewWidget.placeAt(div);
                var editLayoutContainer = document.getElementsByClassName("epi-editorViewport")[0];
                editLayoutContainer.appendChild(div);
            }
            toggleReviewOverlay(toggle);
        });

        var originalOnReadySetupEditMode = OnPageEditing.prototype.onReadySetupEditMode;
        OnPageEditing.prototype.onReadySetupEditMode = function (doc) {
            originalOnReadySetupEditMode.apply(this, arguments);
            toggleReviewOverlay(false);
        };

        var originalDestroy = OnPageEditing.prototype.destroy;
        OnPageEditing.prototype.destroy = function () {
            originalDestroy.apply(this, arguments);
            if (reviewWidget) {
                reviewWidget.destroy();
                reviewWidget = null;
            }
        };
    }

    return {
        initialize: initialize
    };
});
