Title: Converting This Site to Blazor
Published: 2019/01/13
Tags: 
- Blazor
- Web Development
- Blog
---

## Receiving the Quest
*You've been hearing about this new form of spell, WASM. It's a newer way to share your words over the enchanted spiders web, cast wide around the world. Previous ways to do this are not something you ever liked. But this new way is right up your alley.*

*You sit down and realize it takes some getting used to, as any new thing is. It's a nice mix of old and new that will be some getting used to. But it will be an interesting quest.*

## Another Change to the Blog Site?

Yep. After not writing a blog post for nearly a year I decided it was time to devote some love and attention to it. Because I use this site as something to play around with specific technologies, it's easy for it to end up in weird states. I went over the previous site architecture in [Making This Blog A Serverless App](/BlogPosts/04-Making-This-Blog-A-Serverless-App.md). But the short answer is that I used the static site generator tool Wyam to generate the site, and deploy that to Azure. 

Now that Blazor has gotten some steam going it felt like time to convert the entire site over to use it. This upgrade will also give me the chance to add some other features I've wanted to add for a while.

## What is Blazor?

[Web Assembly](https://webassembly.org/), aka WASM, is the W3C STANDARD for allowing the web browser to run compiled code, in addition to JavaScript. Blazor is the implementation from Microsoft for running .NET code through WASM.

Even though Blazor is relatively new and still in preview (only version 0.7 as of this writing), there are already tons of example/tutorials/outlines online for learning to use it. The official website, [Blazor.net](https://blazor.net/), is a great starting point AND the resource I used the most when starting my first project with it. Because of this, I won't go over too much of the technical details.

## App Models
There are two ways to host a Blazor app. A Hosted Deployment with ASP .NET Core, and a Standalone Deployment. The Hosted model is a lot more in line with how a traditional ASP .NET web app is created/run. With it, there is a server-side process that generates the dynamic site the end user sees. The Standalone model is a client-only site where the user downloads the static front end, and any dynamic stuff happens by running custom code from the client. For this site, I went with the Standalone model.

## Gotchas and Considerations

When jumping into Blazor, the first thing to realize is it's not like making a  usual native UI app running on Windows. The UI is still a website with HTML and CSS. Plus you can even include JavaScript if you want. Which you most likely you will because  we all already have so much code written in JavaScript.

The most important part to remember is that the code still follows ALL the same rules Javascript has to follow when running in the browser. Your code is still in a sandbox, and only has access to the APIs JavaScript code will have access to (no reading files from the user's hard drive).

## CORS

After getting my new project set up in Visual Studio with the default template the very first thing I wanted to do was add some C# code to download a file from an HTTP endpoint and display its text on the screen. The code for this is straightforward. Create an instance of HttpClient and run a GET request against the target endpoint. But what bit me immediately was that the code still had to follow CORS rules and the browser blocked my request. Since I own the endpoint the file was coming from I was able to change the CORS settings to allow anyone to make a GET request against it. But nevertheless, it was still as if I wrote the code in JavaScript.

## Json .NET

Another item of note was how to work with JSON. The usual way to do this in a .NET app is to use the Json .NET NuGet package. For a reason I chose not to look too far into, that package isn't working in Blazor yet. But remember, this is code running in the browser, and browsers are great at using JSON. In the end, I was able to deserialize JSON text by using the JavaScript interop code like so: `Microsoft.JSInterop.Json.Deserialize<BlogPostSummary[]>(recentDataText)`

## How Pages Load

If you open your browser's Developer Tools you'll see some standard files (HTML, CSS, etc), but now you'll see the compiled files used by WASM. In the case of Blazor, DLL files are downloaded like System.dll. Meaning the browser needs to download everything first, and then run the site. So if we do nothing, we're now back to the point where the user can't do anything until the entire page loads. In my own testing, if I clear my browsers cache and reload this website it takes about 9 seconds to get past the loading screen. This is definitely something that'll need to be improved for any production site. Amazon and Google learned this when slowing down their services and immediately seeing users leave their respective sites. See the article at Fast Company [here](https://www.fastcompany.com/1825005/how-one-second-could-cost-amazon-16-billion-sales).

## DLL Size

Every website needs to worry about the size of files downloaded by the user's browser. Its been a problem on the NPM side of things, and now we have it with our .NET code. So be on the lookout for it.

For now, I use the Markdig NuGet package to display the blog posts at runtime. The Markdig DLL is 338 KB and takes 1.56 seconds to download. Each blog post is currently stored as a markdown file and at runtime, it's downloaded and converted to HTML, then displayed on the screen. In the future, I'll probably change the blog files to be stored on the endpoint as HTML and remove my dependency on Markdig. Remember, [performance is a feature.](https://blog.codinghorror.com/performance-is-a-feature/)

## Other Considersations

Here are some other considerations I don't know how to deal with:
- Are WASM sites just as accessible as other sites? For example, when using screen readers or other accessibility technologies?
- How do search engine crawlers like Google treat WASM sites? Can they parse them yet? Should I add a site map file to let the crawler know about the different pages?
- I still don't know how to load specific JavaScript files only on certain pages. For example, as of right now I want to use Disqus for comments, but I don't know how to load the JavaScript for that just on pages. All JavaScript is loaded on index.html.

## View the Code
The code for this site is all hosted on GitHub [here](https://github.com/ProgrammerAl/ProgrammerAlSite) in case you're curious how I've implemented anything. 

## Summary

Blazor and WASM are *really* cool. Creating a site using those technologies is still early days, especially since Blazor is in beta. But I for one am very much interested in the future use of this technology.

---

## *Quest Rewards*
- *300 xp*
- *New and Improved Personal Website*
- *Less dependencies on external tools*

