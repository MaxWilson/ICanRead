import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  root: "src",
  plugins: [react()],
  build: {
    outDir: "../publish",
    emptyOutDir: true,
    sourcemap: true
  }
});
