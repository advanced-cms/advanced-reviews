var path = require("path");
var webpackCommon = require("../webpack.config.common");

/* webpackCommon.entry = "./reviewComponent.tsx";
webpackCommon.output = {
  path: path.resolve(__dirname, ".\\dist"),
  filename: "reviewComponent.bundle.js"
}; */

webpackCommon.devtool = "eval-source-map";

//webpackCommon.plugins = [htmlPlugin];

//module.exports = webpackCommon;

module.exports = async ({ config, mode }) => {
  config.devtool = "eval-source-map",
  config.module.rules = [

    ...(config.resolve.rules || []),
    ...webpackCommon.module.rules

  ];
  config.resolve.extensions.push('.ts', '.tsx');


  config.resolve.modules = [
    ...(config.resolve.modules || []),
    path.resolve('./'),
  ];

  return config;
};