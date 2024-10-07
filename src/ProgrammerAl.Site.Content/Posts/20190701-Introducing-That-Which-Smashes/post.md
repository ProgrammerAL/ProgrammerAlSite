Title: "Introducing: That Which Smashes"
Published: 2019/07/01
Tags: 
- Azure
- Azure Functions
- Azure Queue Storage
- Pulumi
- Load Testing
- Blog
---

## New Project: That Which Smashes
This blog post is to describe a new Open Source Software application for Hammer Testing a publicly facing endpoint. Since everyone likes to choose a cool name for projects like these, I went with the name That Which Smashes, which is English for Mjölnir. The project is hosted on GitHub here: https://github.com/ProgrammerAl/ThatWhichSmashes/

## Disclaimer #1:
This blog post is to detail an application I created for TESTING PURPOSES ONLY. It is possible to take this and build something to Hammer Test other infrastructure you don't create, but I do NOT condone that in any way. Don't be an ass.

## Disclaimer #2:
I have not yet run this at scale. I'm trying to convince my employer to let me run it against our publicly facing site, but we need to wait for some other things to happen first. You know, the usual type of things that pop up at large enterprises.

## Hammer Testing
Also known as High-Intensity Load Testing. At least, I assume that's what people usually call it. But I honestly don't know if there's already a name for this or not and I like the phrase Hammer Testing. The general idea is to send so many requests to an endpoint that you are trying to bring it down, similar to a DDoS. The goal of this type of testing is to measure the limits of what your infrastructure/application can handle.

## The Why
The company I work for regularly gets hit with [DDoS](https://en.wikipedia.org/wiki/Denial-of-service_attack) attacks. During one particularly long three-day weekend someone finally asked, "Why don't we attack ourselves to test our infrastructure instead of waiting for them to happen organically?" There was already some talk going on with different teams to start Load Testing, so it was going to happen eventually. But a few days later, I began wondering what it would take to create a solution to DDoS a site. You know, for funsies.

## The Code and Infrastructure
There are multiple aspects to the application, but for the most part, it's pretty simple. The core is an Azure Function which listens for events from Azure Queue Storage. The objects in the queues are JSON messages that detail the HTTP Request Messages the Azure Function should be sending. And that's it for the code. 

Because we're only using a single Azure Function and a single Queue for this, we want to deploy it to multiple Azure Regions. The hard part is figuring out how to deploy this at scale. No one wants to do this by hand, so the next solution to look at it automating that.

## A New Challenger Approaches: Pulumi
If you haven't heard of [Pulumi](https://www.pulumi.com/) before I highly recommend looking at their offering. It's a tool for managing your cloud resource deployments through code. As a developer using Azure, this means I don't have to use ARM templates. Goodbye sea of XML and hello Typescript. 

In this project, Pulumi is used to deploy an instance of the Azure Function and Azure Queue Storage to each of the regions we choose to deploy to. It also creates the parent instance of Application Insights for all of the shared logging to be tracked. Finally, Pulumi is used for deleting the resources from Azure when we're done using them.

## How to run That Which Smashes
The fully fleshed out instructions are in the ReadMe.md file in the code repo. But here's a short overview of the steps you'd need to take to deploy and run the solution.

- Deploy the infrastructure to your chosen Azure Regions
    - The regions are hard-coded in the index.ts file
    - Start with the Azure Functions disabled
- Manually fill all queues
    - Alter the `/ThatWhichSmashes/RequestQueueAdder/data/*.json` files. These are the files used by the console application to fill up the Azure Queues.
- Turn functions on
    - Alter the `Pulumi.dev.yaml` file to enable the Azure Functions and re-run the deployment
- Wait for the functions to run and clear their respective queues
- Learn what you can from Application Insights
- Destroy the infrastructure with Pulumi


---

## *Side Quest Rewards*
- *600 xp*
- *Skill Upgrade with Azure Functions*
- *New Skill:  Pulumi*
