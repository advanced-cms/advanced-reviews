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
            this.reviewStore = this.contentStore || registry.get("approvaladvancedreview");
        },

        add: function (id, reviewLocation) {
            return this._handleContentAction(function (contentLink) {
                return this.reviewStore.put({
                    contentLink: contentLink,
                    reviewLocation: {
                        id: id,
                        data: JSON.stringify(reviewLocation)
            }
                });
            }.bind(this));
        },

        load: function () {
            return this._handleContentAction(function (contentLink) {
                return this.reviewStore.get(contentLink);
            }.bind(this));
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
