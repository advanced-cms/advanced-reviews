import { defineConfig } from "vite";
import viteConfig from "./vite.config";

viteConfig.build.rollupOptions.input = "./src/admin/admin-plugin.tsx";
export default defineConfig(viteConfig as any);
