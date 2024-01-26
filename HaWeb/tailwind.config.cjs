module.exports = {
  darkMode: 'class',
  content: [
    "./wwwroot/**/*.{html,mjs,js}",
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
    fontSize: {
      xs: ['0.7rem', { lineHeight: '1rem' }],
      sm: ['0.85rem', { lineHeight: '1.25rem' }],
      base: ['1rem', { lineHeight: '1.5rem' }],
      lg: ['1.15rem', { lineHeight: '1.75rem' }],
      xl: ['1.25rem', { lineHeight: '1.75rem' }],
      '2xl': ['1.5rem', { lineHeight: '2rem' }],
      '3xl': ['1.875rem', { lineHeight: '2.25rem' }],
      '4xl': ['2.25rem', { lineHeight: '2.5rem' }],
      '5xl': ['3rem', { lineHeight: '1' }],
      '6xl': ['3.75rem', { lineHeight: '1' }],
      '7xl': ['4.5rem', { lineHeight: '1' }],
      '8xl': ['6rem', { lineHeight: '1' }],
      '9xl': ['8rem', { lineHeight: '1' }],
    },
    screens: {
      'sm': '700px',
      'md': '940px',
      'desktop': '1190px',
      'xl': '1440px',
      '2xl': '1680px',    
    },
    extend: {
      screens: {
        'print': { 'raw': 'print' },
      },
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
