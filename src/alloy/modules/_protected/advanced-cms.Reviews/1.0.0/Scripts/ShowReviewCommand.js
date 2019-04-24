define([
    "dojo/_base/declare",
    "dojo/topic",
    "dojo/when",
    "epi/shell/command/_Command",
    "epi-cms/_ContentContextMixin",
    "epi-cms/contentediting/ContentActionSupport",
], function (
    declare,
    topic,
    when,
    _Command,
    _ContentContextMixin,
    ContentActionSupport
) {
    return declare([_Command, _ContentContextMixin], {
        name: "ContentReferences",
        label: "Advanced Review",
        tooltip: "Toggle advanced review",
        iconClass: 'epi-icon--medium epi-iconBubble',
        canExecute: false,
        reviewEnabled: false,

        constructor: function () {
            this._toggleCanExecute();

            this.own(topic.subscribe("toggle:reviews", function (reviewEnabled) {
                this.set("active", reviewEnabled);
                this.set('label', reviewEnabled ? "Turn off Advanced Review" : "Turn on Advanced Review");
            }.bind(this)));
        },

        contentContextChanged: function () {
            this.reviewEnabled = false;
            this.set("active", false);
            this._toggleCanExecute();
            topic.publish("toggle:reviews", this.reviewEnabled, this._currentContext.language);
        },

        _execute: function () {
            this.reviewEnabled = !this.reviewEnabled;
            topic.publish("toggle:reviews", this.reviewEnabled, this._currentContext.language);
        },

        _toggleCanExecute: function () {
            return when(this.getCurrentContent()).then(function (content) {
                this.set({ canExecute: content.status !== ContentActionSupport.versionStatus.Published });
            }.bind(this));
        }
    });
});
