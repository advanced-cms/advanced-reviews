import react from "@vitejs/plugin-react";
import eslint from "vite-plugin-eslint";
import { visualizer } from "rollup-plugin-visualizer";

const isWatch = process.argv.includes("--watch");
const isProductionBuild = process.argv.includes("build") && !isWatch;

const plugins = [react(), visualizer() as any];

const turnOffLinkBuildErrors = isWatch;
plugins.push(
    eslint({
        fix: !isProductionBuild,
        emitError: !turnOffLinkBuildErrors,
        failOnError: !turnOffLinkBuildErrors,
    }),
);

export default {
    plugins: plugins,
    build: {
        rollupOptions: {
            input: "",
            output: {
                entryFileNames: "[name].js",
                assetFileNames: `[name].[ext]`,
                dir: "../ClientResources/dist",
            },
        },
        sourcemap: true,
    },
    test: {
        globals: true,
        environment: "jsdom",
        setupFiles: "./setup.ts",
        css: false,
    },
};
