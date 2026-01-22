import { defineConfig } from 'vitepress'

export default defineConfig({
    title: "Moodle SDK",
    description: "Modern .NET 8.0 SDK for Moodle API",
    base: '/MoodleSdk/',
    themeConfig: {
        logo: '/logo.png',
        siteTitle: 'Moodle SDK',
        nav: [
            { text: 'Home', link: '/' },
            { text: 'Guide', link: '/guide/getting-started' },
            { text: 'API Reference', link: '/guide/advanced-features' },
            { text: 'NuGet', link: 'https://www.nuget.org/packages/MoodleSdk' }
        ],
        sidebar: [
            {
                text: 'Introduction',
                items: [
                    { text: 'Moodle Setup', link: '/guide/moodle-setup' },
                    { text: 'Getting Started', link: '/guide/getting-started' }
                ]
            },
            {
                text: 'Advanced',
                items: [
                    { text: 'Hook System', link: '/guide/advanced-features' },
                    { text: 'Error Handling', link: '/guide/advanced-features#error-handling' }
                ]
            },
            {
                text: 'Migration',
                items: [
                    { text: 'Legacy Support', link: '/guide/legacy-support' }
                ]
            },
            {
                text: 'Deployment',
                items: [
                    { text: 'Publishing Guide', link: '/guide/publish' }
                ]
            }
        ],
        socialLinks: [
            { icon: 'github', link: 'https://github.com/owoaanu/MoodleSdk' }
        ],
        footer: {
            message: 'Released under the MIT License.',
            copyright: 'Copyright © 2026 Owoaanu, Cyrus.Sushiant, SmartClouds'
        }
    }
})
