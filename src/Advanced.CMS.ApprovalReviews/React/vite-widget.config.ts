import { defineConfig } from "vite";
import viteConfig from "./vite.amd.config";
viteConfig.build.rollupOptions.input = "./src/review-component-widget/review-component-widget.tsx";
export default defineConfig(viteConfig as any);
