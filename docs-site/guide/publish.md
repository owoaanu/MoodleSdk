# Publishing Guide

Publishing your Moodle SDK documentation online is simple. Since the site is static, you can host it for free on several platforms.

## 1. GitHub Pages (Highly Recommended)

If your project is hosted on GitHub, this is the easiest way.

1. Create a file named `.github/workflows/deploy.yml` in your repository.
2. Add the official [VitePress GitHub Pages deploy script](https://vitepress.dev/guide/deploy#github-pages).
3. Ensure your repository settings allow GitHub Actions to publish to the `gh-pages` branch.

## 2. Vercel / Netlify

These platforms offer a seamless "push-to-deploy" experience.

1. Connect your GitHub repository to Vercel or Netlify.
2. Set the **Build Command** to: `cd docs-site && npm install && npm run docs:build`
3. Set the **Output Directory** to: `docs-site/.vitepress/dist`
4. Deploy!

## 3. Manual Build

If you want to host it yourself:

1. Run `npm run docs:build` in the `docs-site` directory.
2. Copy the contents of `.vitepress/dist` to your web server.

---

> [!TIP]
> Always build your docs locally first to ensure all links are working and the layout looks perfect.
