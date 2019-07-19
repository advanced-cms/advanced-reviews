define([
    "dojo/_base/declare",
    "dojo/topic",
    "dojo/when",
    "epi/shell/command/_Command",
    "epi-cms/_ContentContextMixin",
    "epi-cms/contentediting/ContentActionSupport",
    "advanced-cms-review/editorDisplayLanguageResolver",
    "epi/i18n!epi/cms/nls/reviewcomponent.command"
], function (
    declare,
    topic,
    when,
    _Command,
    _ContentContextMixin,
    ContentActionSupport,
    editorDisplayLanguageResolver,
    res
) {
    return declare([_Command, _ContentContextMixin], {
        name: "ContentReferences",
        label: res.label,
        tooltip: res.tooltip,
        iconClass: 'epi-icon--medium epi-review-icon',
        canExecute: false,
        reviewEnabled: false,

        constructor: function () {
            this._toggleIsAvailable();

            this.own(topic.subscribe("toggle:reviews", function (reviewEnabled) {
                this.set("active", reviewEnabled);
                this.set('label', reviewEnabled ? res.labelenabled : res.labelnotenabled);
            }.bind(this)));
        },

        contextChanged: function () {
            this.reviewEnabled = false;
            this.set("active", false);
            this._toggleIsAvailable();
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

        _toggleIsAvailable: function () {
            return when(this.getCurrentContext()).then(function (context) {
                if (this._isContentContext(context)) {
                    this._toggleCanExecute();
                } else {
                    this.set({isAvailable: false});
                }
            }.bind(this));
        },

        _toggleCanExecute: function () {
            return when(this.getCurrentContent()).then(function (content) {
                this.set({ isAvailable: true, canExecute: content.status !== ContentActionSupport.versionStatus.Published });
            }.bind(this));
        }
    });
});
