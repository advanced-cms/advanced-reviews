var path = require("path");
var webpackCommon = require("../webpack.config.common");

webpackCommon.entry = "./admin/admin-plugin.tsx";

webpackCommon.output = {
    filename: "adminPlugin.js",
    //libraryTarget: "amd",
    //libraryExport: "default",
    path: path.resolve(__dirname, "../../src/Alloy.Mvc.Template/modules/_protected/alloy.Reviews/Views/admin")
};

webpackCommon.devtool = "eval-source-map";

module.exports = webpackCommon;
