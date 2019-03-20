var path = require("path");
var webpackCommon = require("./webpack.config.common");
const HtmlWebPackPlugin = require("html-webpack-plugin");

const htmlPlugin = new HtmlWebPackPlugin({
  template: "./src/index.html",
  filename: "./index.html"
});

webpackCommon.entry = "./reviewComponent.tsx";
webpackCommon.output = {
  path: path.resolve(__dirname, ".\\dist"),
  filename: "reviewComponent.bundle.js"
};

webpackCommon.devtool = "eval-source-map";

webpackCommon.plugins = [htmlPlugin];

module.exports = webpackCommon;