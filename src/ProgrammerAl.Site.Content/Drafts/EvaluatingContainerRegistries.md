Title: Container Registries Comparison
Published: 2023/05/16
Tags: 
- Blog
- Docker
- Containers
---

## TL;DR

This post is a feature comparison of different Container Registries. In the end I decided to use ???.

## Docker Registries

Containers (like the Docker ones) are hosted in a registry. There are many different kinds of registries hosted by different companies and this blog post does a feature comparison based on requirements for a project I am working on. Those requirements are probably different from yours.

## My Project

I'm working on a personal project that will require using an external Container Registry. The application is an IoT device and I want to be able to push a command to the device to have it pull down N number of containers being run with Docker Compose. Here's a quick run down of my requirements:

The general idea is to:
- On build, push N number of images to the private registry. Tagged with an id number.
- On demand using a custom admin portal, send a message to the IoT device to pull the images for the given id number.
  - The IoT device will be given a token to pull down the images from the private registry. This needs to be short-lived because the IoT device is essentially a public device and someone can take it if they really wanted to. So...3 hours is probably good enough for the token lifetime.

Here's how that translates to system requirements:

- Programatic Access Tokens
  - Need to allow the backend service to create short-lived acces tokens on-demand
- Ample Storage and Egress
  - Each version of my solution will be approximatly 1 GB of data among the shared images
  - To estimate, I'll assume 20 GB of data for storage and 50 GB of egress for 1 month. This is way more usage than I'll need initially, but I hope to grow to use much more than this.
- Cheap
  - I'm paying for this with my own money, and am already paying for other Cloud/SaaS services for this project. So the cheaper the better.

### Docker

### Azure Container Registry

The Azure Container Registry seems very fully featured. Since I'm using Azure very heavily for a lot of other stuff in this project, adding another service isn't a bad idea. 

https://learn.microsoft.com/en-us/azure/container-registry/container-registry-skus

The pay-as-you-go pricing is (Days * PricingTier). For my needs, the Basic pricing tier does everything I need. Assuming 20 GB of data for storage and 50 GB of egress for 1 month, the monthly bill would be $5.90*. That sounds incredibly cheap, but the egress pricing is something to keep in mind. With Azure he first 100 GB of egress out is free, and then you pay $0.087 per GB. If we assumed I already use my first 100 GB on something else and this is addative, then the actual monthly bill is $10.25. Still not bad.

Azure Container Registry Pricing: https://azure.microsoft.com/en-us/pricing/details/container-registry/
Azure Bandwidth Pricing: https://azure.microsoft.com/en-us/pricing/details/bandwidth/

### DigitalOcean

I've had mixed results using DigitalOcean in the past. But they tend to have good pricing for their services and 

With DigitalOcean I would use te Basic 

https://docs.digitalocean.com/products/container-registry/details/pricing/

### GitHub Container Registry

GitHub hosts a handful of package registries (like NuGet, npm, etc) and so they treat their Container Registry as another type of package registry. Pricing is simple and all package registries are lumped in together.
https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry

GitHub had the best pricing I could find. The Pro version is included in what I'm already paying GitHub, $4 per month. This gets 2 GB of image storage, and 10 GB of download egress. The overage costs for those weren't too bad, especially since I know I'll use more than the 2 GB of data.

Assuming 20 GB of data for storage and 50 GB of egress for 1 month, the monthly bill would be $6.92. This does not include the $4 I already pay for my GitHub Pro license.

Pricing info is at: https://docs.github.com/en/billing/managing-billing-for-github-packages/about-billing-for-github-packages 

When I started this research, I assumed I would use this. I already use the GitHub NuGet registry and this felt like the natual solution for me because I want my GitHub Actions to push the images. 

Unfortunatly you are unable to programatically create a token to access the registry. You must manually create a Personal Access Token with just the right permissions to access the registry, and share that with everything. It looks like it would be possible to do this by creating a GitHub App, but that looked like way more work than I wanted to do. For this reason, I will not be able to use the GitHub Container Registry for this project.

TODO: Maybe the token can be sent from server to device and not stored locally somehow???

## My Choice
