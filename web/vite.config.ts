import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import { VitePWA } from 'vite-plugin-pwa'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['favicon-32.png', 'apple-touch-icon.png', 'icon-*.png'],
      manifest: false, // We're using public/manifest.json
      workbox: {
        // Minimal service worker - just enough to enable installation
        // No offline caching as per requirements
        globPatterns: [],
        runtimeCaching: []
      },
      devOptions: {
        enabled: true,
        type: 'module'
      }
    })
  ],
})
