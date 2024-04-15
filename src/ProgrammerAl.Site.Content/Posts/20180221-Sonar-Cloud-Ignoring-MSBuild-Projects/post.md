Title: SonarCloud - Getting Started with VSTS and Bonus - Ignoring MSBuild Projects
Published: 2018/02/21
Tags: 
- SonarQube
- SonarCloud
- VSTS
- Blog
---

## Receiving the Quest
*The spells you've been working on in recent times have all worked well enough. You test them and eventually put your full trust behind them. But you have this feeling in the back of your head that makes you think there's more you can do to trust what the spell is doing. Or, it's another spider bite.*

*You roll a d20 and suddenly realize what you've been missing. A good way to look at your custom spells and get a sense of how well they're crafted. No longer should you only rely on automating the testing of the spell itself. From now on you will analyze how the magical energies come together to ensure you can always update them.*

## SonarQube

After a few years of wanting to play around with [SonarQube](https://www.sonarqube.org/), but not wanting to setup my own server just to use it, I was very excited to finally have A) a reason to use it ([see my shameless plug in this previous post](https://developersidequests.com/posts/06-Continuously-Deploying-A-Nuget-Package)), and B) [SonarCloud](https://sonarcloud.io/).

If you're not already familiar with it, SonarQube is open-source software for static file analysis on code files. It looks for potential issues, and tracks the quality of the code over time. It's quite nifty and works for just about every language. Take a minute to browse the site. Here's the [link again](https://www.sonarqube.org/).

The problem with SonarQube is that you need a server to install this on and make sure it's up and running when you want to analyze code. So either it's up and running 24/7, or you install it in a VM in a cloud environment and start/stop it when you need. Donovan Brown has a good example of doing this in Azure [here](http://donovanbrown.com/post/how-to-setup-a-sonarqube-server-in-azure). But this is not a great solution for the single developer writing code all by themselves.

## SonarCloud

[SonarCloud](https://about.sonarcloud.io/) is a, yep you guessed it, a cloud hosted service that runs SonarQube. It's incredibly easy to setup, and free for any open source projects like the ones you have on GitHub. So now we have something that doesn't require a dedicated machine to run ~24/7.

## CI with SonarCloud in VSTS

Using SonarCloud with my already existing CI builds in VSTS was really easy. The steps were:

1. Create a new account in SonarCloud and link it up with my GitHub profile
1. Get the SonarCloud extension from the [VSTS Marketplace](https://marketplace.visualstudio.com/items?itemName=SonarSource.sonarcloud) so I have the build/release tasks
1. Create a new Service Endpoint in VSTS to link up with SonarCloud
1. Add the three tasks to my build. "Prepare analysis on SonarCloud", "Run Code Analysis", and "Publish Analysis Result"

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/07/Sonar Cloud Build Tasks.png" alt="VSTS Build with SonarCloud tasks" class="img-fluid">
  <figcaption>Almost as easy as copy/paste</figcaption>
</figure>

After adding a few required settings I was getting some data in SonarCloud and everything was good to go.

<div class="alert alert-info">
  <p>**Update:**

The above comes from the documentation on SonarQube's website: https://docs.sonarqube.org/display/SCAN/Excluding+Artifacts+from+the+Analysis 

However what I previously missed is that you can force a project to be known as a test project with a slightly different property being set inside the *.csproj file detailed here: https://docs.sonarqube.org/display/SCAN/Miscellaneous+Advanced+Usages</p>
</div>

## Problems Excluding Projects from Analysis

The .NET solution I was trying to analyze had three projects. One was the main code, and the other two were meant for testing. I wanted to add a setting to make sure the tests were ignored. Everything I saw online said to use the `sonar.exclusions` setting. After setting it to `sonar.exclusions=**/*Test*` I found the code I wanted to exclude was still being analyzed. This was odd. My next step was to try the nuclear option of ignoring everything with `sonar.exclusions=**/*`, and oh boy did that work. So now we know the exclusions do work sometimes, just not the way I want them to.

Turns out the `sonar.exclusions` value is used *per project*. In fact, each project was analyzed separatly, not as a whole solution. This meant the `sonar.exclusions` value was not something I should be using in this case.

Eventually I found the solution online was to add an item to each of the *.csproj files I wanted to be ignored by SonarQube. 

```
<PropertyGroup>
  <!-- Exclude the project from analysis -->
  <SonarQubeExclude>true</SonarQubeExclude>
</PropertyGroup>
```
You can also see a code example [here](https://github.com/ProgrammerAl/CommandComplete/blob/master/CommandComplete/CommandComplete.UnitTests/CommandComplete.UnitTests.csproj).

With that, the two projects were ignored and the analysis charts looked better. Way better. I don't know if this is true for all project types SonarQube can analyze. But this does seem to be true for anything using MSBuild. 

## Bonus Sonar: SonarAnalyzer

Since we're here talking about code analyzing for .NET you should also know that if you want this kind of analysis in the IDE, you can use the proper SonarAnalyzer NuGet package for your language. Like [this one](https://www.nuget.org/packages/SonarAnalyzer.CSharp/) for C#. It's not as fully featured as SonarQube and doesn't give you charts to show data over time. But it has just about all of the same analysis rules, and implements Roslyn analyzers and code fixes to help you easily make changes to follow those rules. This is what I use for any non-open source projects I have.

## Summary

After playing around with SonarCloud a bit, I wish I could use it on every project I work on. The charts are pretty and it gives you a nice sense of how things are going for a project. Give it a try. Come one. All the cool kids are doing it. At least, that's what I'm told.

---

## *Quest Rewards*
- *200 xp*
- *Small spider bite. It should go away soon.*

