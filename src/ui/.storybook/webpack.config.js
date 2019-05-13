module.exports = async ({config}) => {
    const webpackCommon = require("../webpack.config.common")(config, {});

    config.module.rules = [
        ...(config.resolve.rules || []),
        ...webpackCommon.module.rules
    ];
    config.resolve.extensions.push('.ts', '.tsx');

    return config;
};
