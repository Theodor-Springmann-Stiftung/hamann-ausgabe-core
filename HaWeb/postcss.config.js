module.exports = {
  plugins: [
    require('postcss-import'),
    require('tailwindcss'),
    // Production:
    // require('autoprefixer'),
    // require('cssnano')({ preset: 'default' })
  ],
}
