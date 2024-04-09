Title: Why Azure Managed Identities are Awesome
Published: 4/7/2024
Tags: 
- Azure
- Security
- Identity
---

# Alternate Titles
    - Azure Managed Identities: Security by Default

## TL;DR

Azure Managed Identities are an identity your Azure hosted app can use with RBAC permissions. Create an identity, assign permissions to it, and assign the identity to an application hosted in Azure. Then, make a few app code changes so it uses the identity. Now you don't need a connection string to interact with Azure resources like Key Vault, Storage Accounts, etc.

Microsoft has a ton of documentation at: https://learn.microsoft.com/en-us/entra/identity/managed-identities-azure-resources/overview

## What are Azure Managed Identities?

An Azure Managed Identity is an account assigned to an application. In the on-prem world like Windows, you can think of it like a service account. An account that has a name, has permissions assigned to it, and is used by an app to get those permissions.

The big difference is that an Azure Managed Identity doesn't need a password. It's all managed by Azure, hence the word "Managed".

## What about Service Principals?

You may have heard of Service Principals before. If you haven't, feel free to skip this section, it's not that important. A Managed Identity is a type of Service Principal. It's an abstraction to simplify using them. With a Service Principal you are managing the client secrets. You have to track the remaining lifetime of the client secret, and before it expires you have to cycle it and update all apps that use it. All of that is manual work a person has to do. Managed Identities take care of all that work for you.

## How are Managed Identities created?

You have 2 options for creating them. Called a User Assigned, and System Assigned Managed Identities. 

## Now that I know all that, why should I use a Managed Identity?

Because you don't have secrets in your codebase. So you can't leak the secrets. So you don't have to manage the secrets, so you don't have to rotate them, so you don't have to worry someone has it when they shouldn't.

So many of us, myself included, have worked somewhere that lets connection strings or access tokens leak to anyone within the company. Once a secret leaks (and it will, just ask Troy Hunt of HaveIBeenPwned.com), you have to track down

A Managed Identity still uses a token internally. But we're abstracted away from it so we don't even see it. And even better, the token is short lived. 

## You've convinced me. How do I used a Managed Identity?

Thankfully that's pretty easy. Below are two C# code samples creating an instance of `BlobServiceClient`. I've only done this with C#, but I'm told the concept is the same across other languages used in Azure apps like JavaScript/TypeScript, Python, Java, etc. First sample uses a connection string to connect to a blob service and the second does the same thing using the `DefaultAzureCredential` class for authentication. When your app runs, the instance of `DefaultAzureCredential` checks a couple of places for credentials. It checks for environment variables, managed identities, local sign-ins of Azure PowerShell or the Azure CLI.

This means when the app runs in Azure, `DefaultAzureCredential` will find the Managed Identity and when the app runs locally, it will use your developer credentials. So now your code doesn't change across environments, and you don't have to have a token stored locally, or shared across the dev team.

```csharp
var connectionString = "DefaultEndpointsProtocol=https;AccountName=MYSTORAGEACCOUNT;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;EndpointSuffix=core.windows.net";
var container = new BlobServiceClient(connectionString);
```

```csharp
var accountUri = new Uri("https://MYSTORAGEACCOUNT.blob.core.windows.net/my-container");
var client = new BlobServiceClient(accountUri, new DefaultAzureCredential());
```

## What about Azure Managed SQL?

It works there too! But it does work differently. You have to enable a feature on the SQL Server to connect to Entra, and then run some queries to let the database know what database permissions to give to the identity. You can read the tutorial with Microsoft's documentation here: https://learn.microsoft.com/en-us/azure/app-service/tutorial-connect-msi-sql-database?tabs=windowsclient%2Cefcore%2Cdotnet
