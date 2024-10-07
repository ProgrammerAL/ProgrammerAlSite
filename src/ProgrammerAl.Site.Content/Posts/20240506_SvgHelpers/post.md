Title: "New Project: SVGHelpers.com"
Published: 2024/05/06
Tags: 
- Blog
- SVG
- Project
---

## New Project: SvgHelpers.com

Statistically, you aren't aware I make programming themed [web comics](https://programmeral.com/comics/latest). Generic and stupid jokes, kind of like the Dilbert guy, but I'm way less problematic and don't own a pool shaped like Dilbert's head (look it up (both of them)). Anyway, I needed some tools to help create those image files, so I created SVG Helpers.

## What's an SVG?

If you're not familiar, the SVG image format is an XML syntax of primitive shapes. So instead of other formats like PNG, JPEG, etc that specify each individual pixel, SVGs are instructions on what to draw at runtime. Think something similar to HTML, but less abstract. The example below is an SVG image which draws a rectangle.

```svg
<svg xmlns="http://www.w3.org/2000/svg"
    xmlns:xlink="http://www.w3.org/1999/xlink">
    <rect x="5" y="5" heigh="10" width="20" fill="green"></rect>
</svg>
```

I like them because I have no drawing or make-things-look-pretty skills (exhibit 1: this website), and they scale with no extra work.

The cool thing is they also let you embed custom code directly into the file, like CSS or Javascript. Because of that feature, the web comics I've created so far have support for dark mode. BUt this feature can be a double edged sword. Because loading unknown code is a big security issue, a lot of sites don't let you upload SVGs. So there are extra hoops you have to jump through to upload to other sites. But for your own site, it doesn't matter.

## SvgHelpers.com

I create my SVGs artisanally by hand, so some new tools are needed to help with the creation process. And thus [SvgHelpers.com](SvgHelpers.com) was born. The purpose of the site is to host utilities that make hand editing SVG files easier. Right now there are only 2 features, the SVG Mover and Continuous Refresher.

### SVG Mover

The first feature moves all elements in the given XML based on X and Y inputs. It's really common for me to create a section of a comic and later decide to make one section biger or smaller, requiring other elements to be moved. This makes it easy to copy/paste in the section that needs to move, and automate moving those elements.

### Continuous Refresher

The second feature will allow you to "upload" a file to the web client. It will then display the image on screen and refresh from the file every second. When editing SVGs, I usually have the code up in Visual Studio Code and then have to manually refresh a web browser on a different monitor to see changes. This tool saves me the couple seconds switching over to the browser and hitting refresh. This will save a load of time.

## Why not use any of the many tools that already exist?

I didn't feel like it. Honestly I don't know what's out there and my current process for editing SVGs works well enough. In the future I might change things up, but for now this does what I need. 

## Requesting Features

The code is open source on GitHub at [https://github.com/ProgrammerAL/SvgHelpers](https://github.com/ProgrammerAL/SvgHelpers). If you think you have a good idea on what to add, open an issue. I also accept PRs. 

## Tech Behind the Site

This is a C# Blazor WebAssembly site. All hosted in the browser with no server side components. The static files are hosted on Cloudflare Pages, and everything is deployed with IaC using Pulumi run through GitHub Actions. 

If you're any more curious, you can look at the source code at [https://github.com/ProgrammerAL/SvgHelpers](https://github.com/ProgrammerAL/SvgHelpers).

