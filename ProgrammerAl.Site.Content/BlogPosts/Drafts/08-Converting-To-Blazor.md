Title: Converting This Site to Blazor
Published: 2/21/2018
Tags: 
- Blazor
- Web Development
---

### Receiving the Quest
*The spells you've been working on in recent times have all worked well enough. You test them and eventually put your full trust behind them. But you have this feeling in the back of your head that makes you think there's more you can do to trust what the spell is doing. Or, it's another spider bite.*

*You roll a d20 and suddenly realize what you've been missing. A good way to look at your custom spells and get a sense of how well they're crafted. No longer should you only rely on automating the testing of the spell itself. From now on you will analyze how the magical energies come together to ensure you can always update them.*

### Another Change to the Blog Site?

Yep. After not writing a blog post for nearly a year I decided it was time to devote some love and attention to it. Because I use this site as something to play around with specific technologies, it's easy for it to end up in weird states. I went over the previous site architecture in [Making This Blog A Serverless App](/BlogPosts/04-Making-This-Blog-A-Serverless-App.md). But the short answer is that I used the static site generator tool Wyam to generate the site, and deploy that to Azure. 

Now that Blazor has gotten some steam going it felt like time to convert the entire site over to use it. This upgrade will also give me the chance to add some other features I've wanted to add for a while.

### What is Blazor?

[Web Assembly](https://webassembly.org/), aka WASM, is the W3C STANDARD for allowing the web browser to run compiled code, in addition to JavaScript. Blazor is the implementation from Microsoft for running .NET code through WASM.

Even though Blazor is relatively new and still in preview (only version 0.7 as of this writing), there are already tons of example/tutorials/outlines online for learning to use it. The official website, [Blazor.net](https://blazor.net/), is a great starting point AND the resource I used the most when starting my first project with it. Because of this, I won't go over it too much of the technical details.

### Gotchas and Considerations

When jumping into Blazor, the first thing to realize is it's not like making a  usual native UI app running on Windows. The UI is still a website with HTML and CSS. Plus you can even include JavaScript if you want. Which you most likely you will because  we all already have so much code written in JavaScript 

The most important part to remember is that the code still follows ALL the same rules Javascript has to follow when running in the browser. Your code is still in a sandbox, and only has access to the APIs JavaScript code will have access to (no reading files from the user's hard drive).

After getting my new project set up in Visual Studio with the default template the very first thing I wanted to do was add some C# code to download a file from an HTTP endpoint and display its text on the screen. The code for this is straightforward. Create an instance of HttpClient and run a GET request against the target endpoint. But what bit me immediately was that the code still had to follow CORS rules and the browser blocked my request. Since I own the endpoint the file was coming from I was able to change the CORS settings to allow anyone to make a GET request against it. But nevertheless, it was still as if I wrote the code in JavaScript.

Another item of note was how to work with JSON. The usual way to do this in a .NET app is to use the Newtonsoft.JSON NuGet package. For a reason I chose not to look too far into, that package isn't working in Blazor yet. But remember, this is code running in the browser, and browsers are great at using JSON. In the end, I was able to deserializing JSON text by using the JavaScript interop code like so: `Microsoft.JSInterop.Json.Deserialize<BlogPostSummary[]>(recentDataText)`

ADD SOMETHING ABOUT LARGE DLLS HERE


There are other considerations I don't know how to deal with:
- Are WASM sites as accessible as other sites? For examepl, when using screen readers or other accessibility technologies?
- How do search engine crawlers like Google treat WASM sites? Can they parse them yet? Should I add a site map file to let the crawler know about the different sites?
- I still don't know how to load specific JavaScript files only on certain pages. For example, as of right now I want to use Disqus for comments, but I don't know how to load the JavaScript for that just on pages.


TO ADD:
- Large DLLs
- Mention how pages load
	- Meaning, dlls load the entire site, then it runs stuff. So no progressive loading like an HTML doc

### Summary

After playing around with SonarCloud a bit, I wish I could use it on every project I work on. The charts are pretty and it gives you a nice sense of how things are going for a project. Give it a try. Come one. All the cool kids are doing it. At least, that's what I'm told.

---

### *Quest Rewards*
- *200 xp*
- *Small spider bite. It should go away soon.*

