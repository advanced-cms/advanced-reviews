define([
    "dojo/Deferred",
    "dojo/on",
    "epi/shell/widget/dialog/Dialog",
    "epi-cms/content-approval/command/ApproveChanges",
    "advanced-cms-review/advancedReviewService"
], function (
    Deferred,
    on,
    Dialog,
    ApproveChanges,
    AdvancedReviewService) {


    function initialize() {

        // when there are open reviews, then paragraph with link is returned
        ApproveChanges.prototype._getErrorElement = function () {
            var def = new Deferred();

            if (!this._advancedReviewService) {
                this._advancedReviewService = new AdvancedReviewService();
                this.own(this._advancedReviewService);
            }

            this._advancedReviewService.load().then(function (reviewLocations) {
                if (reviewLocations.length === 0) {
                    def.resolve(null);
                    return;
                }

                var filteredLocations = reviewLocations
                    .map(function (x) {
                        return x.data;
                    })
                    .filter(function (x) {
                        return !x.isDone;
                    });

                if (filteredLocations.length === 0) {
                    def.resolve(null);
                    return;
                }

                var reviewInfo = document.createElement("p");
                reviewInfo.innerText =
                    "There are unresolved review issues. Please verify them before approving the content.";
                var actionLink = document.createElement("a");
                actionLink.setAttribute("href", "#");
                actionLink.innerText = "Open reviews";
                actionLink.classList.add("dijitVisible", "epi-visibleLink");
                actionLink.onclick = function () {
                    this._dialog.hide();
                    this._advancedReviewService.setReviewContext();
                }.bind(this);
                reviewInfo.appendChild(actionLink);

                def.resolve(reviewInfo);
            }.bind(this));

            return def.promise;
        };

        /**
         * override showDialog
         */
        var originalShowDialog = ApproveChanges.prototype.showDialog;
        ApproveChanges.prototype.showDialog = function () {
            this._dialogDisplayed = true;

            originalShowDialog.apply(this, arguments);

            this._getErrorElement().then(function (reviewInfo) {
                if (reviewInfo) {
                    this.dialogContent.domNode.parentElement.insertBefore(reviewInfo, this.dialogContent.domNode);
                }
            }.bind(this));
        };

        /**
         * override _execute
         */
        var originalExecute = ApproveChanges.prototype._execute;
        ApproveChanges.prototype._execute = function () {
            // set _dialogDisplayed to false. When 'showDialog' is not calle and reset the flag
            // then custom warning is displayed
            this._dialogDisplayed = false;


            originalExecute.apply(this, arguments);
        };

        /**
         * override _executeServiceMethod
         */
        var originalExecuteServiceMethod = ApproveChanges.prototype._executeServiceMethod;
        ApproveChanges.prototype._executeServiceMethod = function () {
            if (this._dialogDisplayed) {
                return originalExecuteServiceMethod.apply(this, arguments);
            } else {
                return this._getErrorElement().then(function (reviewInfo) {
                    if (reviewInfo) {
                        var deferred = new Deferred();
                        this._dialog = new Dialog({
                            content: reviewInfo,
                            title: "Approve content",
                            dialogClass: "epi-dialog-confirm"
                        });
                        on.once(this._dialog, "execute", function (value) {
                            originalExecuteServiceMethod.apply(this, arguments).then(function (result) {
                                deferred.resolve(result);
                            });
                        }.bind(this));
                        this._dialog.show();
                        return deferred.promise;
                    } else {
                        return originalExecuteServiceMethod.apply(this, arguments);
                    }
                }.bind(this));
            }
        };
    }

    return {
        initialize: initialize
    };
});
