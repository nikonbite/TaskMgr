/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./wwwroot/js/**/*.js"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#3B82F6',
          dark: '#2563EB'
        },
        background: {
          light: '#F9FAFB',
          dark: '#111827'
        },
        surface: {
          light: '#FFFFFF',
          dark: '#1F2937'
        },
        text: {
          light: '#111827',
          dark: '#F9FAFB'
        }
      }
    }
  },
  plugins: []
} 