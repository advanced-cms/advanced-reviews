define([
    "dojo/_base/declare",
    "dojo/topic",
    "dojo/when",
    "epi/shell/command/_Command",
    "epi-cms/_ContentContextMixin",
    "epi-cms/contentediting/ContentActionSupport",
    "episerver-addons-review/editorDisplayLanguageResolver"
], function (
    declare,
    topic,
    when,
    _Command,
    _ContentContextMixin,
    ContentActionSupport,
    editorDisplayLanguageResolver
) {
    return declare([_Command, _ContentContextMixin], {
        name: "ContentReferences",
        label: "Advanced Review",
        tooltip: "Toggle advanced review",
        iconClass: 'epi-icon--medium epi-review-icon',
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
            this._publishTopic();
        },

        _execute: function () {
            this.reviewEnabled = !this.reviewEnabled;
            this._publishTopic();
        },

        _publishTopic: function () {
            when(editorDisplayLanguageResolver.resolve()).then(function (language) {
                topic.publish("toggle:reviews", this.reviewEnabled, language);
            }.bind(this));
        },

        _toggleCanExecute: function () {
            return when(this.getCurrentContent()).then(function (content) {
                this.set({ canExecute: content.status !== ContentActionSupport.versionStatus.Published });
            }.bind(this));
        }
    });
});
