const path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("./webpack.config.common")(env, argv);
    const BundleAnalyzerPlugin = require("webpack-bundle-analyzer").BundleAnalyzerPlugin;

    webpackCommon.entry = "./review-component-widget/review-component-widget.tsx";

    webpackCommon.output = {
        filename: "ReviewWidget.js",
        libraryTarget: "amd",
        libraryExport: "default",
        path: path.resolve(__dirname, "../src/Alloy.Mvc.Template/modules/_protected/alloy.Reviews/1.0.0/Scripts")
    };

    if (!webpackCommon.plugins) {
        webpackCommon.plugins = [];
        webpackCommon.plugins.push(new BundleAnalyzerPlugin({ analyzerMode: "disabled" }));
    }

    webpackCommon.externals = [
        "dojo/_base/declare",
        "dijit/_WidgetBase",
        "epi/i18n!epi/cms/nls/reviewcomponent",
        "epi-cms/ApplicationSettings",
        "epi-cms/_ContentContextMixin",
        "alloy-review/advancedReviewService"
    ];
    return webpackCommon;
};
