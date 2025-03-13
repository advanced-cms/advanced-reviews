import { defineConfig } from "vite";
const path = require("path");
import viteConfig from "./vite.config";

viteConfig.build.rollupOptions.input = "./src/editable-external-reviews/editable-external-review-component.tsx";
viteConfig.build.rollupOptions.output.dir = path.resolve(__dirname, "../../Advanced.CMS.ExternalReviews/ClientResources/dist");
export default defineConfig(viteConfig as any);
