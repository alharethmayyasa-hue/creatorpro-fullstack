import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    // Dev-only proxy: forwards /api/* to the ASP.NET backend so the browser
    // hits the same origin as Vite (avoids CORS without modifying the backend).
    proxy: {
      "/api": {
        target: "http://localhost:5128",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
