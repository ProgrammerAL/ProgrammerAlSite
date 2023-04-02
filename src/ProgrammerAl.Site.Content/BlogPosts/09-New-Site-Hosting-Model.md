Title: New Site Hosting Model
Published: 3/3/2019
Tags: 
- Blazor
- Azure
- Cloudflare
---

### Receiving the Quest
*You recently created your WASM spell to share your words over the enchanted spider's web. Now you want to tell all how you hath created that spell and what was involved. Below are those words.*

### The site AND the hosting changed?

Yep, might as well change both I guess. In my previous post, I wrote about updating this site to be a full client-side web app written with Blazor. The previous site had some pros and cons. 


### The Old Version

The last time I made significant changes to this site (the second of three so far!) I made some changes to the hosting setup. Here's a quick outline of that:
  - Static site generated with Wyam
  - Site files stored in Azure Blob Storage
  - Azure Function used as a proxy to redirect all incoming requests to the proper file in Azure Blob Storage

The worst part of that setup was the performance. Since this site doesn't get a lot of traffic, the Azure Function would spin down and the next request that came in would require the function to spin back up, taking around 10 seconds. I never liked this part.

### Failed Idea: Azure Static Website Hosting

My first idea was to use Azure Static Website Hosting. The idea is you put your static website in an Azure Blob Storage location, click a button somewhere, and now the files in that storage location are accessible like any static website. No server, plus cheap and fast hosting. Plus, it's easy and encouraged to add on a CDN service to the Blob Storage and improve download performance for users.

It wasn't until I was deploying the new Blazor site that I realized the limitations. It's still a new service, so I assume over time things like URL rewriting will be added. For now, this isn't something I can use. The biggest reason is Blazor's reliance on URL rewriting for the different web endpoints. Remember, there's only one actual web page in a Blazor site. So the full URL needs to be converted in a way for Blazor to know how to use it. This is why a custom Web.config file is required. That and for some other settings too. You can find an example of what's needed in this file at https://blazor.net/docs/host-and-deploy/index.html#iis

### Hosting: Azure App Service

So now that Azure Static Website Hosting isn't an option, and Azure Functions are already out, I guess that means we're down to a vanilla Azure App Service. The biggest reason why I wanted to stay away from this is the cost. I want the site to use a Custom Domain and use SSL for all connections. So the minimum App Tier to meet those needs would be the Basic one, which is $54.75 US per month. Not something I want to pay each month for a hobby site.

After some research, I learned it's possible to use Cloudflare as a front end to the site and get some free SSL goodness. Now I can go down to the Shared App Tier within the App Service which is only $9.49 US. Still, more than I'd like to pay, and eventually I'll research going down to the Free Tier, but now we're at a point we can move forward. Woohoo!

I'm not going to go too far in depth on using Azure App Service. I'm using a simple setup, with a custom domain and HTTPS with the free SSL Certificate (though the free certificate is to the default endpoint https://*.azurewebsites.net). 

### Cloudflare
Cloudflare is a cloud platform you put in front of your hosted web resources to manage them. They make it very easy to start using their services for your app with just a few clicks. The shorthand of how Cloudflare works is that each request that comes in hits their servers, which then returns the value from the actual hosted endpoint. For example, a request to https://programmeral.com/about will hit Cloudflare, which then returns https://programmeral-site.azurewebsites.net/about.

They have a lot of other features too and I highly recommend looking at using their platform for an application. For this site, I am using the following features in their free tier:
- DNS
- Resource Caching
- Free SSL Certificate for the custom domain name
- Forced HTTPS Rewrite
- Auto Minify plus Brotli Compression
- Custom Pages Rules for redirecting

The only issues I ran into when using Cloudflare were with setting up DNS right, and that's my own fault for not knowing how that stuff works. I wanted visitors of the site to not have to worry about adding www to the URL and that turned out to be harder than I expected (this is only the second site I've ever managed on my own). But eventually, I got it right. The issue was because the domain name was still setup on the Azure side to point to the full Azure hosted endpoint, which just made things weird once Cloudflare was added. Once I got it all moved over to Cloudflare things started going smoothly. 

### Sitemap
I have absolutely no idea how well search engines can crawl a full Web Assembly site. From the short time this has been up, I'm assuming it's not yet possible because Google can't even find this when searching. But it's always a good idea to help web crawlers where possible. I created a small utility that is run during the build in Azure DevOps, so it's always up to date. For now it only has entries for blog posts, but that's because I forgot to add the other pages like Home and About. You can see it [here](https://programmeral.com/sitemap.xml) if you're curious what one looks like. The code for that is hosted with the rest of the code for this site on GitHub [here](https://github.com/ProgrammerAl/ProgrammerAlSite/tree/master/ProgrammerAl.Site/DynamicContentUpdater).

### Summary

Getting this site hosted was pretty easy. Like many projects, it all came down to what kind of dependencies I want to deal with. Azure is great for a lot of things, but it still lacks some of the web app tier features Cloudflare excels at. To be honest, Cloudflare makes using their service so easy that even if Microsoft added the same features to Azure I probably wouldn't even make the switch. But now it's all setup and I can wait another year to change the whole site again!


---

### *Quest Rewards*
- *400 xp*
- *A working site, again.*
- *Knowledge of Cloudflare*
