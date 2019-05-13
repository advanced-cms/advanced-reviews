var path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("../webpack.config.common")(env, argv);

    webpackCommon.entry = "./external-reviews/editable-external-review-component.tsx";

    webpackCommon.output = {
        filename: "external-review-component.js",
        //libraryTarget: "amd",
        //libraryExport: "default",
        path: path.resolve(__dirname, "../../src/Alloy.Mvc.Template/modules/_protected/alloy.ExternalReviews/Views")
    };
};
