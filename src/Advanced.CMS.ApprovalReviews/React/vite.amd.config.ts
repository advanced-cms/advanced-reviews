import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";
import eslint from "vite-plugin-eslint";
import externalize from "vite-plugin-externalize-dependencies";
import tsconfigPaths from "vite-tsconfig-paths";

const isWatch = process.argv.includes("--watch");
const isProductionBuild = process.argv.includes("build") && !isWatch;

const ignoredAmdDependencies = [
    "dojo/",
    "dijit/",
    "epi/",
    "epi-cms/",
    "episerver-cms-ui-react/",
    "advanced-cms-approval-reviews",
];

const plugins = [
    react(),
    externalize({
        externals: [
            (moduleName) => ignoredAmdDependencies.some((ignoredPattern) => moduleName.startsWith(ignoredPattern)),
        ],
    }),
    tsconfigPaths(),
    eslint({
        fix: !isProductionBuild,
        emitError: !isWatch,
        failOnError: !isWatch,
        exclude: ["**/node_modules/**", "**/React/src/**"],
    }),
];

// https://vitejs.dev/config/
export default defineConfig({
    plugins: plugins,
    build: {
        chunkSizeWarningLimit: 5000,
        outDir: "../ClientResources/epi-cms-react/components",
        rollupOptions: {
            preserveEntrySignatures: "strict",
            input: {
                "content-manager-lite-widget": "src/components/content-manager-lite-widget/content-manager-lite-widget.tsx",
            },
            output: {
                manualChunks: (id) => {
                    if (id.includes('node_modules')) {
                        return 'vendor'; // All node_modules go into vendor.bundle.js
                    }
                },
                entryFileNames: `[name].js`,
                chunkFileNames: `[name]_2.js`,
                assetFileNames: `[name].[ext]`,
                format: "amd",
            },
            external: [
                "dojo/_base/declare",
                "dijit/_WidgetBase",
                "epi/i18n!epi/cms/nls/reviewcomponent",
                "epi-cms/ApplicationSettings",
                "epi-cms/_ContentContextMixin",
                "advanced-cms-approval-reviews/advancedReviewService",
            ],
        },
        minify: true,
        emptyOutDir: true,
        sourcemap: true,
    }
});
