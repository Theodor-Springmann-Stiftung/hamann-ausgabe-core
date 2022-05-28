module.exports = {
  darkMode: 'class',
  content: [
    "./wwwroot/**/*.{html,js}",
    "./Views/**/*.{cshtml,html,js}",
    "./Settings/CSSClassesSettings.cs"
  ],
  safelist: [
    "ha-indent-1",
    "ha-indent-2",
    "ha-indent-3",
    "ha-indent-4",
    "ha-indent-5",
    "ha-indent-6",
    "ha-indent-7",
  ],
  theme: {
    fontFamily: {
      sans: ['Biolinum', 'sans-serif'],
      serif: ['Libertine', 'serif'],
      classy: ['Playfair', 'serif'],
    },
    screens: {
      'sm': '700px',
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
