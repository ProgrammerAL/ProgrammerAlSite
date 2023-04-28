Title: Making this Blog a Serverless App
Published: 2017/09/08
Tags: 
- Azure Functions
- Wyam
- Azure App Service
- VSTS
- Cake
- NuGet
- CI/CD
---

## Receiving the Quest
*You're looking at the Blog-elio spell of another and realise there's something you can incorporate into your own. You want your own Blog-elio spell to be secure and make sure no one can change any of its contents before the reader sees it. It's simple enough, and done by many wizards.*

*A hooded figure appears. "You can add that yourself, you know," she says.*

*"Where did you come from? Why are you in my house?" You respond in a frantic manner. Questioning the security of a door."*

*"Through the open window. Anyway, you can make this secure. Maybe even upgrade to the point where you don't need the underlying enchantment to serve it up."*

*"But it would still need to be served by something. It's a different enchantment sure, but--*

*"Just do the damn thing. You know you want to. Come oooonnn. Do it."*

*She was very persistant. In the end, you gave in to the peer pressure. Everyone rolls a natural 1 from time to time.*

<div class="alert alert-info">
  <p>Before we start there are some quick definitions at the bottom of the page to some key phrases I'll be using. If you're new to the concept of HTTPS and SSL Certificates, it might be a good idea to skim those first.</p>
</div>

## Disclaimer
Update: This site has since been updated and no longer uses Wyam.

## Changes to the Blog?

It all started about a month ago. I was reading through [Troy Hunt's article](https://www.troyhunt.com/life-is-about-to-get-harder-for-websites-without-https/) about why we should be making all websites connect over HTTPS. There are many reasons why a site should work over HTTPS instead of insecure HTTP, performance being the first that comes to mind (it's the only way to get an HTTP/2 connection for faster downloads). So I decided to sit down and make that change to this blog. Not having ever done that before I knew this was going to take some time. 

## Setting up HTTPS for this Blog

Turns out it's super easy to add this to an Azure Website. Since I'm no expert in using the Azure CLI, I decided to stick with my trusty ol' mouse and implement this through the Azure Portal. After clicking a few items I was up and running. I opted to buy the Certificate through the Azure Portal, instead of generating one through some other cheaper service, for my own simplicity. But that was the only difficult choice. 

There was however one concern I personally had with this whole setup. In order to get SSL support on the Azure App Service I had to move from the cheap $10 per month Shared performance profile, to the higher B1 Basic profile at an estimated cost of $55.80 per month. That's a higher cost than I want to pay for the 3 humans and 60 bots that visit this site every month.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/04/AppServiceTierCosts.png" alt="Low-End Azure App Service Costs" class="img-fluid">
  <figcaption>I'm sure the bots will enjoy the faster performance</figcaption>
</figure>

But I didn't notice this until after I had paid for the certificate, so I figured I'd leave it. If the cost annoyed me too much I'd just scrap the whole concept of an Azure website and move to something cheaper and easier like [Netlify](https://www.netlify.com/). I've read they're pretty good about things and their site made it seem pretty easy to get started for free.

## Azure Functions to the Rescue

About a week after I setup HTTPS I came across a blog entry by Stephen Cleary on [using Azure Functions to host a Single Page web app](https://blog.stephencleary.com/2017/08/azure-functions-spa.html) and this immediatly sounded like a really cool idea to me. The general idea behind Azure Functions is to run some set of code when some event happens, aka a Trigger. This example uses the HTTP trigger, meaning once a request comes into the endpoint, the function will run. These are similar to AWS Lambda Functions if you're familiar with that. 

The general idea is that when a request comes in, a proxy for the Azure Function intercepts it and redirects it to some other endpoint. The HTML/JavaScript/CSS files are all stored somewhere else that can be accessed over the internet. I opted to put all of mine into Azure Blob Storage. So when someone enters http://<SomeWebsite>.com/index it returns the file from https://developersidequests.blob.core.windows.net/site/index.html to them.

## Azure Functions Proxy with Wyam

If you remember my very first post I mentioned I'm using Wyam to generate my site. The theme I chose to use is not a SPA. The [Clean Blog](https://wyam.io/recipes/blog/themes/cleanblog) template takes in the files and generates the output files. Now this is all well and good until you get to some relative HTML links:

```html
  <script src="/assets/js/jquery.min.js"></script>
  <script src="/assets/js/bootstrap.min.js"></script>     
  <script src="/assets/js/highlight.pack.js"></script>   
     
  <a href="/posts">Archive</a>
  <a href="/posts/01-Starting-This-Blog">
```

As you recall from a previous paragraph, we're redirecting away from the current site. I was hoping that the browser would know to use the storage location for those files to save me a bunch of time, but it didn't. Meaning that while the HTML file loaded correctly, the CSS and JavaScript files did not. So that's a problem. 

The code for the Wyam Theme I'm using is hosted on GitHub [here](https://github.com/Wyamio/Wyam/tree/develop/themes/Blog/CleanBlog). The *.cshtml files are what's used at 'compile' time to generate the actual static HTML files that make up the blog. Since I like to use a CI/CD pipeline to do the Wyam transformation I decided the best thing for me would be to create my own copy of the Clean Blog theme and make changes that can be used by that pipeline to generate the same website with some hard coded links like this:

```html
  <script src="https://developersidequests.blob.core.windows.net/site/assets/js/jquery.min.js"></script>
  <script src="https://developersidequests.blob.core.windows.net/site/assets/js/bootstrap.min.js"></script>     
  <script src="https://developersidequests.blob.core.windows.net/site/assets/js/highlight.pack.js"></script>   
     
  <a href="https://developersidequests.com/posts">Archive</a>
  <a href="https://developersidequests.com/posts/01-Starting-This-Blog">
```

## Changing the CI/CD Pipeline

There were only two changes made to the build for generating the blog output files. I added a step for replacing a token in the cake file to essentially hard-code where the theme is. The second change is to use that hard-coded theme.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/04/TokenizedCakeTheme.png" alt="Tokenized Cake Field" class="img-fluid">
  <figcaption>The _OutputTheme_ token will be replaced with the full path to the theme by the build</figcaption>
</figure>

The release however was completly changed. It now consists of two tasks. The first is for replacing tokens in the output files. In the below example the text with _StorageSiteUrl_ will be replaced with the value I set as a release variable. The second step in the release is copy/pasting the files to the Azure Storage location.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/04/HostedSiteUrlTokenExample.png" alt="Hosted Site Url Tokenization Example" class="img-fluid">
  <figcaption>The _StorageSiteUrl_ token will be replaced by the build</figcaption>
</figure>

Copying those files to Azure Blob Storage had one gotcha. If you just use the basic Azure Copy Files task, the default content-type returned by the browser will be set to `application/octet-stream`. When you navigate to that with your browser, the file will be downloaded, not display on-screen. There is no way you want that for a website. Basic file storage, sure. But not for a website. So you'll need to add the `/SetContentType` flag as an additional argument to the Azure Copy Files task. This way it'll just default based off the file extensions. 

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/04/CopyFilesAdditionalArguments.png" alt="Hosted Site Url Tokenization Example" class="img-fluid">
  <figcaption>I lost about a week on this. Don't do the same.</figcaption>
</figure>

## Setting up the Azure Function

This was probably the easiest part of the whole process. The blog post by Stephen Cleary uses the tools through Visual Studio, which I should use, but instead opted for using the web portal. For now I'll hope I never click the delete button by accident. In the future I may create the code for this and upload that to a git repo to have history and all that.

I just had to navigate to the Azure Portal and click the button to generate a new Azure Function. Once that was done I added two proxies. The first is to redirect anyone going to the base URL of the function at https://developersidequests.com/ to go to https://developersidequests.blob.core.windows.net/site/index.html. The second proxy is for every other web page. To redirect from https://developersidequests.com/{*url} to https://developersidequests.blob.core.windows.net/site/{url}.html. 

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/04/AzureFunctionProxies.png" alt="Azure Function Proxies" class="img-fluid">
  <figcaption>Keepin' it simple</figcaption>
</figure>

After that I moved over a few services being used by the Azure App Service like the domain name and then setup the SSL certificate. In the end, Azure Functions are just web applications that technically don't have a UI.

## Calculating the new Cost

Before I started this I was paying about $10 per month. After changing the Azure App Service site to use a size that supports SSL the cost jumped to around $60 per month. Those were fixed costs. This new solution is completely based on how much usage there is of the two different services. After playing around with the [Azure Price Calculator](https://azure.microsoft.com/en-us/pricing/calculator/) I'm estimating about 10 GB or storage with a couple million requests per month. That brings the cost of storage to around $1.26 per month. The Azure Functions then have their own separate cost. At an assumed 1 Million requests per month with each request taking 2 seconds to execute, it'll be a grand total of $0. So paying under $2 per month will feel alot nicer.

Of course I still set a quota to automatically shut off the Function in case the cost gets a little ouf of control.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/04/Quota.png" alt="Azure Function Quota" class="img-fluid">
  <figcaption>I have no idea what the number is really representing, so I'll play with the number from time to time until it feels right.</figcaption>
</figure>

## Moving to GitHub

I ended up moving the code for this blog to GitHub. The main reason is to show off the custom Wyam theme I made for this blog. If you want to have a look, feel free. Thankfully this was a one click change in VSTS to point to that repo instead.

## Summary

In the end this is a glorified file server with a nice domain name in front. There aren't even any Azure Functions that run, just a proxy to redirect incoming requests. So the title is super misleading, it's not a real serverless app. But now it's cheaper, so it was a nice set of hoops to jump through.

As a reminder, this isn't even the best solution to go with. A free account with Netlify would be better in just about every aspect. Performance would be faster using their CDN. It would have been cheaper setting up because I wouldn't have bought my own SSL Cert. But I'm going with this solution to have something to practice with. And that works for me.

---
## *Quest Rewards*
- *250 xp*
- *Blog Upgrade*
- *New Github Repository*
- *Azure Functions Proficiency*
---

## Definitions
- HTTPS - Secure connection for a web resouce, generally a web site.
- SSL - See HTTPS.
- Certificate - File generated by some 3rd party everyone trusts to say you are who you say you are for the purposes of using a secure connection. For example, a drivers license is generated by a state DMV and everyone chooses to trust them for keeping track of who's who.
- Serverless Apps - A buzzword for a hosted service that lets you run code without thinking about the underlying infrastructure, like what OS you're running on, where it is physically stored, what software is hosting the service. 
- CI/CD Pipeline - Continuous Integration and Continuous Delivery software used to automatically compile and deploy software.

### Links
* Stephen Cleary's Post on Hosting a SPA with Azure Functions: https://blog.stephencleary.com/2017/08/azure-functions-spa.html
* GitHub link for this site: https://github.com/ProgrammerAl/DeveloperSideQuests

