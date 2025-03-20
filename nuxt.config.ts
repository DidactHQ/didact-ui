// https://nuxt.com/docs/api/configuration/nuxt-config

import tailwindcss from "@tailwindcss/vite";
import Aura from '@primeuix/themes/aura';

export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  devtools: { enabled: true },
  vite: {
    plugins: [
      tailwindcss(),
    ],
  },
  css: ['~/assets/css/main.css'],
  modules: [
    '@primevue/nuxt-module'
  ],
  primevue: {
    options: {
        theme: {
            preset: Aura
        }
    },
    components: {
      // Need to temporarily exclude PrimeVue form components because of npm install bug.
      // See https://github.com/primefaces/primevue/issues/7434.
      exclude: ['Form', 'FormField']
    }
  },
  ssr: false
})
