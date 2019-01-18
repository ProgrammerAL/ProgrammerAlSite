Title: New Site Hosting Model
Published: 1/17/2019
Tags: 
- Blazor
- Azure
- Cloudflare
---

### Receiving the Quest
*The spells you've been working on in recent times have all worked well enough. You test them and eventually put your full trust behind them. But you have this feeling in the back of your head that makes you think there's more you can do to trust what the spell is doing. Or, it's another spider bite.*

*You roll a d20 and suddenly realize what you've been missing. A good way to look at your custom spells and get a sense of how well they're crafted. No longer should you only rely on automating the testing of the spell itself. From now on you will analyze how the magical energies come together to ensure you can always update them.*

### The site AND the hosting changed?

Yep, might as well change both I guess. In my previous post, I wrote about updating this site to be a full client-side web app written with Blazor. The previous site had some pros and cons. 


### The Old Version

The last time I made significant changes to this site (the second of three so far!) I made some changes to the hosting setup. Here's a quick outline of that:
  - Static site generated with Wyam
  - Site files stored in Azure Blob Storage
  - Azure Function used as a proxy to redirect all incoming requests to the proper file in Azure Blob Storage

The worst part of that setup was the performance. Since this site doesn't get a lot of traffic, the Azure Function would spin down and the next request that came in would require the function to spin back up, taking around 10 seconds. I never liked this part.

### Failed Idea: Azure Static Website Hosting

My first idea was to use Azure Static Website Hosting. The idea is you put your static website in an Azure Blob Storage location, click and button somewhere, and now the files in that storage location are accessible like any static website. No server, plus cheap and fast hosting. Plus, it's easy and encouraged to add on a CDN service to the Blob Storage and improve download performance for users.

It wasn't until I was deploying the new Blazor site that I realized the limitations. It's still a new service, so I assume over time things like URL rewriting will be added. For now, this isn't something I can use. The biggest reason is Blazor's reliance on URL rewriting for the different web endpoints. Remember, there's only one actual web page in a Blazor site. So the full URL needs to be converted in a way for Blazor to know how to use it. This is why a custom Web.config file is required. That and for some other settings too. You can find an example of what's needed in this file at https://blazor.net/docs/host-and-deploy/index.html#iis

### Hosting: Azure App Service

So now that Azure Static Website Hosting isn't an option, and I Azure Functions are already out, I guess that means we're down to a vanilla Azure App Service. The biggest reason why I wanted to stay away from this is the cost. I want the site to use a Custom Domain and use SSL for all connections. So the minimum App Tier to meet those needs would be the Basic one, which is $54.75 US per month. Not something I want to pay each month for a hobby site.

After some research, I learned it's possible to use Cloudflare as a front end to the site and get some free SSL goodness. Now I can go down to the Shared App Tier within the App Service which is only $9.49 US. Still, more than I'd like to pay, and eventually I'll research going down to the Free Tier, but now we're at a point we can move forward. Woohoo!

I'm not going to go too far in depth on using Azure App Service. I'm using a basic setup, with a custom domain and HTTPS on with the free SSL Certificate (though the free certificate is to the default endpoint https://*.azurewebsites.net). 

### Cloudflare

### Summary

After playing around with SonarCloud a bit, I wish I could use it on every project I work on. The charts are pretty and it gives you a nice sense of how things are going for a project. Give it a try. Come one. All the cool kids are doing it. At least, that's what I'm told.

---

### *Quest Rewards*
- *200 xp*
- *Small spider bite. It should go away soon.*

