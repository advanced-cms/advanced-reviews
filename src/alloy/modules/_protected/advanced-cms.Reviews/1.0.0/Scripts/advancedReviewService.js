define([
    "dojo/_base/declare",
    "dojo/Deferred",
    "dojo/Stateful",
    "dojo/topic",
    "dojo/when",
    "epi/dependency",
    "epi-cms/_ContentContextMixin",
    "advanced-cms-review/editorDisplayLanguageResolver"
], function (
    declare,
    Deferred,
    Stateful,
    topic,
    when,
    dependency,
    _ContentContextMixin,
    editorDisplayLanguageResolver
) {

    function parseResponse(reviewLocations) {
        return reviewLocations
            .map(function (x) {
                var reviewLocation;
                try {
                    reviewLocation = JSON.parse(x.data);
                } catch (exception) {
                    reviewLocation = null;
                }
                return {
                    id: x.id,
                    data: reviewLocation
                };
            })
            .filter(function (x) {
                return !!x.data
            });
    }

    return declare([Stateful, _ContentContextMixin], {
        postscript: function () {
            this.inherited(arguments);

            var registry = dependency.resolve("epi.storeregistry");
            this.reviewStore = registry.get("approvaladvancedreview");
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

        remove: function (id) {
            return this._handleContentAction(function (contentLink) {
                return this.reviewStore.remove(id + "?contentLink=" + contentLink);
            }.bind(this));
        },

        load: function () {
            return this._handleContentAction(function (contentLink) {
                return this.reviewStore.get(contentLink).then(parseResponse);
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
        },

        setReviewContext: function () {
            when(editorDisplayLanguageResolver.resolve()).then(function (language) {
                topic.publish("reviews:initialize", true, language);
            });
        }
    });
});
