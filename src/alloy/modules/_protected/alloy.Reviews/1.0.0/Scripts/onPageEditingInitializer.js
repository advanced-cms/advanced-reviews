define([
    "epi-cms/contentediting/OnPageEditing",
    "alloy-review/ReviewWidget"
], function (
    OnPageEditing,
    ReviewWidget) {

    function initialize() {
		var originalPostMixinProperties = OnPageEditing.prototype.postMixInProperties;
	    OnPageEditing.prototype.postMixInProperties = function () {
            originalPostMixinProperties.apply(this, arguments);

            var div = document.createElement("div");
            var widget = new ReviewWidget({ iframe:this.iFrame.domNode });
            widget.placeAt(div);
            this.editLayoutContainer.domNode.appendChild(div);
	    };
	}

    return {
        initialize: initialize
    };
});
