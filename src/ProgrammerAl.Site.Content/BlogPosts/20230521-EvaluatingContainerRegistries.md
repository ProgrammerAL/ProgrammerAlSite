Title: Research: Evaluating Container Registry Services
Published: 2023/05/21
Tags: 
- Blog
- Docker
- Containers
---

## TL;DR
This post is a feature comparison of Container Registries hosted by different companies. The comparison is specific to the requirements of a project I'm working on. My final decision was to use the DigitalOcean Container Registry.

## Docker Registries

Containers (like the Docker ones) are hosted in a registry. There are many different kinds of registries hosted by different companies and this blog post is a feature comparison based on requirements for a project I am working on. Those requirements are probably different from yours.

## My Project

I'm working on a project that will require using an external Container Registry. The application is an IoT device and I want to push a command to the device to have it pull down N number of containers being run with Docker Compose. Here's a quick run down of my features/requirements:

- On build, push N number of images to the private registry. All tagged with the same id number (the buildnumber).
- On demand using a custom admin portal, send a message to the IoT device to pull the images for the given idnumber. The IoT device will also be given a token to pull down the images from the private registry. This needs to be short-lived because the IoT device is essentially a public device and someone can take it if they really wanted to. So...3 hours is probably good enough for the token lifetime.

Here's how that translates to system requirements:
- Hosting a Private Registry. Don't want just anyone to access the repository
- Programaticly create Access Tokens. Need to allow the backend service to create short-lived acces tokens on-demand
- Ample Storage and Egress (aka download bandwidth). The container images for each version will take approximatly 1 GB of storage among the shared images.
- Cost. I'm paying for this with my own money, and am already paying for other Cloud/SaaS services for this project. So the cheaper the better

When evaluating the Docker Registries I'll estimate by assuming 20 GB of data storage and 50 GB of egress andwidth for 1 month. This is way more usage than I'll need initially, but I hope to grow enough to use much ore than this

With those requirements, I only care about 2 things. The ability to programatically create access tokens to a private registry, and cost.

## Docker

Might as well start this research by looking at everyone's first thought of Container Registry hosts. Unfortunately I quickly realized this would not work for my own requirements. Access token are created manually through their web portal, and cannot be created programatically. https://docs.docker.com/docker-hub/access-tokens/

For my requirements, I would need to get a minimum of the Docker Pro license which is $7 a month. From what I can tell, there is no extra pricing for image storage or download egress. This sounds great in theory, but the license is per-user. So if anyone ever works with me (which I hope to happen someday), the license cost will jump for each person. https://www.docker.com/pricing/

This does not meet my requirements, so I won't be able to use it.

## GitHub Container Registry

GitHub hosts a handful of package registries (like NuGet, npm, etc) and so they treat their Container Registry as another type of package registry. Pricing is simple and all package registries are lumped in together.
https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry

GitHub has bery good pricing. You get a base amount of storage and egress for different types of licenses, and the option to pay for more on a pay-as-you-go basis. The Pro version is included in what I'm already paying GitHub, $4 per month. This gets 2 GB of image storage, and 10 GB of download egress. The overage costs for those weren't too bad.

Assuming 20 GB of data for storage and 50 GB of egress for 1 month, the monthly bill would be $6.92. This does not include the $4 I already pay for my GitHub Pro license.

Pricing info is at: https://docs.github.com/en/billing/managing-billing-for-github-packages/about-billing-for-github-packages 

When I started this research, I assumed I would use this. I already use the GitHub NuGet registry and this felt like the natual solution for me because I want my GitHub Actions to push the images. 

Unfortunately, like the Docker hosted registry above, you are unable to programatically create a token to access the registry. You must manually create a Personal Access Token with just the right permissions to access the registry, and share that token with everything that needs access. It looks like it would be possible to do this by creating a GitHub App, but that looked like way more work than I want to do. For this reason, I will not be able to use the GitHub Container Registry for this project.

This does not meet my requirements, so I won't be able to use it.

## DigitalOcean

I've had mixed results using DigitalOcean in the past. To be clear, I like DigitalOcean. They make good cloud services at a reasonable price, and I don't think enough people consider this cloud. The IoT project I'm doing this research for already uses some DigitalOcean services. But when I say I've had mixed results in the past, I mean there are times where I want to use a service of theirs but can't because of my own requirements. Usually around cloud automation.

Creating a temporary token is very easy. A straight forward REST API call will generate a token, and you can tell it how long it will live. I like the simplicity. https://docs.digitalocean.com/reference/api/api-reference/#operation/registry_get_dockerCredentials

DigitalOcean has 3 pricing tiers of Container Registry (Starter, Basic, and Professional). For my requirements I can use Basic. It has a base price of $5. Going over the 5 GB base storage costs an extra $0.02 per GB. There seems to be no extra cost for bandwidth usage, so that's a plus.

Assuming 20 GB of data for storage for 1 month, the monthly bill would be $5.30. https://docs.digitalocean.com/products/container-registry/details/pricing/

## Azure Container Registry

The Azure Container Registry seems to have a ton of features as you would expect from a company that makes software for large enterprise companies. Since I'm already using Azure very heavily this project, adding another service isn't a bad idea.

https://learn.microsoft.com/en-us/azure/container-registry/container-registry-skus

The pay-as-you-go pricing is ((Days * PricingTier) + UsageOverageCosts). For my needs, the Basic pricing tier does everything I need. Assuming 20 GB of data for storage and 50 GB of egress for 1 month, the monthly bill would be $5.90*. That's one of the best prices I've seen so far, but the egress pricing is something to keep in mind. With Azure the first 100 GB of egress out from all services is free, and then you pay $0.087 per GB. If we pretend I already use my first 100 GB on something else and this is on top of that, then the actual monthly bill is $10.25. Still not bad.

Azure Container Registry Pricing: https://azure.microsoft.com/en-us/pricing/details/container-registry/

Azure Bandwidth Pricing: https://azure.microsoft.com/en-us/pricing/details/bandwidth/

## Amazon Elastic Container Registry

Of all the SaaS providers on this list, I have the least amount of experience with AWS. Which means I'm the most worried about having wrong information for this section. Be wary of that.

With AWS, you are able to create tokens to access the private registry. It appears they have a lifetime of 12 hours and you cannot change that, but I guess it's okay. https://docs.aws.amazon.com/AmazonECR/latest/userguide/registry_auth.html

Similar to Azure, AWS also uses a pay-as-you-go pricing model. They do not have tiers, so it's just priced on usage. Assuming 20 GB of data for storage and 50 GB of egress for 1 month, the monthly bill would be $6.50. https://aws.amazon.com/ecr/pricing/


## My Choice

I ended up choosing the DIgitalOcean Registry. It meets all of my functional requirements, the cost is low and there are no extra egress charges for it, so I don't have to worry about using it a lot. Remember, I made this decision based on my own requirements which are probably different from yours.

