module.exports = {
  content: [
    "./wwwroot/**/*.{html,js}",
    "./Views/**/*.{cshtml,html,js}",
    "./Settings/CSSClassesSettings.cs"
  ],
  theme: {
    fontFamily: {
      sans: ['Biolinum', 'sans-serif'],
      serif: ['Libertine', 'serif'],
      classy: ['Playfair', 'serif'],
    },
    screens: {
      'sm': '786px',
      'md': '960px',
      'desktop': '1190px',
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
