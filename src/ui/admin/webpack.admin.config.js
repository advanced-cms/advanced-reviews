const path = require("path");

module.exports = (env, argv) => {
    const webpackCommon = require("../webpack.config.common")(env, argv);
    webpackCommon.entry = "./admin/admin-plugin.tsx";
    webpackCommon.output = {
        filename: "adminPlugin.js",
        //libraryTarget: "amd",
        //libraryExport: "default",
        path: path.resolve(__dirname, "../../src/Alloy.Mvc.Template/modules/_protected/alloy.Reviews/Views/admin")
    };
    return webpackCommon;
};
