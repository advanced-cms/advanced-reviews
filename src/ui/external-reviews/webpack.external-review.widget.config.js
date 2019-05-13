const path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("../webpack.config.common")(env, argv);

    webpackCommon.entry = "./external-reviews/external-review-widget.tsx";

    webpackCommon.output = {
        filename: "external-review-component.js",
        libraryTarget: "amd",
        libraryExport: "default",
        path: path.resolve(
            __dirname,
            "../../src/Alloy.Mvc.Template/modules/_protected/alloy.ExternalReviews/1.0.0/Scripts"
        )
    };

    webpackCommon.externals = [
        "dojo/_base/declare",
        "dijit/_WidgetBase",
        //"epi/i18n!epi/cms/nls/reviewcomponent",
        //"epi-cms/ApplicationSettings",
        "epi-cms/_ContentContextMixin",
        "alloy-external-review/external-review-service"
    ];

    return webpackCommon;
};
