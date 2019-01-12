Title: Continuously Deploying a NuGet Package (Through VSTS)
Published: 1/29/2018
Tags: 
- NuGet
- Continuous Deployment
- VSTS
---

### Receiving the Quest
*After some time creating a new spell component you realize you want to share it with the magical community. You package it up and share it with everyone over the publicly accessible New Gett enchantment. But then you make a change and realize manually sending into the ether is taking too much time. Mostly from rolling too low on the intelligence check with remembering the steps for sending it. You decide to invest in creating a new enchantment to automagically send a new version of this spell component to the ether once changes have been made.*

### Shameless Plug

I recently started my first Open Source project - [Command Complete](https://github.com/ProgrammerAl/CommandComplete). It's a .NET Standard library to allow you to add auto-completion to a command string with parameters. It's like auto-complete in PowerShell or other shell tools that add that, but something you can put into your own app. There's built-in support for using this within a console, and you can add some custom bits to use this in something else like a text field in some UI. 

After creating the project I wanted to add some automation to build/test/deploy the library as a NuGet package. I quickly got to a problem where versioning a NuGet package in a Continuous Deployment world is less than ideal.

### How to make a NuGet Package

We have a plethora of choices when we want to generate a NuGet package from our code. We can:
    - Publish from the command line
    - For a .NET Standard project, enable the 'Generate NuGet package on Build' option to create one every time it's compiled
    - In a VSTS build, use the '.NET Core' task with the 'pack' command selected
    - In a VSTS build, use the 'NuGet' task with the 'pack' command selected
    - In other automated pipelines that I know nothing about, use some other way that's probably very similar to the above two

This isn't the hard part. If you're not very familiar with generating a NuGet package feel free to [search the internet](https://www.bing.com/search?q=how+to+create+a+nuget+package). 

### The problem...from a Continuous Deployment Point of View

Uploading a NuGet package to a feed is easy. You can use the command line, or upload a package directly through the browser. Living in the future has its perks. The problem comes when following normal continuous deployment guides. 

When I'm speaking with people about setting up a CI/CD pipeline through TFS/VSTS I say an application will usually need a build and a release. The build is there to compile an app, and/or do whatever else to generate the artifact files. Then in the release, those artifact files are deployed to their target environments (dev/test/performance test/staging/prod/etc). 

When generating a NuGet package file, the version number gets set. *This is done at compile time.* It can't be changed later on. Not in any suppoted way at least. Let's say the generated NuGet package has a generated version number of 2.0.0-alpha-20180129. It gets tested internally and eventually trusted enough to toss out to some form of beta testing. The next logical step would be to upload that exact same package to the public Nuget server with a version number of 2.0.0-beta-20180129. Once that passes muster, we'll want it to be the official package with version number 2.0.0.

After each validation step the goal is to promote the exact same package file artifact to a new place with a version name targeted toward that environment. The bad news is this isn't an option right now. But the good news is Microsoft knows about this scenario and is working toward making this easier, as mentioned in a [blog post](https://blogs.msdn.microsoft.com/devops/2016/05/18/versioning-nuget-packages-cd-2/) 8 months before this one was published.

### The Workaround

Since we can't change the version number at deploy time I swallowed some pride and decided to involve a *gasp!* manual process. The image below shows the steps for the VSTS Build Definition. There is no Release Definition because the build will deploy the package. After compiling and running unit tests, the build will:
- Generate a NuGet package
- Publish the NuGet package to VSTS so we don't lose the files
- Push the package to an internal NuGet feed (I'm using the one provided by VSTS)
- If enabled, push the package to the public NuGet.org package feed

<figure>
  <img src="__StorageSiteUrl__/assets/images/postimages/06/NuGetBuild.png" alt="VSTS Build generating and publishing a NuGet package" class="img-responsive">
  <figcaption>Those bottom two are the important tasks</figcaption>
</figure>

The steps are pretty self explanatory. The only one that needs some explanation is the final step. This step will deploy the package to the publicly hosted NuGet.org feed, but it will do so only based off the result of a custom condition. That condition uses a build variable (IsPublishingToPublicFeed) defaulted to false. In order to deploy to the public feed, I will have to manually change it (and remember to change it) at queue time.

<figure>
  <img src="__StorageSiteUrl__/assets/images/postimages/06/PublishToPublicFeedCondition.png" alt="Condition for when to deploy to public NuGet.org feed is and(succeeded(), eq(variables['IsPublishingToPublicFeed'], true))" class="img-responsive">
  <figcaption>This will only run when I don't mess up at queue time</figcaption>
</figure>

Some other build variables are used for versioning that can be set at queue time. Specifically NuGetVersionNumMajor, NuGetVersionNumMinor, NuGetVersionNumPatch, and NugetVersionNumPrerelease. 

<figure>
  <img src="__StorageSiteUrl__/assets/images/postimages/06/BuildVariables.png" alt="Variables used for Build. Most notably NuGetVersionNumber and IsPublishingToPublicFeed" class="img-responsive">
  <figcaption></figcaption>
</figure>

As you probably guessed I really don't like the manual step. To me, manual means forgettable. I'll have to remember to set more than one variable at queue time, *and* if I've made any changes since the previous build I'll have to look for the right Git commit number to make sure the right code is compiled and deployed. In the few days since creating this build I've already messed up multiple times. Thankfully none of those mistakes caused any real problems, but it doesn't instill confidence.

### Summary

Since we don't have any tooling at the moment to change a NuGet package version after it has been created, we have to re-compile it to the version number we want. Thankfully automating this is easy, but it came with manual steps. Hopefully tooling gets changed for this sooner than later, but for now it's not horrible (even if I want to say it is).

---
### *Quest Rewards*
- *1000 xp*
- *NuGet Proficiency*
- *New OSS Project - Unrelated to this blog post, but it's what the NuGet package is for*

---

#### Links
* Microsoft blog post on versioning NuGet packages: https://blogs.msdn.microsoft.com/devops/2016/05/03/versioning-nuget-packages-cd-1/
* Microsoft blog post on not being able to rename a NuGet Version: https://blogs.msdn.microsoft.com/devops/2016/05/18/versioning-nuget-packages-cd-2/

