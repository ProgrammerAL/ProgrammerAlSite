Title: Improving Azure Functions Cold Boot Time
Published: 2024/05/03
Tags: 
- blog
- Azure Functions
- Serverless
- Performance
---

## ???



Mention this tweet: https://twitter.com/paulyuki99/status/1783695782887170297


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

There's no easy way to test boot time. I created a small app that will stop my Azure Function App, wait 10 seconds, start the app, wait 10 seconds, then send an HTTP request to the function. This loops 10 times. I ran this on my local machine in Release mode. The code I used is below in the final section. 

Finally, it's important to remember that we're testing cold boot time of a Azure Function App. What our app does at startup is our own issue, not Azure's. That's why we will be testing with a minimal Azure Function App.

## Test 1: Defaults

The default version of the app is 5.67 MB, or 2.4 MB when zipped. It also contains a total of 54 files.

Deploying the app and testing, we get the below timings. I guess the first few are slow to start up, giving us some outliers, but after that they normalize to a 300 ms range, between 480 and 780 ms. 

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 2,136 |
| 2 | 2,953 |
| 3 | 605 |
| 4 | 727 |
| 5 | 777 |
| 6 | 567 |
| 7 | 684 |
| 8 | 627 |
| 9 | 487 |
| 10 | 571 |

## Test 2: Delete Unnecessary Files

Part of the cold boot is network activity. The Azure Function App runtime needs to download the zip package, and then unzip the files. So the theory is the less bits we have to transfer over the network, the better.

I've seen a handful of Azure Functions projects that include PDB files in the output. The default app seems to only have 1. The published app also has a `runtimes` folder which has some files for Windows and Linux. Since we know this will only run on Linux, I deleted the Windows folder. This removed 5 dlls.

The new version has 48 files for a total size of 4.99 MB which is 2.07 MB when zipped. For a savings of 0.33 MB I don't expect much of an improvement.

| Test Id | Time in ms |
| :---------: | :-------: |
| 1 | 827 |
| 2 | 687 |
| 3 | 585 |
| 4 | 679 |
| 5 | 706 |
| 6 | 628 |
| 7 | 627 |
| 8 | 1,856 |
| 9 | 644 |
| 10 | 598 |

That is slightly worse. Ignoring the 1 outlier, we have a range of 585 to 827 ms. Which is a range just under 300 ms. This is basically a rounding error. 

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
| 1 | 1,804 |
| 2 | 1,956 |
| 3 | 443 |
| 4 | 511 |
| 5 | 1,712 |
| 6 | 442 |
| 7 | 1,750 |
| 8 | 458 |
| 9 | 526 |
| 10 | 498 |

This is a little better than the default. 3 of the tests are over 1 second, instead of 2 like the original. But the range is different among the test of the tests, from 442 to 526 ms. The range is less than 100 ms, and those ms responses are quicker. This is promising.

## Other Flags

AOT
PublishTrimmed
SelfContained

None of them worked.

## Loading Functions with a Managed Identity

## ???AOT???

```xml
<PropertyGroup>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <PublishAot>true</PublishAot>
</PropertyGroup>
```

Note: I had to use WSL to compile to Linux.

12 files at 103 MB, which was 32.9 MB when zipped.


Unfortunatly I was not able to get this to run. I guess AOT isn't supported for .NET 8 Azure Functions on Linux right now. If this is supported in the future I may remember to update this.

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


