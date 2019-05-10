define([
    "dojo/_base/declare",
    "dojo/Deferred",
    "dojo/Stateful",
    "dojo/when",
    "epi/dependency",
    "epi-cms/_ContentContextMixin"
], function (
    declare,
    Deferred,
    Stateful,
    when,
    dependency,
    _ContentContextMixin
) {

    return declare([Stateful, _ContentContextMixin], {
        postscript: function () {
            this.inherited(arguments);

            var registry = dependency.resolve("epi.storeregistry");
            this.externalReviewStore = this.contentStore || registry.get("externalreviews");
        },

        add: function (isEditable) {
            return this._handleContentAction(function (contentLink) {
                return this.externalReviewStore.put({
                    contentLink: contentLink,
                    isEditable: isEditable
                });
            }.bind(this));
        },

        load: function () {
            return this._handleContentAction(function (contentLink) {
                return this.externalReviewStore.get(contentLink);
            }.bind(this));
        },

        delete: function (token) {
            return this.externalReviewStore.remove(token);
        },

        share: function (token, email, message) {
            this.externalReviewStore.executeMethod("ShareReviewLink", token,
                { email: email, message: message});
        },

        _handleContentAction: function (action) {
            return when(this.getCurrentContent()).then(function (content) {
                if (!content) {
                    var def = new Deferred();
                    def.resolve(null);
                    return def.promise;
                }

                return action(content.contentLink);
            }.bind(this));
        }
    });
});
