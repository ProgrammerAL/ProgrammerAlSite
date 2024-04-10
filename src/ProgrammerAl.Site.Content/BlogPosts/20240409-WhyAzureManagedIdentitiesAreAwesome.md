Title: Azure Managed Identities: No more secrets
Published: 2024/04/09
Tags: 
- Azure
- Security
- Identity
---

## Secrets are a hassle

It's very easy for our applications to be littered with secrets. Connection strings, access tokens, passwords, etc. We can't escape them, but we can avoid them where possible. Tracking and managing application secrets is a chore no one likes to do and is so easy to mess up.

In this post we'll talk about Azure Managed Identities. A feature that lets applications hosted in Azure interact with other resources in Azure without using a plain text secret. You can think of it kind of like a Service Account. It has permissions assigned to it, and it grants those permissions to whichever apps are using it. The big difference is that an Azure Managed Identity doesn't need a password. It's all taken care of by Azure, hence the word "Managed".

## TL;DR

Azure Managed Identities are an identity your Azure hosted app can use with RBAC permissions. Create an identity, assign permissions to it, and assign the identity to an application hosted in Azure. Then make a few code changes so your app uses the identity. Now you don't need a connection string to interact with Azure resources like Key Vault, Storage Accounts, etc.

Microsoft has a ton of documentation at: https://learn.microsoft.com/en-us/entra/identity/managed-identities-azure-resources/overview


## What about Service Principals?

You may have heard of Service Principals before. If you haven't, feel free to skip this section, it's not that important. A Managed Identity is a type of Service Principal. It's an abstraction to simplify using them. With a Service Principal you are managing the client secrets. A person has to track how long until the client secret expires, and before it expires that person has to cycle it and update all apps that use it. All of that is manual work that Managed Identities automate for you.

## Now that I know all that, why should I use a Managed Identity?

So you don't have to manage the secrets, so you don't have to rotate them, so you don't have to worry someone has it when they shouldn't. You don't have secrets in your codebase, chat history, emails, etc. You can't leak secrets you don't have.

So many of us, myself included, have worked somewhere that lets connection strings or access tokens leak to anyone within the company. Once a secret leaks (and it will), you have to cycle it to a new value, track down evey app that uses it, and update it to use the new value.

A Managed Identity still uses a token internally. But we're abstracted away from it so we don't even see it. And even better, the token is short lived. 

You can't get rid of all secrets, but you can limit how many are used.

## How are Managed Identities created?

You have 2 options for creating them, called User Assigned and System Assigned identities. User Assigned identities are just another resource in Azure. You can go into the Azure Portal and create a new Managed Identity resource. From there you can assign permissions to it, and then let as many applications use it.

The other option is a System Assigned identity. This is created on the application resource that will use it (Function App, Container App, App Service, etc) and can only be used by that application. To enable the System Assigned identity, go to the existing resource in the Azure Portal and click on the Identity option in the left-hand menu. On the default tab 'System assigned' selected, switch the Status toggle to On, then click save. A new Managed Identity will be created that is only used by this resource. You can then assign permissions to it just like any other identity.

Which one should you use? A User Assigned or a System Assigned identity? I guess it depends on how you manage them. My default choice is a System Assigned identity because it can only be assigned to one service. It also goes well with IaC like Pulumi. On the other hand, User Assigned identities feel more like groups. You assign permissions to one, and every app using it gets that set of permissions. But you have to make sure you don't add more permissions than are needed, and also need to remember to delete the identity when it's no longer in use. In my mind, User Assigned identities feel like more work.

## You've convinced me. How do I used a Managed Identity?

Thankfully that's pretty easy. Below are two C# code samples creating an instance of `BlobServiceClient`. I've only done this with C#, but I'm told the concept is the same across other languages used in Azure apps like JavaScript/TypeScript, Python, Java, etc. The first sample uses a connection string to connect to a blob service and the second does the same thing using the `DefaultAzureCredential` class for authentication. When your app runs, the instance of `DefaultAzureCredential` checks a couple of places for credentials. It checks for environment variables, managed identities, local sign-ins of Azure PowerShell or the Azure CLI.

This means when the app runs in Azure, `DefaultAzureCredential` will find the Managed Identity assigned to it. And when the app runs on a developer machine, it will use your developer credentials. Just lie before, your code doesn't change across environments, but now you don't have to have a token stored locally, or shared across the dev team.

```csharp
var connectionString = "DefaultEndpointsProtocol=https;AccountName=MYSTORAGEACCOUNT;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;EndpointSuffix=core.windows.net";
var container = new BlobServiceClient(connectionString);
```

```csharp
var accountUri = new Uri("https://MYSTORAGEACCOUNT.blob.core.windows.net/my-container");
var client = new BlobServiceClient(accountUri, new DefaultAzureCredential());
```

## Bonus: What about Azure Managed SQL?

It works there too! But differently. You have to enable a feature on the SQL Server to connect to Entra, and then run some queries to let the database know what database permissions to give to the identity. The concept is the same, but the steps are different. You can read the tutorial with Microsoft's documentation here: https://learn.microsoft.com/en-us/azure/app-service/tutorial-connect-msi-sql-database?tabs=windowsclient%2Cefcore%2Cdotnet
