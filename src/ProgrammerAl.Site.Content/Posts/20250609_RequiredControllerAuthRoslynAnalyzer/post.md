Title: "New Project: Required Controller Auth Analyzer"
Published: 2025/06/09
Tags: 
- Project
- C#
- Roslyn Analyzer
---

## New Project: Required Controller Auth Analyzer

A few months ago I created a C# Roslyn Analyzer to scan ASP.NET endpoints and add a compiler warning if an endpoint didn't have auth specified with an attribute ([Authorize] or [AllowAnonymous]) or some other way. [The old post is here](/posts/20250209_RequiredAuthRoslynAnalyzer)

Requiring it on each endpoint is a bit strict. Some people prefer to only require it at the controller level. So now I've created a new Roslyn Analyzer for just that. It will add a compiler warning if there are no Auth attributes at either the Controller level, OR the individual endpoints.

The full details are in the project README at: https://github.com/ProgrammerAL/required-auth.controller.analyzer

## NuGet Package

The NuGet package is hosted on nuget.org. You can get it from: https://www.nuget.org/packages/ProgrammerAL.Analyzers.ControllerRequiredAuthAnalyzer
