define([
    "dojo/_base/declare",
    "dijit/form/ToggleButton",
    "epi-cms/component/command/_GlobalToolbarCommandProvider",
    "episerver-addons-review/ShowReviewCommand"
], function (
    declare,
    ToggleButton,
    _GlobalToolbarCommandProvider,
    ShowReviewCommand) {
    return declare([_GlobalToolbarCommandProvider], {

        constructor: function () {
            this.inherited(arguments);

            var showReviewCommand = new ShowReviewCommand({ });
            this.addToLeading(showReviewCommand,
                {
                    showLabel: false,
                    widget: ToggleButton,
                    'class': 'epi-mediumButton epi-review-button'
                });
        }
    });
});
