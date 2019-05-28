define([
    "epi/dependency"
], function (dependency) {
    var editorLanguage;

    return {
        resolve: function () {
            if (editorLanguage) {
                return editorLanguage;
            }

            var registry = dependency.resolve("epi.storeregistry");
            var languageStore = registry.get("approvallanguage");

            return languageStore.get().then(function (language) {
                return editorLanguage = language;
            });
        }
    }
});
