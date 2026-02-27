import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig(() => {
  const config = {
    host: true,
    port: parseInt(process.env['PORT'] ?? '4001'),
    strictPort: true
  }

  return {
    server: config,
    preview: config,
    mode: 'development',
    plugins: [vue(), vueDevTools()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    }
  }
})
