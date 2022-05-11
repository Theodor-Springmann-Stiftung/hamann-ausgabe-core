module.exports = {
  content: [
    "./wwwroot/**/*.{html,js}",
    "./Views/**/*.{cshtml,html,js}",
  ],
  theme: {
    fontFamily: {
      sans: ['Biolinum', 'sans-serif'],
      serif: ['Libertine', 'serif'],
    },
    screens: {
      'sm': '786px',
      'md': '1024px',
      'lg': '1312px',
      'xl': '1440px',
      '2xl': '1680px',    
    },
    extend: {
      colors: {
        'hamannHighlight': '#d80000',
        'hamannLightHighlight': '#cc7878'
      }
    },
  },
  plugins: [],
}
