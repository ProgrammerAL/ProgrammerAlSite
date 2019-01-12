Title: New Site Hosting Model
Published: 2/21/2018
Tags: 
- Blazor
- Web Development
- Azure
- Cloudflare
---

### Receiving the Quest
*The spells you've been working on in recent times have all worked well enough. You test them and eventually put your full trust behind them. But you have this feeling in the back of your head that makes you think there's more you can do to trust what the spell is doing. Or, it's another spider bite.*

*You roll a d20 and suddenly realize what you've been missing. A good way to look at your custom spells and get a sense of how well they're crafted. No longer should you only rely on automating the testing of the spell itself. From now on you will analyze how the magical energies come together to ensure you can always update them.*

### Another Change to the Blog?

Yep. After not writing a blog post for nearly a year I decided it was time to devote some love and attention to it. Because I use this site as something to play around with specific technologies, it's easy for it to end up in weird states. I went over the previous site architecture in [Making This Blog A Serverless App](/BlogPosts/04-Making-This-Blog-A-Serverless-App.md). 

Previously this site was:
- Built using the static site generator tool [Wyam](https://wyam.io/)
- Copy/Pasted the entire site to Azure Blob Storage
- Azure Function used as a proxy to redirect all incoming requests to the proper file in Azure Blob Storage

The best part of the setup was all that only had a cost of 3 cents a month. But there were a handful of issues with this setup. The one that bothered me the most was how slow it was. Because so few visitors come to the site, the Azure Function would be 

In my next post, I'll go over the setup for this site now and what/how deployment and hosting works for it (yep, another significant change there).

### Summary

After playing around with SonarCloud a bit, I wish I could use it on every project I work on. The charts are pretty and it gives you a nice sense of how things are going for a project. Give it a try. Come one. All the cool kids are doing it. At least, that's what I'm told.

---

### *Quest Rewards*
- *200 xp*
- *Small spider bite. It should go away soon.*

