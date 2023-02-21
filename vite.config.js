import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  root: "src",
  plugins: [react()],
  assetsInclude: ['**/*.mp3', '**/*.m4a', '**/*.jpg'],
  build: {
    outDir: "../publish",
    emptyOutDir: true,
    sourcemap: true
  }
});
