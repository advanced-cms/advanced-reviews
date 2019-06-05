const path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("../webpack.config.common")(env, argv);
    webpackCommon.entry = "./admin/admin-plugin.tsx";
    webpackCommon.output = {
        filename: "adminPlugin.js",
        //libraryTarget: "amd",
        //libraryExport: "default",
        path: path.resolve(__dirname, "../../alloy/modules/_protected/episerver-addons.Reviews/Views/admin")
    };
    return webpackCommon;
};
