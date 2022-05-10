import { defineConfig } from 'vite'
const { resolve } = require('path')


// https://vitejs.dev/config/
export default defineConfig({
  build: {
    assetsInlineLimit: 0,
    // lib: {
    //   entry: 'main.ts',
    //   formats: ['es']
    // },
    rollupOptions: {
      // external: /^lit/,
      input: {
        main: resolve(__dirname, 'index.html'),
        kolloquien: resolve(__dirname, 'kolloquien.html'),
        kontakt: resolve(__dirname, 'kontakt.html'),
        datenschutzerklaerung: resolve(__dirname, 'datenschutzerklaerung.html'),
        acta: resolve(__dirname, 'acta-baende.html'),
        studien: resolve(__dirname, 'studien.html'),
      }
    }
  }
})
