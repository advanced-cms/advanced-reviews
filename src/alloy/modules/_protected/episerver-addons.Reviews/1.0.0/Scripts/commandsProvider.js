define([
    "dojo/_base/declare",
    "dijit/form/ToggleButton",
    "epi-cms/component/command/_GlobalToolbarCommandProvider",
    "alloy-review/ShowReviewCommand"
], function (
    declare,
    ToggleButton,
    _GlobalToolbarCommandProvider,
    ShowReviewCommand) {
    return declare([_GlobalToolbarCommandProvider], {

        constructor: function () {
            this.inherited(arguments);

            var showReviewCommand = new ShowReviewCommand({
                //label: "First command"
            });
            this.addToLeading(showReviewCommand,
                {
                    showLabel: false,
                    widget: ToggleButton,
                    'class': 'favourite-button'
                    //'epi-disabledDropdownArrow epi-groupedButtonContainer'//'epi-leadingToggleButton epi-disabledDropdownArrow dijitDropDownButton' // dijitChecked
                });
        }
    });
});
