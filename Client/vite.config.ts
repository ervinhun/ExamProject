import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from "@tailwindcss/vite";
import path from 'path';

export default defineConfig(({ mode }) => ({
  esbuild:{
    drop: mode === 'production' ? ['console', 'debugger'] : []
  },
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@core': path.resolve(__dirname, 'src/core/'),
      '@utils': path.resolve(__dirname, 'src/utils/'),
      '@ui': path.resolve(__dirname, 'src/ui/'),
    },
  },
  server: {
    port: parseInt(process.env.VITE_CLIENT_PORT || '5173'),
    proxy: {
      '/api': {
        target: process.env.VITE_API_HOST || 'http://localhost:5152',
        changeOrigin: true,
        secure: process.env.VITE_SECURE === 'true',
        rewrite: (path) => path.replace(/^\/api/, '/api'),
      },
    },
  },
}))