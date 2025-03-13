var path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("./webpack.config.common")(env, argv);

    webpackCommon.entry = "./src/editable-external-reviews/editable-external-review-component.tsx";

    webpackCommon.output = {
        filename: "external-review-component.js",
        path: path.resolve(__dirname, "../../Advanced.CMS.ExternalReviews/ClientResources"),
    };

    return webpackCommon;
};
