import { defineConfig } from "vite";

// Money Pilot is compiled from F# by Fable into ./build, then bundled by Vite.
export default defineConfig({
  root: ".",
  base: "./",
  build: {
    outDir: "dist",
    emptyOutDir: true,
  },
  server: {
    port: 5173,
    host: true,
  },
});
