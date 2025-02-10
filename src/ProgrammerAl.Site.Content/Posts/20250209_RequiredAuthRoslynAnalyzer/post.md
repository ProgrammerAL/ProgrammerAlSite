Title: "New Project: Required Auth Analyzer"
Published: 2025/02/09
Tags: 
- Project
- C#
- Roslyn Analyzer
---

## New Project: Required Auth Analyzer

I've always worried about missing an auth attribute on an ASP.NET Core endpoint and leaving the API open to anyone to abuse. You can set a flag to require auth for the entire site, and then set auth requirements for other endpoints, but that doesn't work when you have more complex auth requirements. For example, different endpoints requiring different levels of authorization. Some endpoitns are anonymous, some require a signed in user, some require an admin, etc.

So I created a C# Roslyn Analyzer to check each ASP.NET Core endpoint and require it to have an attribute which specifies what auth, if any, is required to access that endpoint. It's makes code more verbose, but I feel it's easier for developers to remember what is required for the endpoint by just looking at the code in one spot. It's all right there on the endpoint.

The full details are in the project README at: https://github.com/ProgrammerAL/required-auth.analyzer. 

## NuGet Package

The NuGet package is hosted on nuget.org. You can get it from: https://www.nuget.org/packages/ProgrammerAL.Analyzers.RequiredAuthAnalyzer
