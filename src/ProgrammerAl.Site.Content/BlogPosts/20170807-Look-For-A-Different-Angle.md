Title: Look for a Different Angle
Published: 2017/08/07
Tags: 
- NuGet
- C++
- Big Hero 6
- Google Protobuf
---

## Receiving the Quest
*There's a new spell you've been working on. It'll be so cool. Some may even call it super cool. But first, you need to dust off your skills on some spell technologies you haven't used in a long time. You're so used to working with spells abstracted at such a high level you know this will take some time to get used to.*

*So you dust off some old scrolls and get to work. You struggle and more time passes than it should have. You want to give up, but you can't. You need to solve this problem. But the question is, do you need to solve the problem this way? Or can it be done some other way?..*

## The Back Story

I've mentioned a side project of mine a few times in previous posts. This project will have an app running on a smart device (phone/tablet app, gaming console, etc) to act as a server. It will then need to wirelessly interact with a lower level device (call it IoT if you want) to send/receive messages. After some research and playing around with things, I decided to use Google's Protobuf technology for the messaging layer. If you've never heard of this technology, or played with it, it's a pretty nice way to generate some cross-platform messaging. You define one or more *.proto files that defines what fields each message has. Once you have that, you run a command line tool to generate some code for A) the message class itself, and then B) the serialization/deserialization code for the message in whatever languages you want to use. In my case, I'm using C# for the smart app and C++ for the embedded app. I may go more into this in a future blog post, but don't hold your breath.

## The Problem

I'm a .Net developer who hasn't done much in other camps in quite a long while. Its been about 6 years since I've done any real development in C++ and have to have my browser ready to search for things like 'C++ interfaces' or 'C++ how to reference static libs' or 'C++ how to ...' The later of which was the most common search.

So just like any other day I opened up Visual Studio, created a new C++ project, and added a reference to the Google Protobuf NuGet package. And then I moved on with my life and lost no hair that day.

<div class="alert alert-info">
  <p>Pro Tip: NuGet does support native references for C++ projects like this. </p>
</div>

At least, that's how I wish I could tell the story. In reality the NuGet package at the time was a lower level version than what I needed. The NuGet package was version 2.Something.SomethingElse but the C++ code generated had a requirement for 3.Something.SomethingElse as you can see here:

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/03/CppProtobufVersionRequirement.png" alt="C++ Protobuf Version Requirement" class="img-fluid">
  <figcaption>At least it's a compile time requirement</figcaption>
</figure>

## Down the Rabbit Hole

So now the obvious solution was to make my own NuGet package. Visual Studio Team Services has a feature for hosting private package feeds for [NuGet and NPM](https://www.visualstudio.com/en-us/docs/package/overview), and that's on my list of things to play with anyway, so why not? 

I've never made a NuGet package, but that's not the sort of thing that's stopped me before. So I look up how to make a package, and find some documentation....and it's not the best documentation. The .Net side is pretty detailed, but native is a bit lacking and there isn't an easy tutorial to follow. Okay, that happens sometimes but it's nothing new. The next step, look at some other native NuGet packages and reverse engineer what I'll need. I find a few and have something to work from. Now we're making progress. 

So I have some guesses at what's needed. Awesome, step one more or less complete. Still not sure this will work in the end, but there's something. After combining the commands shown on the [Protobuf GitHub site](https://github.com/google/protobuf/blob/master/src/README.md) for compiling the code with some custom commands for downloading the git repo and copy/pasting the required files somewhere else, we have an automated script. 

Finally, a NuGet package has been made. Now to use it. I upload this to VSTS and then add a reference to it in Visual Studio. Hope this works!...It doesn't. In the end I never figured out how to get that working and decided on a different approach. 

This was two weeks of wasted time. Granted, it's a side project and wasn't every day, but it was at about 10-15 hours of time gone with nothing to show for it. Which leads me to the point of this blog post. 

## Moving On

If you're familiar with the 2014 Disney movie Big Hero 6, you'll recognise the scene where Tadashi Hamada helps his little brother think of a solution to a problem by forcing him to think from a new angle. Put another way, stop your current train of thought and think of a different way to solve the problem.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/03/TadashiThinkFromANewAngle.gif" alt="Tadashi helping Hiro think of a new idea" class="img-fluid">
  <figcaption>I don't know how helpful it is to make someone think of a grand idea on the spot while upside down, but in this case I guess it worked out</figcaption>
</figure>


## The Solution

In the end the solution was staring at me right in the face the entire time. The text on the GitHUb site gave me the answer. Just add a local reference to the code.

<figure>
  <img src="__StorageSiteUrl__Assets/Images/BlogPostImages/03/HowToReferenceProtobufCpp.png" alt="GitHub Protobuf guide to reference the C++ library" class="img-fluid">
  <figcaption>It's like being given the answers to a test, but ignoring it because it's written in comic sans</figcaption>
</figure>

I did initially think about doing this, but didn't want to manually keep track of the versions or deal with any other weirdness that might pop up. Coupled with the fact that this was something I didn't normally do, it was tough to go down this path.

In the end the solution was to have a script that would check for the latest version of the Protobuf releases, download the git repo, compile it, and then copy/paste the code to a specified folder. Then, on the Visual Studio side, there's a pre-build script that'll run to check that we have some sort of Protobuf lib to reference and throw a compile error if that doesn't happen (okay, the latter part hasn't been done yet, but it will be). And bam, now there's a solution that's easy to reproduce and easy for anyone new to start working with. Plus, I don't have to check-in the Protobuf code to my own repository. 

## The Lesson

But the solution isn't what's important. The important thing is to remember that there is often a better solution that we don't even consider because in our heads we already have a viable, working answer to the problem. Sure, there are some quirks here and there, but it gets the job done. And that's the trap we all fall into. 'It gets the job done,' is a very scary phrase because it stops us from improving. 'It gets the job done *quick*,' or 'It gets the job done *efficiently*,' or 'It gets the job done *right*,' are the things we should be aiming for. Just like any kind of performance tuning in software development, we should always be measuring our processes to know what can/should be improved. Otherwise we're just guessing and hoping for the best. And where does that get us if we're just guessing? About 30 calls deep into a stack trace hoping we can just blame the framework for our own inadequacies.

## Summary

No matter how much time you've spent working on the wrong solution, you can always turn around. We fight existing solutions on such a regular basis because we don't even consider a better way even exists. I'm sure we all have plenty of examples of this that have nothing to do with software development. At least this experience has reminded me to look at the other things in my life and reconsider if there's an easier way. At least for the next week or so. If only there was a better way to remind myself about things like this.

---

## *Quest Rewards*
- *20 xp*
- *Not an extra dependency*
---

### Links
- [Google Protobuf Github Page](https://github.com/google/protobuf)
- [VSTS Package Management](https://www.visualstudio.com/en-us/docs/package/overview)
