define([
    "epi-cms/content-approval/command/RejectChanges",
    "advanced-cms-review/advancedReviewService"
], function (
    RejectChanges,
    AdvancedReviewService) {


        function initialize() {
            var originalShowDialog = RejectChanges.prototype.showDialog;
            RejectChanges.prototype.showDialog = function () {
                originalShowDialog.apply(this, arguments);

                if (!this._advancedReviewService) {
                    this._advancedReviewService = new AdvancedReviewService();
                }

                this._advancedReviewService.load().then(function (reviewLocations) {
                    if (reviewLocations.length === 0) {
                        return;
                    }

                    var filteredLocations = reviewLocations
                        .map(function (x) {
                            return x.data;
                        })
                        .filter(function (x) {
                            return !x.isDone && x.firstComment && x.firstComment.text;
                        })
                        .map(function (x) {
                            return x.firstComment.text;
                        });

                    if (filteredLocations.length === 0) {
                        return;
                    }

                    var text = "- " + filteredLocations.join("\n- ");
                    text = "Issues to be solved before publish:\n" + text;

                    this.dialogContent.set("value", text);
                }.bind(this));
            };
        }

        return {
            initialize: initialize
        };
    });
