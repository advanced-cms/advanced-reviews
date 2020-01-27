define([
    "dojo/topic",
    "dojo/dom-class",
    "epi-cms/contentediting/OnPageEditing",
    "advanced-cms-review/ReviewWidget"
], function (
    topic,
    domClass,
    OnPageEditing,
    ReviewWidget) {

    var reviewWidget = null;

    var latestPropertyNameMappings = [];

    function toggleReviewOverlay(value) {
        if (reviewWidget) {
            domClass.toggle(reviewWidget.domNode, "dijitHidden", !value);
        }
    }

    function getDisplayNamesDictionary(editableNodes) {
        if (!editableNodes) {
            return {};
        }

        var result = {};
        editableNodes.filter(function (x) {
            return x.property && x.property.metadata;
        }).forEach(function (x) {
            // epi attributes are capital case but property names are lowercase
            var key = x.property.propertyNodeName[0].toUpperCase() + x.property.propertyNodeName.substring(1);
            result[key] = x.property.metadata.displayName;
            });
        return result;
    }

    function initialize() {
        topic.subscribe("reviews:toggle", function (toggle, language) {
            if (!reviewWidget) {
                if (toggle) {
                    var div = document.createElement("div");
                    var iframe = document.getElementsByName("sitePreview")[0];
                    reviewWidget = new ReviewWidget({
                        iframe: iframe,
                        language: language,
                        propertyNameMapping: latestPropertyNameMappings
                    });
                    reviewWidget.placeAt(div);
                    var editLayoutContainer = document.getElementsByClassName("epi-editorViewport")[0];
                    editLayoutContainer.appendChild(div);
                }
            }
            else {
                reviewWidget.loadPins();
            }

            toggleReviewOverlay(toggle);
        });

        var originalOnReadySetupEditMode = OnPageEditing.prototype.onReadySetupEditMode;
        OnPageEditing.prototype.onReadySetupEditMode = function (doc) {
            originalOnReadySetupEditMode.apply(this, arguments);
            toggleReviewOverlay(false);
        };

        var originalCreateUpdateControllers = OnPageEditing.prototype._createUpdateControllers;
        OnPageEditing.prototype._createUpdateControllers = function (doc) {
            originalCreateUpdateControllers.apply(this, arguments);

            // create dictionary for names and display names
            var editableNodes = this._getEditableNodes(doc);
            latestPropertyNameMappings = getDisplayNamesDictionary(editableNodes);
            if (reviewWidget) {
                reviewWidget.updateDisplayNamesDictionary(latestPropertyNameMappings);
            }
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
