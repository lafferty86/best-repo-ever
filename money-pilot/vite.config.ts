import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// Money Pilot — React + TypeScript single-page app.
export default defineConfig({
  base: "./",
  plugins: [react()],
  build: { outDir: "dist", emptyOutDir: true },
  server: { port: 5173, host: true },
});
