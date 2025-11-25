import {defineConfig, loadEnv} from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from "@tailwindcss/vite";
import path from 'path';
<<<<<<< Updated upstream
// import {envConfig}

=======
import { log } from 'console';

// console.log(envConfig.)
// https://vite.dev/config/

>>>>>>> Stashed changes
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@core': path.resolve(__dirname, 'src/core/'),
      '@utils': path.resolve(__dirname, 'src/utils/'),
      '@ui': path.resolve(__dirname, 'src/ui/'),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5152',
<<<<<<< Updated upstream
        changeOrigin: false,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '/api'),
      },
    },
  },
=======
        changeOrigin: true,
        secure: false,
        // rewrite: (path) => path.replace(/^\/api/, '/api'),
      },
    },
  },
  // some other configuration
>>>>>>> Stashed changes
})