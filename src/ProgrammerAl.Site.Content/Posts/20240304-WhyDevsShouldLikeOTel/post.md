Title: Why developers should like OpenTelemetry
Published: 2024/03/04
Tags:

- Blog
- OpenTelemetry

---

## Why Developers Should Like OpenTelemetry

I often see people looking at OpenTelemetry (OTel) and thinking it's not very useful because other tools they use already do the same thing. So why switch? For me, the short answer is because it's a single standard, but more on that below.

Before we get to the full answer, let's do a quick history review to make sure we're on the same page.

By the way, I will not be recapping what OTel is. If you're unfamiliar with that awesome tool, ask an A.I. chatbot or something. Then double check what it tells you by clicking on at least one of those reference sites, because those things are still way too wrong to trust.

## Observability

For a long time the only way to observe what an application was doing was to look at log statements. So as developers, that's all we did; we just pushed log messages. Sometimes a, "Starting process ABC" message or simply outputting a stack trace was all we needed. When something went wrong we could trace the flow of the process with those logs.

Eventually I.T. departments started adding their own information to track how the app is running. Things like CPU/Memory/Disk usage, average request length, requests per minute, etc. But that info was stored in different places from the logs, so combining logs and these app metrics wasn't easy. 

Once all this info became commonplace, the business side started using it all to answer questions. How many carts are abandoned? Are responses returned too slow that people leave the site? And more (I assume!).

For a long time all this info used to answer business questions was stored in separate systems. Developers would put information in log statements, which would have to be queried with some kind of string searching. Other information would come from a database or some external third party. Combining this data was a hassle, and a big part of someone's job (thankfully not mine).

Fast forward to today and just about every logging provider out there has an SDK to let you send all of this information to them. Developers can send application logs to the provider, while the infrastructure team can send server metrics to that same external service. Some of these providers use an agent you run on the same machine, others are a code change that require a package reference. And however you use that provider, the code is different for each. 

## Just answer the question. Why should devs like OTel?

Finally, with the history lesson out of the way, lets answer the question, "Why should developers like OTel?" I said above the short answer is because it's a single standard. Whenever a phrase like that pops up, a single standard, everyone says the big benefit is that you can move your data from one provider to another. That's cool, but, when does a company do that? Every few years at most? The longer answer is that it's a single API that you (a developer!) have to learn.

Yeah, it's not difficult to learn a logging API. But they each have different feature sets with different pros/cons. Different types of data you can send. Different dashboards that query the data in different ways. It's a whole project to get one setup effectively. With OTel you have a single API you can learn in and out, and know you can use it across all of your applications. You can also change companies and/or tech stacks and know you can push out the same data without learning a whole new system first. 

With OTel, you can develop locally and push traces/spans/metrics to a local listener to view. To be fair this can be a bit of a hassle to setup, and it's not as easy as just seeing a log message, but things are getting easier. Plus you have companies like Microsoft building things like Aspire to help the dev loop (if you haven't heard of Aspire before, it's really cool and you should [check it out](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)). After you commit your changes and push to qa/staging/prod/etc environments, the code can push to an external provider like Azure Application Insights, Honeycomb, etc. A ton of companies have support for OTel, and your provider is probably on the list. It's the same data in each environment, including your local environment.

And that's it. OTel is a time saver for developers that will let us focus on writing code for the application and worry less about a requirement every app needs.


