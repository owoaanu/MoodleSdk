---
layout: home

hero:
  name: "Moodle SDK .NET"
  text: "High-performance LMS integration."
  tagline: "Strongly-typed, service-oriented, and built for modern .NET applications."
  image:
    src: /logo.png
    alt: Moodle SDK Logo
  actions:
    - theme: brand
      text: Get Started
      link: /guide/getting-started
    - theme: alt
      text: View on GitHub
      link: https://github.com/owoaanu/MoodleSdk
    - theme: alt
      text: Install via NuGet
      link: https://www.nuget.org/packages/MoodleSdk

features:
  - title: Modern .NET 8.0
    details: Leverages the latest performance optimizations and cloud-native patterns in .NET 8.
    icon: 🚀
  - title: Native DI Support
    details: First-class integration with IServiceCollection for clean, testable application architecture.
    icon: 💉
  - title: Strongly Typed Models
    details: Interact with Moodle entities using rich types instead of manual dictionaries or dynamic objects.
    icon: 🛡️
  - title: Extensible Hook System
    details: Easily intercept and modify requests, log responses, or handle errors globally.
    icon: 🎣
  - title: Specialized Exceptions
    details: Rich error handling that maps Moodle error codes to granular C# exceptions automatically.
    icon: ⚠️
  - title: Legacy Compatible
    details: Includes a migration path and façade for existing Moodle.cs users to ease the transition.
    icon: 🔄
---

<style>
:root {
  --vp-home-hero-name-color: transparent;
  --vp-home-hero-name-background: -webkit-linear-gradient(120deg, #3498db 30%, #2ecc71);
}
</style>
