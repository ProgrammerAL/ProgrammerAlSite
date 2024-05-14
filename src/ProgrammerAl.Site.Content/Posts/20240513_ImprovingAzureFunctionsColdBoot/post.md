Title: Improving Azure Functions Cold Boot
Published: 2024/05/13
Tags: 
- blog
- Azure Functions
- Serverless
- Performance
---

## Why care about cold boot?

A while ago I saw the below tweet from Paul Yuknewicz mentioning that cold start for Azure Functions Apps has improved a lot lately. This is great because cold boot can be been a big pain for HTTP based Function Apps.

<blockquote class="twitter-tweet"><p lang="en" dir="ltr">Cold start in <a href="https://twitter.com/AzureFunctions?ref_src=twsrc%5Etfw">@AzureFunctions</a> improved ~53% across all regions and languages in last 18 months. Plus we have our own opinion how you can be even more efficient by customizing the concurrent number of events per instance. Learn from perf architect Hamid how this was done and what…</p>&mdash; Paul Yuknewicz (@paulyuki99) <a href="https://twitter.com/paulyuki99/status/1783695782887170297?ref_src=twsrc%5Etfw">April 26, 2024</a></blockquote> <script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>

## What is cold boot?

Cold boot is when an application starts up for a request after being shut down for not being in use. With so much of our cloud infrastructure, we live in an on-demand world. That means if something isn't being used, turn it off. The down side to that is that when it turns back on, there's an added delay. And that's cold boot!

Consumption based Azure Function Apps will shut down after around 5 minutes of non-use. So lets check out how much that affects the app, and what we can do to to improve the start time. Paul's tweet above has a link to a page with tips for improving cold boot time.

## Base Sample

To start I used the Azure Portal to create the below services. Unless otherwise specified, the default options were used. 
- Azure Storage Account
    - V2 storage
    - Locally-Redundant Storage
- Azure Function App
    - Consumption Plan
    - Runtime: .NET 8
    - Operating System: Linux
    - Storage Account: Link to the one created above
    - App Insights: Off

After creating the Function App instance I looked at the pre-generated Environment Variables. Those are config settings so the Function App knows where to download the zip package from in order to run the code. Except for `WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED` which is a new variable specific to Azure Functions. Microsoft's documentation says it does some kind of optimization. Which is not helpful, but I guess we'll leave it on. 

![New Azure Function Environment Variables](__PostImagesUrl__/NewAzFuncConfig.png "New Azure Function Environment Variables")

After that I opened Visual Studio and created a new Azure Functions project with default values for an HTTP Triggered, .NET 8, Isolated Functions project.

## Publishing and Testing

Some of these tests will involve manually editing the zip package pushed to Azure. So I chose to create a publish profile in Visual Studio, publishing to my local machine. Before uploading to Azure I move all of the files from my `/bin/Release/net8.0/publish` directory to a new zip file.

When deploying the app using a zip package you must set the `WEBSITE_RUN_FROM_PACKAGE` environment variable on the Function App. The value to that environment variable is the URL the Function App runtime will download your app from. For these tests I used a SAS tokento the zip package, until said otherwise.

As an example, here is the SAS token I used for the `WEBSITE_RUN_FROM_PACKAGE` variable: `https://alrodrifuncscoldbootexpr.blob.core.windows.net/my-funcs/funcs.zip?sp=r&st=2024-05-08T22:10:20Z&se=2024-05-10T06:10:20Z&spr=https&sv=2022-11-02&sr=b&sig=QfBzOYAwAhYvJR6%2BXEXXM4dKv41al1kbyfWdnm0zxNw%3D`

There's no easy way to test cold boot time. I created a small app that will do the below steps. The app was run from my local machine, and the code is in the final section of this post if you want to look it over.

- Stop the Function App
- Wait 10 seconds
- Start the Function App
- Wait 60 seconds
- Loop 10 times
  - Wait 6 minutes
  - Send a GET request to an endpoint in the Function App

I read somewhere once that it takes about 5 minutes for a Function App to be taken down by Azure. So the 6 minute wait is to be sure the Function App has been deallocated in Azure. Plus this will simulate a more realistic cold boot scenario since usually apps cold boot after being deallocated, not from a restart.

Finally, it's important to remember that we're testing cold boot time of a Function App. What our app does at startup is our own issue, not Azure's. That's why we will be testing with a minimal Function App.

## Test 1: Defaults

The default published version of the app contains 54 filaes and is 5.67 MB, or 2.4 MB when zipped. Deploying the app and testing, we get the below timings. Ranging from 1.3 seconds to 3.2 seconds. 

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 2,250 |
| 2 | 2,130 |
| 3 | 1,901 |
| 4 | 1,913 |
| 5 | 1,780 |
| 6 | 2,448 |
| 7 | 1,510 |
| 8 | 1,396 |
| 9 | 3,214 |
| 10 | 1,823 |

## Test 2: Delete Unnecessary Files

Part of the cold boot is network activity. The Function App runtime needs to download the zip package, and unzip the files. So the theory is the less bits we have to transfer over the network, the better.

I've seen a handful of Function App projects that include PDB files in the output. This default app only has 1. The published app also has a `runtimes` folder which has some files for Windows and Linux. Since we know this will only run on Linux, I deleted the Windows folder. This removed 5 dlls.

The new version has 48 files for a total size of 5.67 MB, which is 2.36 MB when zipped. With a savings of 0.04 MB I don't expect much of an improvement, if any.

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 1,899 |
| 2 | 1,855 |
| 3 | 1,420 |
| 4 | 1,852 |
| 5 | 2,451 |
| 6 | 1,844 |
| 7 | 1,426 |
| 8 | 2,792 |
| 9 | 2,538 |
| 10 | 2,023 |

That is pretty much the same. Ranging from 1.4 to 2.7 seconds. I guess it averages to being a little better than the previous test, but it's not something a human would notice.

## Test 3: Ready to Run

Ready to Run is a compiler flag to pre-compile the JIT code for a platform. This saves on startup time because the CLR does not need to compile the JIT code to native code. 

To be clear this is a type of AOT and not the AOT everyone has been talking about recently. Ready to Run has been around in the .NET world for a long time. I don't know the exact reasons why this is different from AOT, because there are similarities, but I know it's different. Also, the sole purpose of Ready to Run is to improve startup time. Anyway, the only change I made to my `csproj` file is by adding the below code, then ran publish like normal.

```xml
<PropertyGroup>
	<RuntimeIdentifier>linux-x64</RuntimeIdentifier>
	<PublishReadyToRun>true</PublishReadyToRun>
</PropertyGroup>
```

Now the published app moved up to 8.89 MB, which was 3.88 MB when zipped. It also does not have the `runtimes` folder, so no need to delete anything from there. There was 1 pdb file, which I elected to not delete because it's small and I forgot to do it before starting the test.

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 2,041 |
| 2 | 2,229 |
| 3 | 1,430 |
| 4 | 2,486 |
| 5 | 2,327 |
| 6 | 1,309 |
| 7 | 1,703 |
| 8 | 1,353 |
| 9 | 2,467 |
| 10 | 2,300 |

This is a little better than the default, but not by much. Ranging from 1.4 to 2.4 seconds. I was hoping for more.

## AOT

With the release of .NET 8, AOT is all the rage these days. So let's try it by adding the below to the `csproj` file, and removing what we added for Ready to Run in the previous test.

Note: I had to use WSL to compile to Linux.

```xml
<PropertyGroup>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <PublishAot>true</PublishAot>
</PropertyGroup>
```

When published the output is 12 files at 103 MB, which was 32.9 MB when zipped.

Unfortunatly I was not able to get this to run. I guess AOT isn't supported for .NET 8 Azure Functions on Linux right now? I tried a handful of different ways to get this to run and could not find any documentation online. So I'll ignore it for now. If this is supported in the future I may remember to update this.

## Other Flags

There are some other flags you can use in .NET projects to make the final set in files smaller. PublishTrimmed, and SelfContained are the two I see talked about a lot. Unfortunatly, like AOT, they also did not work when I published with those flags on. So no testing there!

## Loading Functions with a Managed Identity

Quick refresher. When the Function App loads the executables for the Azure Function, it downloads the zip file from a blob in an Azure Storage Account. Up until now, we had the Function App was use a SAS token for the authorization to be allowed to download the file. 

In this test we will be using a Managed Identity on the Function App and granting it the Blob Storage Contributor permissions on the storage account so it can get zip file.

If you're unfamiliar with Managed Identities I highly reccomend you look into them. I've written about this in the past [here](https://programmeral.com/posts/20240409-WhyAzureManagedIdentitiesNoMoreSecrets), but that was from the application level, not for this specific scenario. But the concept is the same.

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 14,456 |
| 2 | 11,360 |
| 3 | 20,668 |
| 4 | 15,118 |
| 5 | 14,986 |
| 6 | 14,798 |
| 7 | 18,868 |
| 8 | 21,792 |
| 9 | 15,143 |
| 10 | 11,428 |

Those timings are CRAZY. Over 10x to 15x slower than using a SAS token. I never would have expected this. 

Pure speculation here, I assume the underlying system that gets the auth token is making a call to some other system and that call is what's running slow. But I have no idea if that's true. Again, just guessing. If you know why this is so much slower, please reach out to me on some form of social media (links on the About page!).

## A Real Life Example

I have a personal project that is all grown up and become a full application. Here are some stats about the it:

- .NET 8 Isolated
- Uses Managed Identity to load application from zip
- Runs on Linux
- NOT Ready to Run
- 76.5 MB unzipped and 26.4 MB zipped
- 234 Files
  - 7 of those are Windows specific .DLLs
  - 78 of those are .DLLs (not sure why these are here, likely from a 3rd party Nuget, another thing for me to)
  - 2 of those are .PDBs

Let's go ahead and test it 3 times. The first to get a baseline, the second will remove the Managed Identity, and the third time to test moving the Ready to Run.

Here are the baseline test results:

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 17,772 |
| 2 | 15,779 |
| 3 | 11,739 |
| 4 | 15,846 |
| 5 | 11,431 |
| 6 | 21,528 |
| 7 | 15,874 |
| 8 | 15,570 |
| 9 | 16,940 |
| 10 | 16,117 |

Those numbers are bad. But it's what we expect considering it's still using a Managed Identity to get the zip file. The next test is without the Managed Identity for getting the zip file (it still has a Managed Identity for certain operations).

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 8,354 |
| 2 | 588 |
| 3 | 9,192 |
| 4 | 6,664 |
| 5 | 7,358 |
| 6 | 7,751 |
| 7 | 567 |
| 8 | 7,389 |
| 9 | 9,237 |
| 10 | 7,205 |

Those numbers are better, but still not great. Two of them are less than 1 second, but considering the rest of the numbers, I feel like it's best to ignore those. The 7-9 second range is half of what we got in the first test. This is an upgrade. 

For the next test we'll add Ready to Run to the project. This updates the project to 228 files at 121 MB, or 48.5 MB when zipped. This means the zip file is twice the size of the previous test, which is similar to what we saw when we added Ready to Run to the default project.

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 6,768 |
| 2 | 6,478 |
| 3 | 6,914 |
| 4 | 9,404 |
| 5 | 8,741 |
| 6 | 8,223 |
| 7 | 6,658 |
| 8 | 7,141 |
| 9 | 6,378 |
| 10 | 7,810 |

Those numbers are just barely better than before. In my case, it looks like adding Ready to Run didn't improve the cold boot by much. If I were to guess, it's because of the amount of startup code that gets run. There is a lot in this project. It looks like any more gains in this project will be from an code changes.

## Test Code

Below is the code I used to test the cold boot timings. There are some hard coded strings in there, so make sure to change those values if you plan to run this code yourself. I have already deleted the resource group these services were hosted in, so don't think you'll mess with my experiment in some way.

```csharp
using System.Diagnostics;

const int StopWaitSeconds = 10;
const int StartWaitSeconds = 60;
const int BetweenTestsWaitSeconds = 360;//6 minutes

var watch = new Stopwatch();
var client = new HttpClient();
client.Timeout = TimeSpan.FromMinutes(6);

var stopArgs = new ProcessStartInfo
{
    FileName = @"C:\Program Files (x86)\Microsoft SDKs\Azure\CLI2\wbin\az.cmd",
    Arguments = "functionapp stop --name experiment-cold-bool-az-funcs --resource-group experiment-az-funcs-cold-boot",
    CreateNoWindow = true,
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
};

var startArgs = new ProcessStartInfo
{
    FileName = @"C:\Program Files (x86)\Microsoft SDKs\Azure\CLI2\wbin\az.cmd",
    Arguments = "functionapp start --name experiment-cold-bool-az-funcs --resource-group experiment-az-funcs-cold-boot",
    CreateNoWindow = true,
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
};

var timings = new List<TestResult>();

Console.WriteLine("Stopping Function App");
var stopProcess = new Process()
{
    StartInfo = stopArgs
};
stopProcess.Start();
await stopProcess.WaitForExitAsync();

Console.WriteLine($"Waiting {StopWaitSeconds} seconds for stop to complete");
await Task.Delay(TimeSpan.FromSeconds(StopWaitSeconds));

Console.WriteLine("Starting Function App");
var startProcess = new Process()
{
    StartInfo = startArgs
};
startProcess.Start();
await startProcess.WaitForExitAsync();

Console.WriteLine($"Waiting {StartWaitSeconds} seconds to start to complete");
await Task.Delay(TimeSpan.FromSeconds(StartWaitSeconds));

for (int i = 0; i < 10; i++)
{
    var runId = i + 1;
    Console.WriteLine($"Testing Run: {runId}");

    //I've been told Azure Functions a brought down after 5 minutes
    //  Wait 6 minutes to be sure it's down
    Console.WriteLine($"Waiting {BetweenTestsWaitSeconds} seconds for app to be shut down");
    await Task.Delay(TimeSpan.FromSeconds(BetweenTestsWaitSeconds));

    watch.Restart();
    var httpResult = await client.GetAsync("https://experiment-cold-bool-az-funcs.azurewebsites.net/api/Function1?");
    watch.Stop();

    if (!httpResult.IsSuccessStatusCode)
    {
        var content = await httpResult.Content.ReadAsStringAsync();
        throw new Exception($"Exception result from endpoint with status code {httpResult.StatusCode}: {content}");
    }

    Console.Write($"Completed test ");
    var result = new TestResult(runId, watch.Elapsed);
    DisplayResult(result);
    timings.Add(result);

    Console.WriteLine();
    Console.WriteLine();
}

Console.WriteLine("Completed with timings:");
Console.WriteLine("| Test Id | Time in ms |");
Console.WriteLine("| :---------: | :-------: |");
foreach (var timing in timings)
{
    DisplayResult(timing);
}

static void DisplayResult(TestResult result)
    => Console.WriteLine($"| {result.TestNumber} | {result.Result.TotalMilliseconds:#,#} |");

public record TestResult(int TestNumber, TimeSpan Result);
```


