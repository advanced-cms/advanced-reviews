define([
    "dojo/on",
    "epi-cms/contentediting/OnPageEditing",
    "alloy-review/pointBuilder"
], function (
    on,
    OnPageEditing,
    pointBuilder) {
    
    function initialize() {
        //
        // postMixInProperties
        //
		var originalPostMixinProperties = OnPageEditing.prototype.postMixInProperties;
	    OnPageEditing.prototype.postMixInProperties = function () {
            originalPostMixinProperties.apply(this, arguments);

	        this._createReviewOverlay = _createReviewOverlay.bind(this);
            this._updateReviewPaneVisibility = _updateReviewPaneVisibility.bind(this);

	        this._createReviewOverlay();
	    };

        //
        // onReadySetupEditMode
        //
        var originalOnReadySetupEditMode = OnPageEditing.prototype.onReadySetupEditMode;
	    OnPageEditing.prototype.onReadySetupEditMode = function (doc) {
            originalOnReadySetupEditMode.apply(this, arguments);
            this._doc = doc;
	        this._updateReviewPaneVisibility(doc);
	    };

        //
        // destroy
        //
        var originalDestroy = OnPageEditing.prototype.destroy;
        OnPageEditing.prototype.destroy = function () {
            this._doc = null;
            originalDestroy.apply(this, arguments);
        };

        function _updateReviewPaneVisibility(doc) {
            function isReviewPanelVisible(viewModel) {
                if (!doc) {
                    return false;
                }
                if (viewModel.canChangeContent()) {
                    return false;
                }
                return true;
            }

            //TODO: add object mutators on those classes
            this._reviewOverlay.classList.toggle("dijitHidden", !isReviewPanelVisible(this.viewModel));
            setTimeout(function () {
                this._reviewOverlayDocumentArea.style.height = this.iFrame.domNode.offsetHeight + "px";
            }.bind(this), 2000);
            this._reviewOverlay.style.height = this._reviewOverlay.parentElement.style.height;
            this._reviewOverlay.style.maxHeight = this._reviewOverlay.parentElement.style.height;
        }

        function _createReviewOverlay () {
            this._reviewOverlay = document.createElement("div");
            this._reviewOverlay.classList.add("review-overlay", "dijitHidden");

            this._reviewOverlayDocumentArea = document.createElement("div");
            this._reviewOverlayDocumentArea.classList.add("review-overlay-document-area");
            this._reviewOverlay.appendChild(this._reviewOverlayDocumentArea);

            this.editLayoutContainer.domNode.appendChild(this._reviewOverlay);

            this.own(on(this._reviewOverlay, "click", function (e) {
                var el = this._doc.elementFromPoint(e.offsetX, e.offsetY);
                var point = pointBuilder.create(el);

                var pointEl = document.createElement("div");
                pointEl.classList.add("review-location");
                pointEl.style.top = (e.offsetY -12) + "px";
                pointEl.style.left = (e.offsetX - 12) + "px";
                on(pointEl, "click", function (e) {
                    e.stopPropagation();
                    alert(e.srcElement.innerHTML);
                });
                if (!window.globalIndex) {
                    window.globalIndex = 1;
                }
                pointEl.innerHTML = window.globalIndex++;
                this._reviewOverlayDocumentArea.appendChild(pointEl);

            }.bind(this)));

            this.own(on(this._reviewOverlay, "scroll", function (e) {
                // get element with scroll
                var previewContainer = this.iFrame.domNode.parentNode;
                previewContainer.scrollTop = e.srcElement.scrollTop;
            }.bind(this)));
        }
	}

    return {
        initialize: initialize
    };
});
