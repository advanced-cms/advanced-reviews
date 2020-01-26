define([
    "dojo/_base/declare",
    "dojo/Deferred",
    "dojo/Stateful",
    "dojo/when",
    "epi/dependency",
    "epi-cms/ApplicationSettings",
    "epi-cms/_ContentContextMixin"
], function (
    declare,
    Deferred,
    Stateful,
    when,
    dependency,
    ApplicationSettings,
    _ContentContextMixin
) {

    return declare([Stateful, _ContentContextMixin], {
        postscript: function () {
            this.inherited(arguments);

            var registry = dependency.resolve("epi.storeregistry");
            this.externalReviewStore = this.contentStore || registry.get("externalreviews");
        },

        add: function (isEditable) {
            return this._handleContentAction(function (id, type) {
                var externalLink = {
                    isEditable: isEditable
                };
                if (type === "epi.cms.contentdata") {
                    externalLink.contentLink = id;
                } else {
                    externalLink.contentLink = ApplicationSettings.startPage.toString();
                }
                return this.externalReviewStore.put(externalLink);
            }.bind(this));
        },

        edit: function (token, validTo, pinCode, displayName) {
            return this.externalReviewStore.executeMethod("Edit", token, { validTo: validTo, pinCode: pinCode, displayName: displayName });
        },

        load: function () {
            return this._handleContentAction(function (id, type) {
                var data = type === "epi.cms.project" ? {projectId: id} : {id: id};
                return this.externalReviewStore.query(data);
            }.bind(this));
        },

        delete: function (token) {
            return this.externalReviewStore.remove(token);
        },

        share: function (token, email, subject, message) {
            return this.externalReviewStore.executeMethod("ShareReviewLink", token,
                { email: email, subject: subject, message: message});
        },

        _handleContentAction: function (action) {
            return when(this.getCurrentContext()).then(function (context) {
                if (!context) {
                    var def = new Deferred();
                    def.resolve(null);
                    return def.promise;
                }

                return action(context.id, context.type);
            }.bind(this));
        }
    });
});
