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

        constructor: function () {
            this._toggleCanExecute();
        },

        contextChanged: function () {
            this.set("active", false);
            this._toggleCanExecute();
            topic.publish("reviews:toggle", this.active);
        },

        itemChanged: function (id, content) {
            this._refreshContent(content);
        },

        _refreshContent: function (content) {
            this.set({
                isAvailable: true,
                canExecute: content.status !== ContentActionSupport.versionStatus.Published
            });
        },

        _activeSetter: function (active) {
            this.active = active;
            this.set('label', active ? res.labelenabled : res.labelnotenabled);
        },

        _execute: function () {
            this.set("active", !this.active);
            when(editorDisplayLanguageResolver.resolve()).then(function (language) {
                topic.publish("reviews:toggle", this.active, language);
            }.bind(this));
        },

        _toggleCanExecute: function () {
            return when(this.getCurrentContext()).then(function (context) {
                if (context.type === "epi.cms.contentdata") {
                    return when(this.getCurrentContent()).then(function (content) {
                        this._refreshContent(content);
                    }.bind(this));
                } else {
                    this.set({
                        isAvailable: false
                    });
                }

            }.bind(this));
        }
    });
});
