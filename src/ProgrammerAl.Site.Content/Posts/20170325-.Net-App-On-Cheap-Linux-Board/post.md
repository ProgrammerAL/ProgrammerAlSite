Title: "Running a .Net app console app on an 'Embedded' linux board"
Published: 2017/03/25
Tags: 
- .Net MicroFramework
- .NET
- Embedded
- Blog
---

## Receiving the Quest
*You've been working on a new spell recently. The main spell is simple. Something you've worked on before in one form or another over the years. But this one is different. It requires enchanted items to interact with the main incantation. The enchanted items have some requirements. But above all else, they must be powerful enough to contain your custom spells.*

*Below are your findings of items you can enchant for your new spell.*

## Choosing an Embedded Board

For a while now I've been working on a side project that I hope to someday sell as a full product. I don't want to go into details very much, but the basic idea is that it's a network connected toy for a game. Each user will need this device for the game, which is controlled by some server software running on some other smarter device (smartphone/tablet/etc). So the first question is, "What should run the software for this board?" Well as a software guy I had no idea where to start. Up until now my only experience with real embedded development was blinking an LED.

<div class="alert alert-info">
  <p>In the winter I get hit with static shock so often it becomes habit reach out and touch metal to force the shock sooner. Thus my genuine fear of exposed wires.</p>
</div>


### Option 1) Raspberry Pi

The first option many people think of is the Raspberry Pi. You can get one for as cheap as $35. [Unless you're buying it as a kit, of course.](https://www.amazon.com/CanaKit-Raspberry-Ultimate-Starter-WiFi/dp/B00G1PNG54/?keywords=canakit+raspberry+ultimate+starter) These are great boards with a ton of support from the organization and community. Unfortunately there are a couple of problems with the board for my specific use case. It's too big, and relies on extensions for some extra functionality, [called hats](https://www.raspberrypi.org/blog/introducing-raspberry-pi-hats/), like Wi-Fi. It's amazing for other uses, just not right for what I want.

### Option 2) Something Running .Net MicroFramework

I've spent some time in the past playing with the .Net MicroFramework on a few boards. If you're not familiar, it's a version of the .Net CLR for running on embedded boards similar to an Arduino. The most popular board is the [Netduino](http://www.netduino.com/), but there are a handful of others out there too. The toolset was built to work seamlessly with Visual Studio and you can even attach the debugger and go line-by-line. Since I already had a handful of these boards around it was easy to get started. 

I actually got pretty far with the application code with .Net MicroFramework. But eventually got hung up by the lack of updates. Microsoft hasn't been very vocal about work on this and the last release was version 4.4 in October 2015. This lead me to start looking for another solution.

### Option 3) C.H.I.P. Linux Board

In the end I found the [C.H.I.P. Linux Board](https://getchip.com/). It's a $9 open source (hardware and software) board that runs a version of Debian Linux. My current thinking is I'll either be able to buy a lot of these in the future for my own device, or manufacture my own with the open source schematics [hosted on GitHub](https://github.com/NextThingCo/CHIP-Hardware). For the time being I'll be working with the board for all "embedded" development. It has Wifi and Bluetooth built in, and exposes some pins for extension.

Since the C.H.I.P. is running a full Linux OS, we get a handful of features for free compared to traditional embedded development.

### Working with the C.H.I.P. 

So .Net Core is cross-platform (yay). But that's only true while you're using a supported platform, similar to running Java applications. As of this writing it is not supported running on an [Arm CPU on Debian Linux](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog). The good news is [Mono](http://www.mono-project.com/) does support this. 

<div class="alert alert-info">
  <p>If you're unfamiliar with Mono, it's an open source runtime of the CLR. Great for taking compiled .Net code and running it on some other supported platform. Mono is also the basis for Xamarin applications.</p>
</div>

<div class="alert alert-info">
  <p>Pro Tip: The C.H.I.P. board doesn't come with an easy way to connect  to a monitor. I bought the HDMI adapter that attaches directly to the board. If you buy one of these boards, make sure you browse [the store](https://getchip.com/pages/store) for any extras that can make working with this a little easier.</p>
</div>


So how to we get our code to run on this device? Well the steps in and of themselves are fairly easy. It's basically:

- Flash the board
- Install Mono on the board
- Create your .Net application and compile it
- Use Mono to run the application on the C.H.I.P.

Looks easy right? Well lets go through the steps.

## Flashing the C.H.I.P.

Before we can use the C.H.I.P. board we need to put the most up to date software on it. There are a couple of ways to flash the board, but I went with the most user friendly option, a [Chrome Extension](http://flash.getchip.com/). After that's installed, the page will guide you through the steps on how to flash the board itself. Just make sure to have a safety pin ready.

## Installing Mono on th C.H.I.P.
Installing Mono is pretty straightforward. Just follow the instructions here: http://www.mono-project.com/docs/getting-started/install/linux/ 

Because the C.H.I.P. board uses a version of Debian Linux, we just need to follow the Debian install instructions. Assuming you chose a version of the OS that has a GUI, you can open that link on the C.H.I.P. itself and then copy/paste the apt-* commands into a terminal window. Otherwise you're typing it out by hand.

<div class="alert alert-warning">
  <p>Pro Tip: The instructions page has a section about libgdiplus. Be sure to follow those instructions or we'll have issues running .Net applications and you'll lose 6 hours of your life trying to debug the issue. Also, 6 is an arbitrary number. I only lost 4.</p>
</div>

## Create your .Net Application and Compile It
Depending on how far you went with setting up your linux board you may have installed Mono Develop to create the entire application on the C.H.I.P. board itself. This is not what I did. Instead I created an entirely new project through Visual Studio on my Windows desktop. A simple Hello World application works. I made a .Net Core console app, but a simple .Net Framework console app will also do. 

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/02/NewApp.png" alt="New App in Visual Studio 2017" class="img-fluid">
  <figcaption>As long as it's a Console app you're good</figcaption>
</figure>

From here you can go ahead and compile your console app and call it a day....Or can you? Probably not.

## Publishing a .Net Application with Mono
Remember that Mono has its own implementation of the .Net Framework. So if our application only references the .Net Framework and our own projects, things are good, our code will run, and we can go about our day doing a little Irish jig. But what if we start referencing other code that isn't implemented by Mono? The runtime won't know where to get the code to run. This is true for just about every application we'll write. To see what I'm talking about, add a reference to any NuGet package and see how many other references are added that aren't included in your build output files.

Here's what you're prompted with when adding a NuGet reference to Json.Net 10.1
<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/02/AddingJsonNetNugetReference.png" alt="Adding Json.Net Nuget Reference" class="img-fluid">
  <figcaption>Lots of references. Mono won't know how to use all of these by default.</figcaption>
</figure>

Here's the code we'll be using as an example:
<script src="https://gist.github.com/ProgrammerAl/586a2e2ac4479ecff403a818b8b2580a.js"></script>


Now that we have some other references, build the application and look at the output files created.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/02/DebugBuildOfApp.png" alt="Build Output" class="img-fluid">
  <figcaption>Notice all we have is the build output of our application. No dlls for Json.Net or anything else.</figcaption>
</figure>

This is where we'll need to publish the application. Because I made my app with .Net Core, I'll be turning to the command line to use the Dotnet Publish command.

<div class="alert alert-info">
  <p>There are a lot of useful commands you can use from the command line. The Dotnet Publish command is here: https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-publish. Just remember to have the dotnet core stuff installed on your machine from here: https://www.microsoft.com/net/download/core</p>
</div>

Open a command line to the directory with the project file and then run the command 'dotnet publish' to generate an output with everything required to run your application. 

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/02/RunPublishCommand.png" alt="Dotnet Publish command" class="img-fluid">
  <figcaption>Something like this will be output. Anything else is probably an error.</figcaption>
</figure>


<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/02/PublishOfApp.png" alt="Published Output" class="img-fluid">
  <figcaption>Notice there's way more now in the 'publish' folder.</figcaption>
</figure>

## Use Mono to run your application
Now we can take what's in the published folder and move it over to the C.H.I.P. board. Move the files however you want. I went with a trust USB drive. Once the files are on the C.H.I.P. open up a terminal window and move to the directory you placed your .Net app. Once there just use Mono to run it. I used: mono ExampleApplicationMadeOnWindows.dll

## Summary

Small linux boards like the C.H.I.P. are really starting to become more and more popular andsaving a lot of people the headaches of doing 'real' embedded development. For the kind of application I'm working on this is perfect. The cost is right and the specs are probably more than enough. I'll keep an eye out for others and see if anything really one-ups the C.H.I.P. but for now this is my favorite.

---
## *Quest Rewards*
- *30 xp*
- *2 C.H.I.P. boards*
- *Less space on my desk*

