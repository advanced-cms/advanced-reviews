const path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("./webpack.config.common")(env, argv);

    webpackCommon.entry = "./src/review-component-widget/review-component-widget.tsx";

    webpackCommon.output = {
        filename: "ReviewWidget.js",
        libraryTarget: "amd",
        libraryExport: "default",
        path: path.resolve(__dirname, "../ClientResources"),
    };

    webpackCommon.externals = [
        "dojo/_base/declare",
        "dijit/_WidgetBase",
        "epi/i18n!epi/cms/nls/reviewcomponent",
        "epi-cms/ApplicationSettings",
        "epi-cms/_ContentContextMixin",
        "advanced-cms-approval-reviews/advancedReviewService",
    ];
    return webpackCommon;
};
