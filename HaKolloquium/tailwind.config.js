module.exports = {
  content: [
    "./index.html",
    "./kolloquien.html",
    "./acta-baende.html",
    "./studien.html",
    "./kontakt.html",
    "./datenschutzerklaerung.html",
    "./main.js",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    fontFamily: {
      sans: ['Biolinum G', 'Biolinum', 'sans-serif'],
      serif: ['Libertine G', 'Libertine', 'serif'],
    },
    screens: {
      'sm': '640px',
      'md': '1024px',
      'lg': '1344px',
      'xl': '1440px',
      '2xl': '1680px',    
    },
    extend: {},
  },
  plugins: [],
}
