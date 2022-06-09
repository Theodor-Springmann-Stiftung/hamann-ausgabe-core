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
      mono: ['ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace', 'mono']
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
        'hamannLightHighlight': '#cc7878',
        'hamannSlate': {
          50: '#6A829E',
          100: '#416C9E',
          200: '#3F8FEB',
          300: '#3270B8',
          500: '#2B619E',
          700: '#1E4570',
          900: '#173557'
        }
      }
    },
  },
  plugins: [],
}
