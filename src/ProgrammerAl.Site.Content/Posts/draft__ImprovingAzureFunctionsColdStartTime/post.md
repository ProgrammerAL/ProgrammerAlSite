Title: Improving Azure Functions Cold Boot Time
Published: 2024/05/03
Tags: 
- blog
- Azure Functions
- Serverless
- Performance
---

## ???

A while ago I saw the below tweet from Paul Yuknewicz mentioning that cold start for Azure Functions has really improved lately. 

<blockquote class="twitter-tweet"><p lang="en" dir="ltr">Cold start in <a href="https://twitter.com/AzureFunctions?ref_src=twsrc%5Etfw">@AzureFunctions</a> improved ~53% across all regions and languages in last 18 months. Plus we have our own opinion how you can be even more efficient by customizing the concurrent number of events per instance. Learn from perf architect Hamid how this was done and what…</p>&mdash; Paul Yuknewicz (@paulyuki99) <a href="https://twitter.com/paulyuki99/status/1783695782887170297?ref_src=twsrc%5Etfw">April 26, 2024</a></blockquote> <script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>

## What is Cold Boot?


## Base Sample

To start I used the Azure Portal to create the below services. Unless otherwise specified, the default options were used. 
- Azure Storage Account
    - V2 storage
    - Locally-Redundant Storage
- Azure Function App
    - Consumption Plan
    - Runtime: .NET 8
    - Operating System: Linux based
    - Storage Account: LInk to the one created above
    - App Insights: Off

After creating the Azure Function App instance I looked at the pre-generated Environment Variables. Those variables are setting where the function can read the zip package from in order to run the Azure Function App code. Except for `WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED` which is a new variable specific to Azure Functions. Microsoft's documentation says it does some kind of optimization. Which is not helpful, but I guess we'll leave it on. 

![New Azure Function Environment Variables](__PostImagesUrl__/NewAzFuncConfig.png "New Azure Function Environment Variables")

After that I opened Visual Studio and created a new Azure Functions project with default values for an HTTP Triggered, .NET 8, Isolated Functions project.

## Publishing and Testing

Some of these tests will involve manually editing the zip package pushed to Azure. So I chose to create a publish profile in Visual Studio publishing to my local machine. Before uploading to Azure I move all of the files from my `/bin/Release/net8.0/publish` directory to a new zip file.

When deploying the app using a zip package you must set the `WEBSITE_RUN_FROM_PACKAGE` environment variable. This is the URL to the file the Azure Function runtime will load your app from. For these tests I used a SAS token to the zip package I uploaded to my storage location. The final test uses a Managed Identity. 

As an example, here is the SAS token I used for the `WEBSITE_RUN_FROM_PACKAGE` variable: `https://alrodrifuncscoldbootexpr.blob.core.windows.net/my-funcs/funcs.zip?sp=r&st=2024-05-08T22:10:20Z&se=2024-05-10T06:10:20Z&spr=https&sv=2022-11-02&sr=b&sig=QfBzOYAwAhYvJR6%2BXEXXM4dKv41al1kbyfWdnm0zxNw%3D`

There's no easy way to test boot time. I created a small app that will do the following:

- Stop the Function App
- Wait 10 seconds
- Start the Function App
- Wait 60 seconds
- Loop 10 times
  - Wait 6 minutes
  - Send a GET request to an endpoint in the Function App

I read somewhere once that it takes about 5 minutes for a Function App to be taken down by Azure. So the 6 minute wait is to be sure 

I ran this from my local machine. The code for this is below in the final section of this post. 

Finally, it's important to remember that we're testing cold boot time of a Azure Function App. What our app does at startup is our own issue, not Azure's. That's why we will be testing with a minimal Azure Function App.

## Test 1: Defaults

The default version of the app is 5.67 MB, or 2.4 MB when zipped. It also contains a total of 54 files. Deploying the app and testing, we get the below timings. Ranging from 1.3 seconds to 3.2 seconds. 

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

Part of the cold boot is network activity. The Azure Function App runtime needs to download the zip package, and then unzip the files. So the theory is the less bits we have to transfer over the network, the better.

I've seen a handful of Azure Functions projects that include PDB files in the output. The default app seems to only have 1. The published app also has a `runtimes` folder which has some files for Windows and Linux. Since we know this will only run on Linux, I deleted the Windows folder. This removed 5 dlls.

The new version has 48 files for a total size of 5.67 MB which is 2.36 MB when zipped. With a savings of 0.04 MB I don't expect much of an improvement, if any.

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

That is pretty much the same. Ranging from 1.4 seconds to 2.7. I guess it averages to being a little better than the previous test.

## Test 3: Ready to Run

Ready to Run is a way to compile the application to pre-JIT the application. This saves on startup time because the CLR does not need to compile the JIT code to native code. 

To be clear this is not AOT. I don't know how this is different from AOT, because there are similarities. But the purpose of Ready to Run is to improve startup time. Anyway, the only change I made to my `csproj` file is by adding the below code, then ran publish like normal.

```xml
<PropertyGroup>
	<RuntimeIdentifier>linux-x64</RuntimeIdentifier>
	<PublishReadyToRun>true</PublishReadyToRun>
</PropertyGroup>
```

Now the published app moved up to 8.89 MB, which was 3.88 MB when zipped. It also does not have the `runtimes` folder, so no need to delete anything from there. There was 1 pdb file, which I elected to not delete because it's small and I forgot to do it.

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

This is a little better than the default. 3 of the tests are over 1 second, instead of 2 like the original. But the range is different among the test of the tests, from 442 to 526 ms. The range is less than 100 ms, and those ms responses are quicker. This is promising.

## AOT

With the release of .NET 8, AOT is all the rage these days. So let's try it by adding the below to the csproj file, and removing what we added for Ready to Run in the previous test.

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

Quick refresher. When the Function App loads the code for the Azure Function, it downloads the zip file from a blob in an Azure Storage Account. Up until now, the Function App was using a SAS token for the authorization to be allowed to download the file. 

In this test we will be using a Managed Identityon the Function App and granting it the Blob Storage Contributor permissions on the storage account so it can get zip file. 

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


Those timings are CRAZY. About 10x slower than using a SAS token. I never would have expected this. 

If I were to guess why it's slower, I assume the underlying system that gets the auth token is making a call to some other system and that call is what's running slow. But I have no idea if that's true. Again, just guessing. If you know why this is much slower, please reach out to me on some form of social media (links on the About page!).

## A Real Life Example

I have a personal project that has grown to become a full application. Here are some stats about the application:

- .NET 8 Isolated
- Uses Managed Identity to load application from zip
- Runs on Linux
- NOT Ready to Run
- 76.5 MB unzipped and 26.4 MB zipped
- 234 Files
  - 7 of those are Windows specific .DLLs
  - 78 of those are .DLLs (not sure why these are here, likely from a 3rd party Nuget, another thing for me to)
  - 2 of those are .PDBs




## Test Code

Below is the code I used to test the cold boot timings. There are some hard coded strings in there, so make sure to change those values if you plan to run this code yourself. I have already deleted the resource group these services were hosted in, so don't think you'll mess with my experiment in some way.

```csharp
using System.Diagnostics;

const int StopWaitSeconds = 10;
const int StartWaitSeconds = 60;

var watch = new Stopwatch();
var client = new HttpClient();

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

for (int i = 0; i < 10; i++)
{
    var runId = i + 1;
    Console.WriteLine($"Testing Run: {runId}");
    Console.WriteLine("Stopping Function App");
    var stopProcess = new Process()
    {
        StartInfo = stopArgs
    };
    stopProcess.Start();
    await stopProcess.WaitForExitAsync();

    Console.WriteLine($"Waiting {StopWaitSeconds} seconds for stop");
    await Task.Delay(TimeSpan.FromSeconds(StopWaitSeconds));

    Console.WriteLine("Starting Function App");
    var startProcess = new Process()
    {
        StartInfo = startArgs
    };
    startProcess.Start();
    await startProcess.WaitForExitAsync();

    Console.WriteLine($"Waiting {StartWaitSeconds} seconds to start");
    await Task.Delay(TimeSpan.FromSeconds(StartWaitSeconds));

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


