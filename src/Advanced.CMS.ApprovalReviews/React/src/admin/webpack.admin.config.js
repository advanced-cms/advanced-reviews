const path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("../../webpack.config.common")(env, argv);
    webpackCommon.entry = "./src/admin/admin-plugin.tsx";
    webpackCommon.output = {
        filename: "adminPlugin.js",
        path: path.resolve(__dirname, "../../../ClientResources")
    };
    return webpackCommon;
};
