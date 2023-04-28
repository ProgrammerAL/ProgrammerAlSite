Title: Removing the www from a URL Controlled by a Third Party
Published: 2017/11/01
Tags: 
- Custom Domain
---

## Receiving the Quest
*An adventuring party you work with from time to time needs your magical expertise. You're the only one in the group with knowledge of commanding the mystic arts. After a short bit of time you have everything working as expected. Except for one issue. Others wishing to make use of your new enchantment need to do more work than expected to get to it. Sure, it's not a big issue. But over time it gets annoying. So you sit down and decide to keep at it until it works the way it should.*

## Hosting a Podcast

A few months ago I joined a friend, and someone else I had just met, in a new podcast. As the tech guy in the group all of the web-related work fell to me. But this was the very first time I had gone through the process of setting up a podcast site. Sure, my first thought was how I'd make a custo web application to host a podcast and generate the RSS feed. But I'm an adult with a full time job and other responsibilities the rest of the week. So I decided to just pay someone and use the services of [Fireside.fm](https://fireside.fm/). At $20 USD per month, it's worth it to not deal with the hassle of everything that comes with hosting a podcast.

## Domain Problems

For the most part, customizing the web page hosted by Fireside.fm is really easy. Especially when you do the bare minimum of customizations. But the one thing I really wanted to change was to use my own domain name I purchased somewhere else. After years of advertising and tv commercials, [GoDaddy](https://www.godaddy.com/) was the first company that comes to mind when I think of buying a domain. I guess their plan worked.

So I bought the domain. Easy enough. Pulled out a credit card, typed in a few words, and then I was the proud owner of a domain. After following some steps in Fireside.fm's Advanced settings, I was able to start using that newly purchased domain and all was perfect in the world (woo!). But a few days later I realized I couldn't get to the site unless I typed the 'www' at the beginning.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/05/www-domain.png" alt="www domain" class="img-fluid">
  <figcaption>Apparently that's not an auatomatic thing. Who knew?</figcaption>
</figure>

It took some time to figure this out. Turns out there are answers online, but finding a direct answer is harder than I expected.

## Removing the WWW with Hover.com

I tried removing the www requirement for the domain a few times but just couldn't figure it out. I actually gave up on this whole thing for a few months. But when I heard some ads in some other podcast I listen to I decided I'd transfer the domain the [Hover.com](https://www.hover.com/) and see if things would be easier there. The short answer is, it kinda was. I personally found the UI easier to navigate, but that was all.

Since I paid to transfer the domain, and the whole idea was back on my mind, I doubled down and decided to try this out. After some experimentation, I got the steps down.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/05/fireside-custom-domain-field.png" alt="Fireside Custom Domain Field" class="img-fluid">
  <figcaption>1) In Fireside's advanced settings, make sure the custom domain field had my domain with the 'www' at the beginning</figcaption>
</figure>

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/05/hover-cname-record.png" alt="Hover CNAME Record" class="img-fluid">
  <figcaption>2) In Hover, add a CNAME record with the Host set to 'www' and the Value set to 'hosted.fireside.fm.' (note the dot at the end)</figcaption>
</figure>

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/05/hover-domain-forward.png" alt="Hover Domain Forward" class="img-fluid">
  <figcaption>3) In Hover, create a forward. This is to send everything from '<MyDomain>.com' to 'www.<MyDomain>.com'</figcaption>
</figure>

## Reflecting on what was done

This was the very first time I've ever worked with CNAME records for a domain. Which, on one hand, is nice for me. But on the other, it's a little jarring to realize how unhelpful documentation around this subject is. Everything I found online was just a set of steps to follow to do the thing I wanted to do, but no real explanations on why that would work. I'm hoping this is because working with CNAME records and forwards is such a common thing that no one thinks to write about it in detail. Any other reason is a little worrisome for the IT side of the industry.

I'm still not 100% sure what exactly happened on the backend. From what I can tell the CNAME record I created on my domain says that Fireside can take control of my domain for redirecting requests to it. Since CNAME records are public, Fireside is able to look at that to make sure I'm really the owner of the domain and want them to take control. From there, the 'Forward' from the non-www URL to the with-www URL is just because those are two different endpoints and we want to make sure everything uses only the with-www URL. If I'm wrong on any of this, please let me know in the comments. *I would actually like to know how this works on the backend. Right now my level of understanding is low enough that it feels too far on the side of magic where I just click something and hope for the best.*

## Summary

In the end I had an easy set of steps to follow to get my own custom domain to not require the 'www' at the beginning of the URL. Technically, it is required. But Hover forwards a request to that URL to the one with the 'www' and users are none the wiser.

---
## *Quest Rewards*
- *50 xp*
- *Knowledge: CNAME Records*
---
