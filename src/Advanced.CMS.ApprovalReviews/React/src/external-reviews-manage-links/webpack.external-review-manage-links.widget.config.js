const path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("../../webpack.config.common")(env, argv);

    webpackCommon.entry = "./src/external-reviews-manage-links/external-review-manage-links-widget.tsx";

    webpackCommon.output = {
        filename: "external-review-manage-links-component.js",
        libraryTarget: "amd",
        libraryExport: "default",
        path: path.resolve(__dirname, "../../../ClientResources")
    };

    webpackCommon.externals = [
        "dojo/_base/declare",
        "dijit/_WidgetBase",
        "epi/i18n!epi/cms/nls/externalreviews",
        "epi-cms/_ContentContextMixin",
        "advanced-cms-external-reviews/external-review-service"
    ];

    return webpackCommon;
};
