/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "../../ProgrammerAl.Site/wwwroot/index.html",
    "../../ProgrammerAl.Site/App.razor",
    "../../ProgrammerAl.Site/Pages/*.{html,razor}",
    "../../ProgrammerAl.Site/Pages/**/*.{html,razor}",
    "../../ProgrammerAl.Site/Components/*.{html,razor}",
    "../../ProgrammerAl.Site/Components/**/*.{html,razor}",
    "../../ProgrammerAl.Site/Shared/*.{html,razor}",

    "../../ProgrammerAl.Site/DynamicContentUpdater/StaticTemplates/*.{cshtml}",
    "../../ProgrammerAl.Site.Content/BlogPosts/*.{md}"
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
