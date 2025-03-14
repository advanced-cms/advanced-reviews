import { defineConfig } from "vite";
import viteConfig from "./vite.amd.config";
viteConfig.build.rollupOptions.input = "./src/external-reviews-manage-links/external-review-manage-links-widget.tsx";
export default defineConfig(viteConfig as any);
