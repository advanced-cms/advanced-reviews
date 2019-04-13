var path = require("path");
var webpackCommon = require("./webpack.config.common");

webpackCommon.entry = "./reviewComponentWidget.tsx";

webpackCommon.output = {
    filename: "ReviewWidget.js",
    libraryTarget: "amd",
    libraryExport: "default",
    path: path.resolve(__dirname, "../src/Alloy.Mvc.Template/modules/_protected/alloy.Reviews/1.0.0/Scripts")
};

webpackCommon.externals = [
    "dojo/_base/declare",
    "dijit/_WidgetBase",
    "epi/i18n!epi/cms/nls/reviewcomponent",
    "dojo/topic",
    "epi-cms/ApplicationSettings",
    "epi-cms/_ContentContextMixin",
    "alloy-review/advancedReviewService"
];

module.exports = webpackCommon;

