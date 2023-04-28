Title: Starting This Blog
Published: 2017/01/16
Tags: 
- Wyam
- Azure App Service
- VSTS
- Cake
- NuGet
- CI/CD
---

## Receiving the Quest
*The town is saved! Thanks to you and the rest of your party, the village no longer need worry about the evil Countess Memorine Leaks, and her trusty adviser, Sir Doesn't Call Dispose. After completing the day's adventure, you and your party have left each other to your own devices. The friendly Barbarian has gone home to his family, while the party leading Paladin has stayed late yet again to answer his long list of Enchanted-Mails. You have gone to an inter-network of fellow Code Wizards.*

*Looking around, you see leagues of other wizards posting updates of their recent quests. Many even with entire tomes chronicling their years of adventuring. You've long thought of learning one of the many Blog-elio spells. But today the thought gnaws at you more than usual. While perusing through small messages brought in by the little blue tweet-tweet birds, you think to yourself how easy a task this really might be. Unlike the many other quests you tend to take on. There really is only one way the find out.*

*You're lost in your own thoughts when a hooded figure approaches, and in a raspy voice says, "I see you..." Cough-Cough-Cough. "Sorry about that, I'm still getting over a cold." He says, with a much clearer voice this time. "I see you admiring the work of your fellow peers. You can begin doing the same too."*

*His words make sense. You may have never worked with some of the magics involved before, but they look simple enough. "How would I go about starting? Many here have developed their own spells to enchant their words. Many more have used pre-made spells which require extra time to learn to use and setup correctly. My daily quests adventures keep me busy enough. I don't think I can really start this right now," you reply.*

*The hooded figure responds, "That matters not. This quest is simple enough. You may choose to accept it, or not."*

*"I accept!" You respond, realizing you said that much louder than you meant to. "Sorry about that, I got a little too excited. I mean, I accept," this time at a much more reasonable volume.*


## Why a Blog?

Now that we've accepted our quest, we need to explore the reasons why we even need one. I don't know about you, but I've been toying with the idea of starting a blog for at least three years. The same thoughts always come up.

- Someone else already writes about <stuff>
- I don't want to start another project
- I don't want to maintain another project
- What should the folder structure look like?
    - Migrating this later could be a real pain
- It would all be a waste to just give up a year down the line
- How should I host it?
- Etc, etc, etc


The list goes on, but you get the idea. In the end, we can make excuses as long as we want, but in my case I just got fed up with wanting to write one and not doing anything about it. Below are the steps I went to in order to get this blog up and running. Feel free to use it as a starting point for yourself, or ignore it altogether. Remember, choose your own side quests.

## Starting a Blog the Programmer Way

I try to follow a lot of other people on Twitter in the software development field. So naturally over the years I got into the habit of seeing some technology and thinking, "Hey, that looks cool. Maybe I could use it in my blog. Maybe now's the time to start a blog. Maybe even...oooh, a shiny thing." Then one day I came across a great post by [Scott Hanselman on using Wyam](http://www.hanselman.com/blog/ExploringWyamANETStaticSiteContentGenerator.aspx). Turns out [Wyam](https://wyam.io/) is a great open source utility described as a *static content toolkit*. This is great! Just write out the posts in Markdown, run a quick command line utility and bam! Instant blog site. 

But why stop there? Once we have the blog written up, we should use other tools to get it deployed easily. I mean, why do things by hand when we can automate it all? Once the blog is created, we'll use tools to automatically build and release the site.

## Disclaimer
Update: This site has since been updated and no longer uses Wyam.

## Wyam
Getting started with Wyam is easy enough. There are a handful of places to get the bits from, but I chose to use the Windows installer [here](https://github.com/Wyamio/Wyam/releases). Just download the Setup.exe and run it on the machine. Now Wyam is installed and you get a cool command prompt in your start menu.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/WyamCommandPrompt.png" alt="Wyam Command Prompt" class="img-fluid">
  <figcaption>You know, a command prompt.</figcaption>
</figure>

Go ahead and open that up, then navigate to some folder. We're going to be generating files, so you probably want an empty directory. If you clicked on the link above to Scott Hanselman's blog post on using Wyam, the next few tidbits will be very familiar.

You've got a few commands to learn for the getting started. The first is ``Wyam new -r Blog``. That will create a new folder called input with a few files. The ``new`` flag in that command specifies we're starting a new project. But the ``-r Blog`` specifies we're using the 'Blog' recipe. Think of the recipe as the template. There are a handful of other recipes, but I haven't played with them so learning more there is on you (Your own side quest!).

<div class="alert alert-info">
  <p><strong>Pro Tip</strong>: The recipe flag (-r) is required here. So don't forget it.</p>
</div>

Now we have a folder with some basic markdown files. Woopty-Doo. We're making a blog for the internet, so we should learn how to turn the markdown files into html. To do that, we need to run the command: ``Wyam -r Blog``. This is just like before, only now we don't have the ``new`` flag. Any guess why? I'm going to assume you shouted, "Because this isn't a new project anymore!" And for that, you're right. Also calm down, no need to shout.

After running that command we have the 'output' folder. Go ahead and explore that folder. We have some html files, css, and some other things. You know, what we need for a site. Yipee! Now open that index.html file in your browser. Wait a minute or so and you'll see things are not at all what you'd expect. Well that's because we need to actually run this with the built in Wyam Previewer. 

Using that Previewer is simple enough. Go back to your trusty command line and use the command: ``Wyam -r Blog -p``. That new flag ``-p`` will start the small web server and host your site locally. The default site is http://localhost:5080/. Open it up in your browser to look around.


## Actually Making a Blog with Wyam

Now that we've mastered three commands from Wyam, we need to be able to work with the files themselves. Wyam is very extensible and you can use any combination or Markdown, cshtml razor files, and others too I'm sure I don't know about. I debated for quite a while on what combination to use. In the end I decided to try using only Markdown for content. The deciding factors were a) I want to keep things simple, and b) [I found this blog post by Dave Glick too late](https://daveaglick.com/posts/integrating-wyam-into-an-aspnet-mvc-site).

So now that we know we're using markdown for as much as possible, which editor should we use? As a .Net Developer I spend just about all of my development time in Visual Studio. Sometimes in Visual Studio Code. Turns out, VS Code is a great Markdown editor. It was great at first, but it's pretty bare bones for someone like me who still needs to learn the Markdown format. Then I remembered the [.Net Rocks Podcast episode](http://www.dotnetrocks.com/?show=1395) talking about [Markdown Monster](https://markdownmonster.west-wind.com/).

<div class="alert alert-info">
  <p><strong>Disclaimer</strong>: This is one of the few .Net Rocks episodes I didn't get a chance to listen to. Things always get hectic around December and I had to skip a few things. To be honest, I only gave Markdown Monster a try because I remembered the name. Luckily, things worked out.</p>
</div>

My favorite thing about Markdown Monster is that it opened its own SampleMarkdown.md file as an example. My second favorite, is that the right-side window automatically displays the markdown content as you would see it. It's great. I definitely recommend giving it a try if you haven't already. It's not free, but definitely a great Markdown editor. There is a free trial available.

With our trusty new Markdown editor, we can go ahead and start writing out the blog entries. Go ahead and open the file under ``input -> first-post.md``. It's pretty empty, but you'll see something similar to this:

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/first-post-description.png" alt="First Port File Description" class="img-fluid">
  <figcaption>Assuming you're using Markdown Monster, you'll see the File text on the left, and the preview on the right.</figcaption>
</figure>

The first-post.md file is a little different than what we'll actually end up seeing. The stuff above the thee dashes on the fourth line will be used for some Wyam specific settings. They're pretty self explanatory, so we don't need to go into detail here. Everything below those dashes on line four will be the blog entry itself. 

This is as far as we'll go with looking at Wyam. As I mentioned earlier, it's pretty extensible and there's a lot you can do with it. If you want more advanced options you can find them online with a quick search.

## Source Control
After some writing, we have a blog. Or at least a good enough template. But it's software, so we need some sort of source control. You're nodding your head in agreement right? If not, start now. I'll wait.

There are a lot of places that can host our code for free. Publicly hosting on GitHub is a popular option. It's very easy to setup a new repository with GitHub. As long as it's open source, it's free. Just go to https://github.com/new and make it. This was very tempting, but in the end, I decided to go with Microsoft's Visual Studio Team Services (VSTS). Not because I like long names, or want to keep this code hidden from everyone, but because I use it a lot for my day job and it'll be nice to have an active project I can use to experiment with new features. If you want to use something else, have at it. Plus, this also is free as I'm the only person working on the code.

VSTS hosts Git repositories pretty easily. If you've never worked with it, you can find some good content online. Either through Microsoft's own [Channel 9](https://channel9.msdn.com/), [Microsoft Virtual Academy (MVA)](https://mva.microsoft.com/), or wherever else you choose to search online. Just be sure the content you're learning from is fairly recent. Features are constantly evolving, which has even tripped me up in front of customers (like last week). 

If you're new to working with VSTS, it's pretty easy to get started. You can create your own account from the getting started button [on this page](https://azure.microsoft.com/en-us/services/visual-studio-team-services/). It's been a while since I made a new instance and don't want to write up the whole getting started process here, so I'll leave you to your own devices. But the general idea is that it's free and you should have a default 'Team Project' already created for you. I created my own Team Project called Personal. 

Inside my 'Personal' Team project I added a new Git Repository. Just navigate to your Team Project, go to the 'Code' tab, and add a new repository (or use the default one already there). 

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/VSTSNewRepository.png" alt="VSTS New Repository Menu" class="img-fluid">
  <figcaption>I already have a Blog repository, but you can create one from this menu</figcaption>
</figure>

Now that we have a Git Repo, we can pull it down to our local machine for development. Again, it's Git, so you can use whatever tool you prefer. I prefer to use Visual Studio since it's what I know. Yep, crazy that I'm loading up such a huge program just to use it as a Git client, but for now this is my preference. I'm sure things will change someday, but that's not today.

Within Visual Studio, find the Team Explorer Tab and click on the Manage Connections button (the green icon that looks like a plug). From there, click on the Manage Connection button link to Connect to Team Project (Yes, two different buttons with a very similar name). From there you'll get a menu like the one below. Choose your Git Repo, give it the path you want to use on your machine and click Connect.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/ConnectingToVSTSInVS.png" alt="Connect to Team Project Menu from Visual Studio" class="img-fluid">
</figure>

If you want some better documentation for doing what we just talked about, [try this documentation from Microsoft](https://www.visualstudio.com/en-us/docs/setup-admin/team-services/connect-to-visual-studio-team-services#connect-and-share-code-from-visual-studio).

With your Git Repo on your machine, once you feel like it, go ahead and commit your changes to sync up with the server. I chose to only commit my 'input' folder for now. More files will be added over time, but for now this is good enough.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/BlogCodeInVSTS.png" alt="Code Check-In to VSTS Repo" class="img-fluid">
  <figcaption>Ignore the extra files for now. We'll be adding those in a bit.</figcaption>
</figure>

## Continuous Integration

Pop Quiz: What's Continuous Integration? 

If you said, "Some Buzz Word," I'll give you 10 xp. But the correct answer would be something like, "Running a build as soon as code is checked in." You'll also see Continuous Integration shortened to CI very often. No one wants to keep typing that out.

Since we brought up CI, you're probably thinking we'll set this blog up to use that concept. VSTS has a good UI for doing this. The eventual goal is to have every check-in do a build **and** a deployment, but we'll get to that later. For now we'll setup an automated build in VSTS to take what's checked into the repository and run Wyam over it to generate the 'output' folder.

Go to the Build tab, and create a new build with an empty template (instead of the default Visual Studio template). Other than that, I'm using the defaults. Master Branch, Hosted build queue, etc. If you noticed, I said we're using the 'Hosted' build system. This means the build process is being run by Microsoft on their servers. I can setup an agent to do all of that work on a machine I control, but really don't want to do that for a handful of reasons. We're going to stick with a 'Hosted' build for this.

So far all of our 'building' has been from the command line with Wyam already installed on the local machine. This is great locally, but causes a problem when we want to use the hosted build. Remember, this will be run on some random machine in Azure that we have zero access to. We can check-in the Wyam executable to the Git repo and call that, but now we have to keep track of the version and update it on a regular basis. Maintaining this will just be too much work. So how do we get this building in the cloud? I'm sure there are other ways, but I decided on using Cake with the NuGet package.

#### Cake
I've been wanting to play around with Cake for a while, just haven't had a good enough excuse yet. The general idea behind Cake is to create a single script file with C# syntax and use that for just about everything. Building, Releasing, running tests, etc. It's a great [open source utility hosted on GitHub](https://github.com/cake-build/cake) that recently got into the [.Net Foundation](https://dotnetfoundation.org/blog/cake-welcome), so you can feel a little better about it being continually updated. It's also very extensible with an active community.

The general process is to run a PowerShell script that calls the *.cake script, which then does all the work. The getting started tutorial [here](http://cakebuild.net/docs/tutorials/setting-up-a-new-project) makes it look pretty easy and I was up and running in just a few minutes.

#### NuGet
The basic package manager for .Net. If you haven't worked with it before you can find some tutorials online pretty easily. [This one looks good](https://docs.microsoft.com/en-us/nuget/quickstart/use-a-package). 

If you're new to the concept of package managers, the basic idea is that the manager (NuGet, NPM, etc), keep track of the external packages (libraries, SDKs, whatever you call them) used by your software. Those packages are hosted online and when you need to do a build, the package is downloaded to the machine. Want to update to a newer version? Just click the update button. Want to roll back to an old one? Click that button instead. 

<div class="alert alert-info">
  <p><strong>Disclaimer</strong>: Use a package manager as much as possible. Have an external 'Third Party Libs' folder? That's a pain to work with. No one wants to keep track of the right version of these files.</p>
</div>


## Setting up the Build

On our local machine, we'll need to make a Cake script. Because this was my first Cake script I decided to take a look [at what Dave Glick uses for his blog](https://github.com/daveaglick/daveaglick/blob/master/build.cake). That's pretty in depth. Our script just needs to do a build. Below is what I made for this blog. You can see the first two lines say to use NuGet to download locally the very latest version of Wyam (prerelease version). Then the task named 'Default' is called. This task uses Wyam to run on the local directory with a few options. You'll recognize some of those options from the command line we used earlier.

```csharp
#tool nuget:?package=Wyam&prerelease
#addin nuget:?package=Cake.Wyam&prerelease

var target = Argument("target", "Default");

Task("Default")
    .Does(() =>
    {
        Wyam(new WyamSettings
        {
            Recipe = "Blog",
            Theme = "CleanBlog",
            UpdatePackages = true
        });
    });
    
RunTarget(target);
```

This was saved to a 'build.cake' file at the root of the Git repo. Once you have this, go ahead and check it into your Git Repo. 

Remember that Build Definition we started with the VSTS web client? Let's go back to it. We'll be adding two build steps. The first one is to run the Cake script for the build. In order to do that, we'll need to add the Cake plugin to our VSTS instance. [Go here and install the plugin](https://marketplace.visualstudio.com/items?itemName=cake-build.cake). Once you have that, you'll have the Cake build step. Just add it to the build definition.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/AddingBuildStep.png" alt="Adding a Cake Build Step" class="img-fluid">
  <figcaption>3) Eat cake? I swear it's not a lie.</figcaption>
</figure>

Once you have that, go ahead and specify the right *.cake build script to run for this build task. Click on the ellipsis button for the 'Cake Script' entry and choose your cake build script file from the popup.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/ChoosingBuildScript.png" alt="Adding a Cake Build Step" class="img-fluid">
  <figcaption>Just add the one step for now. This is all we'll need.</figcaption>
</figure>

Now that we can build the blog, we'll need to ship the built files up to VSTS. Click the button again to add a new build step. Under Utility add the 'Publish Build Artifacts' task and then close the window. Then go ahead and make sure the settings you have match the image below. You'll want to specify that the Path To Publish is the 'output' folder (what's generated by running Wyam), and that the Artifact Type is set to 'Server' (meaning the 'output' folder will be sent up to VSTS instead of some other path on a server we control).

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/PublishTaskSettings.png" alt="Settings for Publish Task" class="img-fluid">
  <figcaption>We're publishing the 'output' folder created by running Wyam.</figcaption>
</figure>

Lastly, go to the 'Triggers' tab for this build definition. Make sure the Continuous Integration option is checked. Go ahead and save the build definition and fine the 'Queue new build...' button. After some time the build will run and you should get a result summary. In that summary, find the 'Artifacts' tab and click Explore for the specific output. This is exactly what was sent up to VSTS. It should look something like this:

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/BuildArtifacts.png" alt="Output Build Artifacts" class="img-fluid">
  <figcaption>If you're missing files, check the settings for the Publish task in your build definition</figcaption>
</figure>

## Continuous Delivery

Pop Quiz Part 2: What is Continuous Delivery?

If you said, "Some Buzz Word," You lose 10 xp for being unoriginal. But yes, just like CI, CD is another buzzword meaning as soon as a build is completed, a release is triggered. Not all the way to Production. But deploying often has some great benefits.

Before we start a deployment, we need to know where to deploy to. Wyam generates static html files, so the requirements for hosting are pretty slim. Dave Glick has a [great post on using Netlify](https://daveaglick.com/posts/moving-to-netlify). You can use anytrhing you want, but I decided to go with Azure. Again, because I want to experiment with features in Azure for use with my Main Quest. 

Before creating our release process, we're going to need to create an Azure App Service to deploy to. I'm going to skim over this, but leave you with a few notes.

1. In your Azure portal, go to the App Services section and click the Add button, Select Web App
2. Go through the wizard to setup the web app
3. Once the web app has been provisioned by Azure, go to its settings view
4. Under Custom Domains you can buy your own domain. I did this for this blog and it was quite easy. My first domain too.
5. Under Scale Up, make any changes to use more or less powerful hardware. 
    * I use 'D1 Shared' for this blog to keep the cost reasonable. 
6. Make sure to turn the site off when you're just starting out and don't want anyone to navigate to it.


Now that we have our Azure App Service, we can create a new release in VSTS. Navigate to the Releases section and click the 'Create release definition' button. Choose the 'Azure App Service Deployment' option and then use the rest of the defaults. You'll see a single task in the definition that will take out output and send it on over the Azure. Just need to add a few settings so the task knows how to release to your App. Under the 'Package or Folder' option, select the output folder from our build we're release from.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/01/ReleaseDefinition.png" alt="My Release Definition" class="img-fluid">
  <figcaption>Only need a single task</figcaption>
</figure>

<div class="alert alert-warning">
  <p>This release goes right to production. In general you'll have multiple environments like Dev and Test. For a simple blog like this, I'll go directly to production until I need those other environments.</p>
</div>

Now that we have a release, we should make sure it is Deploying Continuously. Go to the Triggers tab and make sure Continuous Deployment is checked and you have the proper build selected. 

Now save this and queue up a release. In theory everything should go well and your site will be online hosted by Azure. 

## Summary

This is a very simple setup for a blog. Over time things will change and I'll make sure to update this entry with those changes. But starting a blog, the programmer way, can have a lot of tasks to go through. Again, there are many ways to run a blog. I wanted to go a route that was simple, but involved learning new technologies. Hopefully this was easy enough for you to follow and I didn't leave you with too many extra questions.

---
## *Quest Rewards*
- *1000 xp*
- *Blog Dependency*
- *Wyam Proficiency*
- *Markdown Proficiency*

---

### Links
* Scott Hanselman's Wyam Blog Post: http://www.hanselman.com/blog/ExploringWyamANETStaticSiteContentGenerator.aspx
* Wyam: https://wyam.io/
* Wyam Releases: https://github.com/Wyamio/Wyam/releases
* .Net Rocks Episode 1395: http://www.dotnetrocks.com/?show=1395
* Markdown Monster: https://markdownmonster.west-wind.com/
* Cake: https://github.com/cake-build/cake
* Cake Plugin to VSTS: https://marketplace.visualstudio.com/items?itemName=cake-build.cake

### Wyam Commands:
- Wyam new -r <recipe type>
    - Creates a new Wyam setup folder within the current directory
        - A single folder named 'input'
- Wyam -r Blog
    - Takes what's in the input folder and "builds" it to the output folder
- Wyam -r Blog -p
    - Will do the build and then start the Previewer
    - Previewer hosts the site
    - Defaults to http://localhost:5080/
- Wyam -r Blog -p -w
    - Does like before, but now watches for any file changes and will re-build so you can go right to the browser and see what things look like
    - Seems to build everything and restarts the server. So this can take a few seconds.

